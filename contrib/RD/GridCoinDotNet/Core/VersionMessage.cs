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
using System.Net;
using System.Text;
using BitCoinSharp.Common;
using BitCoinSharp.IO;

namespace BitCoinSharp
{
    [Serializable]
    public class VersionMessage : Message
    {
        /// <summary>
        /// A services flag that denotes whether the peer has a copy of the block chain or not.
        /// </summary>
        public const int NodeNetwork = 1;

        /// <summary>
        /// The version number of the protocol spoken.
        /// </summary>
        public uint ClientVersion { get; private set; }

        /// <summary>
        /// Flags defining what is supported. Right now <see cref="NodeNetwork"/> is the only flag defined.
        /// </summary>
        public ulong LocalServices { get; private set; }

        /// <summary>
        /// What the other side believes the current time to be, in seconds.
        /// </summary>
        public ulong Time { get; private set; }

        /// <summary>
        /// What the other side believes the address of this program is. Not used.
        /// </summary>
        public PeerAddress MyAddr { get; private set; }

        /// <summary>
        /// What the other side believes their own address is. Not used.
        /// </summary>
        public PeerAddress TheirAddr { get; private set; }

        private ulong _localHostNonce;

        /// <summary>
        /// An additional string that today the official client sets to the empty string. We treat it as something like an
        /// HTTP User-Agent header.
        /// </summary>
        public string SubVer { get; private set; }

        /// <summary>
        /// How many blocks are in the chain, according to the other side.
        /// </summary>
        public uint BestHeight { get; private set; }

        /// <exception cref="ProtocolException"/>
        public VersionMessage(NetworkParameters @params, byte[] msg)
            : base(@params, msg, 0)
        {
        }

        public VersionMessage(NetworkParameters @params, uint newBestHeight)
            : base(@params)
        {
            ClientVersion = NetworkParameters.ProtocolVersion;
            LocalServices = 0;
            Time = UnixTime.ToUnixTime(DateTime.UtcNow);
            // Note that the official client doesn't do anything with these, and finding out your own external IP address
            // is kind of tricky anyway, so we just put nonsense here for now.
            MyAddr = new PeerAddress(IPAddress.Loopback, @params.Port, 0);
            TheirAddr = new PeerAddress(IPAddress.Loopback, @params.Port, 0);
            SubVer = "BitCoinSharp 0.3-SNAPSHOT";
            BestHeight = newBestHeight;
        }

        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            ClientVersion = ReadUint32();
            LocalServices = ReadUint64();
            Time = ReadUint64();
            MyAddr = new PeerAddress(Params, Bytes, Cursor, 0);
            Cursor += MyAddr.MessageSize;
            TheirAddr = new PeerAddress(Params, Bytes, Cursor, 0);
            Cursor += TheirAddr.MessageSize;
            // uint64 localHostNonce  (random data)
            // We don't care about the localhost nonce. It's used to detect connecting back to yourself in cases where
            // there are NATs and proxies in the way. However we don't listen for inbound connections so it's irrelevant.
            _localHostNonce = ReadUint64();
            //   string subVer  (currently "")
            SubVer = ReadStr();
            //   int bestHeight (size of known block chain).
            BestHeight = ReadUint32();
        }

        /// <exception cref="IOException"/>
        public override void BitcoinSerializeToStream(Stream buf)
        {
            Utils.Uint32ToByteStreamLe(ClientVersion, buf);
            Utils.Uint64ToByteStreamLe(LocalServices, buf);
            Utils.Uint64ToByteStreamLe(Time, buf);
            // My address.
            MyAddr.BitcoinSerializeToStream(buf);
            // Their address.
            TheirAddr.BitcoinSerializeToStream(buf);
            // Next up is the "local host nonce", this is to detect the case of connecting
            // back to yourself. We don't care about this as we won't be accepting inbound
            // connections.
            Utils.Uint64ToByteStreamLe(_localHostNonce, buf);
            // Now comes subVer.
            var subVerBytes = Encoding.UTF8.GetBytes(SubVer);
            buf.Write(new VarInt((ulong) subVerBytes.Length).Encode());
            buf.Write(subVerBytes);
            // Size of known block chain.
            Utils.Uint32ToByteStreamLe(BestHeight, buf);
        }

        /// <summary>
        /// Returns true if the version message indicates the sender has a full copy of the block chain,
        /// or if it's running in client mode (only has the headers).
        /// </summary>
        public bool HasBlockChain()
        {
            return (LocalServices & NodeNetwork) == NodeNetwork;
        }
    }
}