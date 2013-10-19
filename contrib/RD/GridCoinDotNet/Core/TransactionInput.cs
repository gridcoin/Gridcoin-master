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
using System.Diagnostics;
using System.IO;
using BitCoinSharp.IO;

namespace BitCoinSharp
{
    /// <summary>
    /// A transfer of coins from one address to another creates a transaction in which the outputs
    /// can be claimed by the recipient in the input of another transaction. You can imagine a
    /// transaction as being a module which is wired up to others, the inputs of one have to be wired
    /// to the outputs of another. The exceptions are coinbase transactions, which create new coins.
    /// </summary>
    [Serializable]
    public class TransactionInput : Message
    {
        public static readonly byte[] EmptyArray = new byte[0];

        // Allows for altering transactions after they were broadcast. Tx replacement is currently disabled in the C++
        // client so this is always the UINT_MAX.
        // TODO: Document this in more detail and build features that use it.
        private uint _sequence;
        // Data needed to connect to the output of the transaction we're gathering coins from.
        internal TransactionOutPoint Outpoint { get; private set; }
        // The "script bytes" might not actually be a script. In coinbase transactions where new coins are minted there
        // is no input transaction, so instead the scriptBytes contains some extra stuff (like a rollover nonce) that we
        // don't care about much. The bytes are turned into a Script object (cached below) on demand via a getter.
        internal byte[] ScriptBytes { get; set; }
        // The Script object obtained from parsing scriptBytes. Only filled in on demand and if the transaction is not
        // coinbase.
        [NonSerialized] private Script _scriptSig;
        // A pointer to the transaction that owns this input.
        internal Transaction ParentTransaction { get; private set; }

        /// <summary>
        /// Used only in creation of the genesis block.
        /// </summary>
        internal TransactionInput(NetworkParameters @params, Transaction parentTransaction, byte[] scriptBytes)
            : base(@params)
        {
            ScriptBytes = scriptBytes;
            Outpoint = new TransactionOutPoint(@params, -1, null);
            _sequence = uint.MaxValue;
            ParentTransaction = parentTransaction;
        }

        /// <summary>
        /// Creates an UNSIGNED input that links to the given output
        /// </summary>
        internal TransactionInput(NetworkParameters @params, Transaction parentTransaction, TransactionOutput output)
            : base(@params)
        {
            var outputIndex = output.Index;
            Outpoint = new TransactionOutPoint(@params, outputIndex, output.ParentTransaction);
            ScriptBytes = EmptyArray;
            _sequence = uint.MaxValue;
            ParentTransaction = parentTransaction;
        }

        /// <summary>
        /// Deserializes an input message. This is usually part of a transaction message.
        /// </summary>
        /// <exception cref="ProtocolException"/>
        public TransactionInput(NetworkParameters @params, Transaction parentTransaction, byte[] payload, int offset)
            : base(@params, payload, offset)
        {
            ParentTransaction = parentTransaction;
        }

        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            Outpoint = new TransactionOutPoint(Params, Bytes, Cursor);
            Cursor += Outpoint.MessageSize;
            var scriptLen = (int) ReadVarInt();
            ScriptBytes = ReadBytes(scriptLen);
            _sequence = ReadUint32();
        }

        /// <exception cref="IOException"/>
        public override void BitcoinSerializeToStream(Stream stream)
        {
            Outpoint.BitcoinSerializeToStream(stream);
            stream.Write(new VarInt((ulong) ScriptBytes.Length).Encode());
            stream.Write(ScriptBytes);
            Utils.Uint32ToByteStreamLe(_sequence, stream);
        }

        /// <summary>
        /// Coinbase transactions have special inputs with hashes of zero. If this is such an input, returns true.
        /// </summary>
        public bool IsCoinBase
        {
            get { return Outpoint.Hash.Equals(Sha256Hash.ZeroHash); }
        }

        /// <summary>
        /// Returns the input script.
        /// </summary>
        /// <exception cref="ScriptException"/>
        public Script ScriptSig
        {
            get
            {
                // Transactions that generate new coins don't actually have a script. Instead this
                // parameter is overloaded to be something totally different.
                if (_scriptSig == null)
                {
                    Debug.Assert(ScriptBytes != null);
                    _scriptSig = new Script(Params, ScriptBytes, 0, ScriptBytes.Length);
                }
                return _scriptSig;
            }
        }

        /// <summary>
        /// Convenience method that returns the from address of this input by parsing the scriptSig.
        /// </summary>
        /// <exception cref="ScriptException">If the scriptSig could not be understood (eg, if this is a coinbase transaction).</exception>
        public Address FromAddress
        {
            get
            {
                Debug.Assert(!IsCoinBase);
                return ScriptSig.FromAddress;
            }
        }

        /// <summary>
        /// Returns a human readable debug string.
        /// </summary>
        public override string ToString()
        {
            if (IsCoinBase)
            {
                return "TxIn: COINBASE";
            }
            return "TxIn from tx " + Outpoint + " (pubkey: " + Utils.BytesToHexString(ScriptSig.PubKey) + ") script:" + ScriptSig;
        }

        internal enum ConnectionResult
        {
            NoSuchTx,
            AlreadySpent,
            Success
        }

        // TODO: Clean all this up once TransactionOutPoint disappears.

        /// <summary>
        /// Locates the referenced output from the given pool of transactions.
        /// </summary>
        /// <returns>The TransactionOutput or null if the transactions map doesn't contain the referenced tx.</returns>
        internal TransactionOutput GetConnectedOutput(IDictionary<Sha256Hash, Transaction> transactions)
        {
            Transaction tx;
            if (!transactions.TryGetValue(Outpoint.Hash, out tx))
                return null;
            var @out = tx.Outputs[Outpoint.Index];
            return @out;
        }

        /// <summary>
        /// Connects this input to the relevant output of the referenced transaction if it's in the given map.
        /// Connecting means updating the internal pointers and spent flags.
        /// </summary>
        /// <param name="transactions">Map of txhash-&gt;transaction.</param>
        /// <param name="disconnect">Whether to abort if there's a pre-existing connection or not.</param>
        /// <returns>True if connection took place, false if the referenced transaction was not in the list.</returns>
        internal ConnectionResult Connect(IDictionary<Sha256Hash, Transaction> transactions, bool disconnect)
        {
            Transaction tx;
            if (!transactions.TryGetValue(Outpoint.Hash, out tx))
                return ConnectionResult.NoSuchTx;
            var @out = tx.Outputs[Outpoint.Index];
            if (!@out.IsAvailableForSpending)
            {
                if (disconnect)
                    @out.MarkAsUnspent();
                else
                    return ConnectionResult.AlreadySpent;
            }
            Outpoint.FromTx = tx;
            @out.MarkAsSpent(this);
            return ConnectionResult.Success;
        }

        /// <summary>
        /// Release the connected output, making it spendable once again.
        /// </summary>
        /// <returns>True if the disconnection took place, false if it was not connected.</returns>
        internal bool Disconnect()
        {
            if (Outpoint.FromTx == null) return false;
            Outpoint.FromTx.Outputs[Outpoint.Index].MarkAsUnspent();
            Outpoint.FromTx = null;
            return true;
        }
    }
}