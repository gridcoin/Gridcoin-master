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

namespace BitCoinSharp
{
    [Serializable]
    public class AddressMessage : Message
    {
        private const ulong _maxAddresses = 1024;

        internal IList<PeerAddress> Addresses { get; private set; }

        /// <exception cref="ProtocolException"/>
        internal AddressMessage(NetworkParameters @params, byte[] payload, int offset)
            : base(@params, payload, offset)
        {
        }

        /// <exception cref="ProtocolException"/>
        internal AddressMessage(NetworkParameters @params, byte[] payload)
            : base(@params, payload, 0)
        {
        }

        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            var numAddresses = ReadVarInt();
            // Guard against ultra large messages that will crash us.
            if (numAddresses > _maxAddresses)
                throw new ProtocolException("Address message too large.");
            Addresses = new List<PeerAddress>((int) numAddresses);
            for (var i = 0UL; i < numAddresses; i++)
            {
                var addr = new PeerAddress(Params, Bytes, Cursor, ProtocolVersion);
                Addresses.Add(addr);
                Cursor += addr.MessageSize;
            }
        }

        public override void BitcoinSerializeToStream(Stream stream)
        {
            stream.Write(new VarInt((ulong) Addresses.Count).Encode());
            foreach (var addr in Addresses)
            {
                addr.BitcoinSerializeToStream(stream);
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("addr: ");
            foreach (var a in Addresses)
            {
                builder.Append(a.ToString());
                builder.Append(" ");
            }
            return builder.ToString();
        }
    }
}