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
using log4net;

namespace BitCoinSharp
{
    /// <summary>
    /// A TransactionOutput message contains a scriptPubKey that controls who is able to spend its value. It is a sub-part
    /// of the Transaction message.
    /// </summary>
    [Serializable]
    public class TransactionOutput : Message
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (TransactionOutput));

        // A transaction output has some value and a script used for authenticating that the redeemer is allowed to spend
        // this output.
        private ulong _value;
        private byte[] _scriptBytes;

        // The script bytes are parsed and turned into a Script on demand.
        [NonSerialized] private Script _scriptPubKey;

        // These fields are Java serialized but not BitCoin serialized. They are used for tracking purposes in our wallet
        // only. If set to true, this output is counted towards our balance. If false and spentBy is null the tx output
        // was owned by us and was sent to somebody else. If false and spentBy is true it means this output was owned by
        // us and used in one of our own transactions (eg, because it is a change output).
        private bool _availableForSpending;
        private TransactionInput _spentBy;

        // A reference to the transaction which holds this output.
        internal Transaction ParentTransaction { get; set; }

        /// <summary>
        /// Deserializes a transaction output message. This is usually part of a transaction message.
        /// </summary>
        /// <exception cref="ProtocolException"/>
        public TransactionOutput(NetworkParameters @params, Transaction parent, byte[] payload, int offset)
            : base(@params, payload, offset)
        {
            ParentTransaction = parent;
            _availableForSpending = true;
        }

        internal TransactionOutput(NetworkParameters @params, Transaction parent, ulong value, Address to)
            : base(@params)
        {
            _value = value;
            _scriptBytes = Script.CreateOutputScript(to);
            ParentTransaction = parent;
            _availableForSpending = true;
        }

        /// <summary>
        /// Used only in creation of the genesis blocks and in unit tests.
        /// </summary>
        internal TransactionOutput(NetworkParameters @params, Transaction parent, byte[] scriptBytes)
            : base(@params)
        {
            _scriptBytes = scriptBytes;
            _value = Utils.ToNanoCoins(50, 0);
            ParentTransaction = parent;
            _availableForSpending = true;
        }

        /// <exception cref="ScriptException"/>
        public Script ScriptPubKey
        {
            get { return _scriptPubKey ?? (_scriptPubKey = new Script(Params, _scriptBytes, 0, _scriptBytes.Length)); }
        }

        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            _value = ReadUint64();
            var scriptLen = (int) ReadVarInt();
            _scriptBytes = ReadBytes(scriptLen);
        }

        /// <exception cref="IOException"/>
        public override void BitcoinSerializeToStream(Stream stream)
        {
            Debug.Assert(_scriptBytes != null);
            Utils.Uint64ToByteStreamLe(Value, stream);
            // TODO: Move script serialization into the Script class, where it belongs.
            stream.Write(new VarInt((ulong) _scriptBytes.Length).Encode());
            stream.Write(_scriptBytes);
        }

        /// <summary>
        /// Returns the value of this output in nanocoins. This is the amount of currency that the destination address
        /// receives.
        /// </summary>
        public ulong Value
        {
            get { return _value; }
        }

        internal int Index
        {
            get
            {
                Debug.Assert(ParentTransaction != null);
                for (var i = 0; i < ParentTransaction.Outputs.Count; i++)
                {
                    if (ParentTransaction.Outputs[i] == this)
                        return i;
                }
                // Should never happen.
                throw new Exception("Output linked to wrong parent transaction?");
            }
        }

        /// <summary>
        /// Sets this objects availableToSpend flag to false and the spentBy pointer to the given input.
        /// If the input is null, it means this output was signed over to somebody else rather than one of our own keys.
        /// </summary>
        internal void MarkAsSpent(TransactionInput input)
        {
            Debug.Assert(_availableForSpending);
            _availableForSpending = false;
            _spentBy = input;
        }

        internal void MarkAsUnspent()
        {
            _availableForSpending = true;
            _spentBy = null;
        }

        internal bool IsAvailableForSpending
        {
            get { return _availableForSpending; }
        }

        public byte[] ScriptBytes
        {
            get { return _scriptBytes; }
        }

        /// <summary>
        /// Returns true if this output is to an address we have the keys for in the wallet.
        /// </summary>
        public bool IsMine(Wallet wallet)
        {
            try
            {
                var pubkeyHash = ScriptPubKey.PubKeyHash;
                return wallet.IsPubKeyHashMine(pubkeyHash);
            }
            catch (ScriptException e)
            {
                _log.ErrorFormat("Could not parse tx output script: {0}", e);
                return false;
            }
        }

        /// <summary>
        /// Returns a human readable debug string.
        /// </summary>
        public override string ToString()
        {
            return "TxOut of " + Utils.BitcoinValueToFriendlyString(_value) + " to " + ScriptPubKey.ToAddress +
                   " script:" + ScriptPubKey;
        }

        /// <summary>
        /// Returns the connected input.
        /// </summary>
        internal TransactionInput SpentBy
        {
            get { return _spentBy; }
        }
    }
}