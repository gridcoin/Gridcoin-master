/*
 * Copyright 2011 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BitCoinSharp.IO;
using log4net;

namespace BitCoinSharp
{
    /// <summary>
    /// Methods to serialize and de-serialize messages to the BitCoin network format as defined in the BitCoin protocol
    /// specification at https://en.bitcoin.it/wiki/Protocol_specification
    /// </summary>
    /// <remarks>
    /// To be able to serialize and deserialize new Message subclasses the following criteria needs to be met.
    /// <ul>
    ///   <li>The proper Class instance needs to be mapped to it's message name in the names variable below</li>
    ///   <li>There needs to be a constructor matching: NetworkParameters params, byte[] payload</li>
    ///   <li>Message.bitcoinSerializeToStream() needs to be properly subclassed</li>
    /// </ul>
    /// </remarks>
    public class BitcoinSerializer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (BitcoinSerializer));
        private const int _commandLen = 12;

        private readonly NetworkParameters _params;
        private bool _usesChecksumming;

        private static readonly IDictionary<Type, string> _names = new Dictionary<Type, string>();

        static BitcoinSerializer()
        {
            _names.Add(typeof (VersionMessage), "version");
            _names.Add(typeof (InventoryMessage), "inv");
            _names.Add(typeof (Block), "block");
            _names.Add(typeof (GetDataMessage), "getdata");
            _names.Add(typeof (Transaction), "tx");
            _names.Add(typeof (AddressMessage), "addr");
            _names.Add(typeof (Ping), "ping");
            _names.Add(typeof (VersionAck), "verack");
            _names.Add(typeof (GetBlocksMessage), "getblocks");
        }

        /// <summary>
        /// Constructs a BitcoinSerializer with the given behavior.
        /// </summary>
        /// <param name="params">MetworkParams used to create Messages instances and determining packetMagic</param>
        /// <param name="usesChecksumming">Set to true if checksums should be included and expected in headers</param>
        public BitcoinSerializer(NetworkParameters @params, bool usesChecksumming)
        {
            _params = @params;
            _usesChecksumming = usesChecksumming;
        }

        public void UseChecksumming(bool usesChecksumming)
        {
            _usesChecksumming = usesChecksumming;
        }

        /// <summary>
        /// Writes message to to the output stream.
        /// </summary>
        /// <exception cref="IOException"/>
        public void Serialize(Message message, Stream @out)
        {
            string name;
            if (!_names.TryGetValue(message.GetType(), out name))
            {
                throw new Exception("BitcoinSerializer doesn't currently know how to serialize " + message.GetType());
            }

            var header = new byte[4 + _commandLen + 4 + (_usesChecksumming ? 4 : 0)];

            Utils.Uint32ToByteArrayBe(_params.PacketMagic, header, 0);

            // The header array is initialized to zero so we don't have to worry about
            // NULL terminating the string here.
            for (var i = 0; i < name.Length && i < _commandLen; i++)
            {
                header[4 + i] = (byte) name[i];
            }

            var payload = message.BitcoinSerialize();

            Utils.Uint32ToByteArrayLe((uint) payload.Length, header, 4 + _commandLen);

            if (_usesChecksumming)
            {
                var hash = Utils.DoubleDigest(payload);
                Array.Copy(hash, 0, header, 4 + _commandLen + 4, 4);
            }

            @out.Write(header);
            @out.Write(payload);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Sending {0} message: {1}", name, Utils.BytesToHexString(header) + Utils.BytesToHexString(payload));
        }

        /// <summary>
        /// Reads a message from the given InputStream and returns it.
        /// </summary>
        /// <exception cref="ProtocolException"/>
        /// <exception cref="IOException"/>
        public Message Deserialize(Stream @in)
        {
            // A BitCoin protocol message has the following format.
            //
            //   - 4 byte magic number: 0xfabfb5da for the testnet or
            //                          0xf9beb4d9 for production
            //   - 12 byte command in ASCII
            //   - 4 byte payload size
            //   - 4 byte checksum
            //   - Payload data
            //
            // The checksum is the first 4 bytes of a SHA256 hash of the message payload. It isn't
            // present for all messages, notably, the first one on a connection.
            //
            // Satoshi's implementation ignores garbage before the magic header bytes. We have to do the same because
            // sometimes it sends us stuff that isn't part of any message.
            SeekPastMagicBytes(@in);
            // Now read in the header.
            var header = new byte[_commandLen + 4 + (_usesChecksumming ? 4 : 0)];
            var readCursor = 0;
            while (readCursor < header.Length)
            {
                var bytesRead = @in.Read(header, readCursor, header.Length - readCursor);
                if (bytesRead == -1)
                {
                    // There's no more data to read.
                    throw new IOException("Socket is disconnected");
                }
                readCursor += bytesRead;
            }

            var cursor = 0;

            // The command is a NULL terminated string, unless the command fills all twelve bytes
            // in which case the termination is implicit.
            var mark = cursor;
            for (; header[cursor] != 0 && cursor - mark < _commandLen; cursor++)
            {
            }
            var commandBytes = new byte[cursor - mark];
            Array.Copy(header, mark, commandBytes, 0, commandBytes.Length);
            for (var i = 0; i < commandBytes.Length; i++)
            {
                // Emulate ASCII by replacing extended characters with question marks.
                if (commandBytes[i] >= 0x80)
                {
                    commandBytes[i] = 0x3F;
                }
            }
            var command = Encoding.UTF8.GetString(commandBytes, 0, commandBytes.Length);
            cursor = mark + _commandLen;

            var size = Utils.ReadUint32(header, cursor);
            cursor += 4;

            if (size > Message.MaxSize)
                throw new ProtocolException("Message size too large: " + size);

            // Old clients don't send the checksum.
            var checksum = new byte[4];
            if (_usesChecksumming)
            {
                // Note that the size read above includes the checksum bytes.
                Array.Copy(header, cursor, checksum, 0, 4);
            }

            // Now try to read the whole message.
            readCursor = 0;
            var payloadBytes = new byte[size];
            while (readCursor < payloadBytes.Length - 1)
            {
                var bytesRead = @in.Read(payloadBytes, readCursor, (int) (size - readCursor));
                if (bytesRead == -1)
                {
                    throw new IOException("Socket is disconnected");
                }
                readCursor += bytesRead;
            }

            // Verify the checksum.
            if (_usesChecksumming)
            {
                var hash = Utils.DoubleDigest(payloadBytes);
                if (checksum[0] != hash[0] || checksum[1] != hash[1] ||
                    checksum[2] != hash[2] || checksum[3] != hash[3])
                {
                    throw new ProtocolException("Checksum failed to verify, actual " +
                                                Utils.BytesToHexString(hash) +
                                                " vs " + Utils.BytesToHexString(checksum));
                }
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Received {0} byte '{1}' message: {2}",
                                 size,
                                 command,
                                 Utils.BytesToHexString(payloadBytes)
                    );
            }

            try
            {
                return MakeMessage(command, payloadBytes);
            }
            catch (Exception e)
            {
                throw new ProtocolException("Error deserializing message " + Utils.BytesToHexString(payloadBytes) + Environment.NewLine + e.Message, e);
            }
        }

        /// <exception cref="ProtocolException"/>
        private Message MakeMessage(string command, byte[] payloadBytes)
        {
            // We use an if ladder rather than reflection because reflection can be slow on some platforms.
            if (command.Equals("version"))
            {
                return new VersionMessage(_params, payloadBytes);
            }
            if (command.Equals("inv"))
            {
                return new InventoryMessage(_params, payloadBytes);
            }
            if (command.Equals("block"))
            {
                return new Block(_params, payloadBytes);
            }
            if (command.Equals("getdata"))
            {
                return new GetDataMessage(_params, payloadBytes);
            }
            if (command.Equals("tx"))
            {
                return new Transaction(_params, payloadBytes);
            }
            if (command.Equals("addr"))
            {
                return new AddressMessage(_params, payloadBytes);
            }
            if (command.Equals("ping"))
            {
                return new Ping();
            }
            if (command.Equals("verack"))
            {
                return new VersionAck(_params, payloadBytes);
            }
            throw new ProtocolException("No support for deserializing message with name " + command);
        }

        /// <exception cref="IOException"/>
        private void SeekPastMagicBytes(Stream @in)
        {
            var magicCursor = 3; // Which byte of the magic we're looking for currently.
            while (true)
            {
                var b = @in.Read(); // Read a byte.
                if (b == -1)
                {
                    // There's no more data to read.
                    throw new IOException("Socket is disconnected");
                }
                // We're looking for a run of bytes that is the same as the packet magic but we want to ignore partial
                // magics that aren't complete. So we keep track of where we're up to with magicCursor.
                var expectedByte = (byte) (_params.PacketMagic >> magicCursor*8);
                if (b == expectedByte)
                {
                    magicCursor--;
                    if (magicCursor < 0)
                    {
                        // We found the magic sequence.
                        return;
                    }
                    // We still have further to go to find the next message.
                }
                else
                {
                    magicCursor = 3;
                }
            }
        }
    }
}