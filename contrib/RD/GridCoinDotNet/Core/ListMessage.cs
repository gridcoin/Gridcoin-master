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

using System.Collections.Generic;
using System.IO;
using BitCoinSharp.IO;

namespace BitCoinSharp
{
    /// <summary>
    /// Abstract super class of classes with list based payload, i.e. InventoryMessage and GetDataMessage.
    /// </summary>
    public abstract class ListMessage : Message
    {
        // For some reason the compiler complains if this is inside InventoryItem
        private IList<InventoryItem> _items;

        private const ulong _maxInventoryItems = 50000;

        /// <exception cref="ProtocolException"/>
        protected ListMessage(NetworkParameters @params, byte[] bytes)
            : base(@params, bytes, 0)
        {
        }

        protected ListMessage(NetworkParameters @params)
            : base(@params)
        {
            _items = new List<InventoryItem>();
        }

        public IList<InventoryItem> Items
        {
            get { return _items; }
        }

        public void AddItem(InventoryItem item)
        {
            _items.Add(item);
        }

        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            // An inv is vector<CInv> where CInv is int+hash. The int is either 1 or 2 for tx or block.
            var arrayLen = ReadVarInt();
            if (arrayLen > _maxInventoryItems)
                throw new ProtocolException("Too many items in INV message: " + arrayLen);
            _items = new List<InventoryItem>((int) arrayLen);
            for (var i = 0UL; i < arrayLen; i++)
            {
                if (Cursor + 4 + 32 > Bytes.Length)
                {
                    throw new ProtocolException("Ran off the end of the INV");
                }
                var typeCode = ReadUint32();
                InventoryItem.ItemType type;
                // See ppszTypeName in net.h
                switch (typeCode)
                {
                    case 0:
                        type = InventoryItem.ItemType.Error;
                        break;
                    case 1:
                        type = InventoryItem.ItemType.Transaction;
                        break;
                    case 2:
                        type = InventoryItem.ItemType.Block;
                        break;
                    default:
                        throw new ProtocolException("Unknown CInv type: " + typeCode);
                }
                var item = new InventoryItem(type, ReadHash());
                _items.Add(item);
            }
            Bytes = null;
        }

        /// <exception cref="IOException"/>
        public override void BitcoinSerializeToStream(Stream stream)
        {
            stream.Write(new VarInt((ulong) _items.Count).Encode());
            foreach (var i in _items)
            {
                // Write out the type code.
                Utils.Uint32ToByteStreamLe((uint) i.Type, stream);
                // And now the hash.
                stream.Write(Utils.ReverseBytes(i.Hash.Bytes));
            }
        }
    }
}