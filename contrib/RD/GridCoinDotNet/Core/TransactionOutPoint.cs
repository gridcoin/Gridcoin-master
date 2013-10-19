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
using System.Diagnostics;
using System.IO;
using BitCoinSharp.IO;

namespace BitCoinSharp
{
    // TODO: Fold this class into the TransactionInput class. It's not necessary.

    /// <summary>
    /// This message is a reference or pointer to an output of a different transaction.
    /// </summary>
    [Serializable]
    public class TransactionOutPoint : Message
    {
        /// <summary>
        /// Hash of the transaction to which we refer.
        /// </summary>
        internal Sha256Hash Hash { get; set; }

        /// <summary>
        /// Which output of that transaction we are talking about.
        /// </summary>
        internal int Index { get; private set; }

        // This is not part of BitCoin serialization. It's included in Java serialization.
        // It points to the connected transaction.
        internal Transaction FromTx { get; set; }

        internal TransactionOutPoint(NetworkParameters @params, int index, Transaction fromTx)
            : base(@params)
        {
            Index = index;
            if (fromTx != null)
            {
                Hash = fromTx.Hash;
                FromTx = fromTx;
            }
            else
            {
                // This happens when constructing the genesis block.
                Hash = Sha256Hash.ZeroHash;
            }
        }

        /// <summary>
        /// Deserializes the message. This is usually part of a transaction message.
        /// </summary>
        /// <exception cref="ProtocolException"/>
        public TransactionOutPoint(NetworkParameters @params, byte[] payload, int offset)
            : base(@params, payload, offset)
        {
        }

        // All zeros.
        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            Hash = ReadHash();
            Index = (int) ReadUint32();
        }

        /// <exception cref="IOException"/>
        public override void BitcoinSerializeToStream(Stream stream)
        {
            stream.Write(Utils.ReverseBytes(Hash.Bytes));
            Utils.Uint32ToByteStreamLe((uint) Index, stream);
        }

        /// <summary>
        /// If this transaction was created using the explicit constructor rather than deserialized,
        /// retrieves the connected output transaction. Asserts if there is no connected transaction.
        /// </summary>
        internal TransactionOutput ConnectedOutput
        {
            get { return FromTx != null ? FromTx.Outputs[Index] : null; }
        }

        /// <summary>
        /// Returns the pubkey script from the connected output.
        /// </summary>
        internal byte[] ConnectedPubKeyScript
        {
            get
            {
                var result = ConnectedOutput.ScriptBytes;
                Debug.Assert(result != null);
                Debug.Assert(result.Length > 0);
                return result;
            }
        }

        /// <summary>
        /// Convenience method to get the connected outputs pubkey hash.
        /// </summary>
        /// <exception cref="ScriptException"/>
        internal byte[] ConnectedPubKeyHash
        {
            get { return ConnectedOutput.ScriptPubKey.PubKeyHash; }
        }

        public override string ToString()
        {
            return "outpoint " + Index + ":" + Hash;
        }
    }
}