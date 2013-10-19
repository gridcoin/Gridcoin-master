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
using System.IO;
using System.Text;

namespace BitCoinSharp
{
    /// <summary>
    /// A Message is a data structure that can be serialized/deserialized using both the BitCoin proprietary serialization
    /// format and built-in Java object serialization. Specific types of messages that are used both in the block chain,
    /// and on the wire, are derived from this class.
    /// </summary>
    /// <remarks>
    /// This class is not useful for library users. If you want to talk to the network see the <see cref="Peer"/> class.
    /// </remarks>
    [Serializable]
    public abstract class Message
    {
        public const uint MaxSize = 0x2000000;

        [NonSerialized] private int _offset;
        [NonSerialized] private int _cursor;
        [NonSerialized] private byte[] _bytes;
        [NonSerialized] private uint _protocolVersion;

        // The offset is how many bytes into the provided byte array this message starts at.
        protected int Offset
        {
            get { return _offset; }
            private set { _offset = value; }
        }

        // The cursor keeps track of where we are in the byte array as we parse it.
        // Note that it's relative to the start of the array NOT the start of the message.
        protected int Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }

        // The raw message bytes themselves.
        protected byte[] Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        protected uint ProtocolVersion
        {
            get { return _protocolVersion; }
            set { _protocolVersion = value; }
        }

        // This will be saved by subclasses that implement Serializable.
        protected NetworkParameters Params { get; private set; }

        /// <summary>
        /// This exists for the Java serialization framework to use only.
        /// </summary>
        protected Message()
        {
        }

        internal Message(NetworkParameters @params)
        {
            Params = @params;
        }

        /// <exception cref="ProtocolException"/>
        internal Message(NetworkParameters @params, byte[] msg, int offset, uint protocolVersion = NetworkParameters.ProtocolVersion)
        {
            ProtocolVersion = protocolVersion;
            Params = @params;
            Bytes = msg;
            Cursor = Offset = offset;
            Parse();
#if SELF_CHECK
    // Useful to ensure serialize/deserialize are consistent with each other.
            if (GetType() != typeof (VersionMessage))
            {
                var msgbytes = new byte[Cursor - offset];
                Array.Copy(msg, offset, msgbytes, 0, Cursor - offset);
                var reserialized = BitcoinSerialize();
                if (!reserialized.SequenceEqual(msgbytes))
                    throw new Exception("Serialization is wrong: " + Environment.NewLine +
                                        Utils.BytesToHexString(reserialized) + " vs " + Environment.NewLine +
                                        Utils.BytesToHexString(msgbytes));
            }
#endif
            Bytes = null;
        }

        // These methods handle the serialization/deserialization using the custom BitCoin protocol.
        // It's somewhat painful to work with in Java, so some of these objects support a second
        // serialization mechanism - the standard Java serialization system. This is used when things
        // are serialized to the wallet.
        /// <exception cref="ProtocolException"/>
        protected abstract void Parse();

        public virtual byte[] BitcoinSerialize()
        {
            using (var stream = new MemoryStream())
            {
                BitcoinSerializeToStream(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Serializes this message to the provided stream. If you just want the raw bytes use bitcoinSerialize().
        /// </summary>
        /// <exception cref="IOException"/>
        public virtual void BitcoinSerializeToStream(Stream stream)
        {
        }

        internal int MessageSize
        {
            get { return Cursor - Offset; }
        }

        internal uint ReadUint32()
        {
            var u = Utils.ReadUint32(Bytes, Cursor);
            Cursor += 4;
            return u;
        }

        internal Sha256Hash ReadHash()
        {
            var hash = new byte[32];
            Array.Copy(Bytes, Cursor, hash, 0, 32);
            // We have to flip it around, as it's been read off the wire in little endian.
            // Not the most efficient way to do this but the clearest.
            hash = Utils.ReverseBytes(hash);
            Cursor += 32;
            return new Sha256Hash(hash);
        }

        internal ulong ReadUint64()
        {
            return (((ulong) Bytes[Cursor++]) << 0) |
                   (((ulong) Bytes[Cursor++]) << 8) |
                   (((ulong) Bytes[Cursor++]) << 16) |
                   (((ulong) Bytes[Cursor++]) << 24) |
                   (((ulong) Bytes[Cursor++]) << 32) |
                   (((ulong) Bytes[Cursor++]) << 40) |
                   (((ulong) Bytes[Cursor++]) << 48) |
                   (((ulong) Bytes[Cursor++]) << 56);
        }

        internal ulong ReadVarInt()
        {
            var varint = new VarInt(Bytes, Cursor);
            Cursor += varint.SizeInBytes;
            return varint.Value;
        }

        internal byte[] ReadBytes(int length)
        {
            var b = new byte[length];
            Array.Copy(Bytes, Cursor, b, 0, length);
            Cursor += length;
            return b;
        }

        internal string ReadStr()
        {
            var varInt = new VarInt(Bytes, Cursor);
            if (varInt.Value == 0)
            {
                Cursor += 1;
                return "";
            }
            var characters = new byte[varInt.Value];
            Array.Copy(Bytes, Cursor, characters, 0, characters.Length);
            Cursor += varInt.SizeInBytes;
            return Encoding.UTF8.GetString(characters, 0, characters.Length);
        }
    }
}