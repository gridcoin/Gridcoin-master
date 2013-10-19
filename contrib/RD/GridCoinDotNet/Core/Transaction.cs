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
using System.Text;
using BitCoinSharp.IO;

namespace BitCoinSharp
{
    /// <summary>
    /// A transaction represents the movement of coins from some addresses to some other addresses. It can also represent
    /// the minting of new coins. A Transaction object corresponds to the equivalent in the BitCoin C++ implementation.
    /// </summary>
    /// <remarks>
    /// It implements TWO serialization protocols - the BitCoin proprietary format which is identical to the C++
    /// implementation and is used for reading/writing transactions to the wire and for hashing. It also implements Java
    /// serialization which is used for the wallet. This allows us to easily add extra fields used for our own accounting
    /// or UI purposes.
    /// </remarks>
    [Serializable]
    public class Transaction : Message
    {
        // These are serialized in both BitCoin and java serialization.
        private uint _version;
        private List<TransactionInput> _inputs;
        private List<TransactionOutput> _outputs;
        private uint _lockTime;

        // This is an in memory helper only.
        [NonSerialized] private Sha256Hash _hash;

        internal Transaction(NetworkParameters @params)
            : base(@params)
        {
            _version = 1;
            _inputs = new List<TransactionInput>();
            _outputs = new List<TransactionOutput>();
            // We don't initialize appearsIn deliberately as it's only useful for transactions stored in the wallet.
        }

        /// <summary>
        /// Creates a transaction from the given serialized bytes, eg, from a block or a tx network message.
        /// </summary>
        /// <exception cref="ProtocolException"/>
        public Transaction(NetworkParameters @params, byte[] payloadBytes)
            : base(@params, payloadBytes, 0)
        {
        }

        /// <summary>
        /// Creates a transaction by reading payload starting from offset bytes in. Length of a transaction is fixed.
        /// </summary>
        /// <exception cref="ProtocolException"/>
        public Transaction(NetworkParameters @params, byte[] payload, int offset)
            : base(@params, payload, offset)
        {
            // inputs/outputs will be created in parse()
        }

        /// <summary>
        /// Returns a read-only list of the inputs of this transaction.
        /// </summary>
        public IList<TransactionInput> Inputs
        {
            get { return _inputs.AsReadOnly(); }
        }

        /// <summary>
        /// Returns a read-only list of the outputs of this transaction.
        /// </summary>
        public IList<TransactionOutput> Outputs
        {
            get { return _outputs.AsReadOnly(); }
        }

        /// <summary>
        /// Returns the transaction hash as you see them in the block explorer.
        /// </summary>
        public Sha256Hash Hash
        {
            get { return _hash ?? (_hash = new Sha256Hash(Utils.ReverseBytes(Utils.DoubleDigest(BitcoinSerialize())))); }
        }

        public string HashAsString
        {
            get { return Hash.ToString(); }
        }

        /// <summary>
        /// Calculates the sum of the outputs that are sending coins to a key in the wallet. The flag controls whether to
        /// include spent outputs or not.
        /// </summary>
        internal ulong GetValueSentToMe(Wallet wallet, bool includeSpent)
        {
            // This is tested in WalletTest.
            var v = 0UL;
            foreach (var o in _outputs)
            {
                if (!o.IsMine(wallet)) continue;
                if (!includeSpent && !o.IsAvailableForSpending) continue;
                v += o.Value;
            }
            return v;
        }

        /// <summary>
        /// Calculates the sum of the outputs that are sending coins to a key in the wallet.
        /// </summary>
        public ulong GetValueSentToMe(Wallet wallet)
        {
            return GetValueSentToMe(wallet, true);
        }

        /// <summary>
        /// Returns a set of blocks which contain the transaction, or null if this transaction doesn't have that data
        /// because it's not stored in the wallet or because it has never appeared in a block.
        /// </summary>
        internal ICollection<StoredBlock> AppearsIn { get; private set; }

        /// <summary>
        /// Adds the given block to the internal serializable set of blocks in which this transaction appears. This is
        /// used by the wallet to ensure transactions that appear on side chains are recorded properly even though the
        /// block stores do not save the transaction data at all.
        /// </summary>
        internal void AddBlockAppearance(StoredBlock block)
        {
            if (AppearsIn == null)
            {
                AppearsIn = new HashSet<StoredBlock>();
            }
            AppearsIn.Add(block);
        }

        /// <summary>
        /// Calculates the sum of the inputs that are spending coins with keys in the wallet. This requires the
        /// transactions sending coins to those keys to be in the wallet. This method will not attempt to download the
        /// blocks containing the input transactions if the key is in the wallet but the transactions are not.
        /// </summary>
        /// <returns>Sum in nanocoins.</returns>
        /// <exception cref="ScriptException"/>
        public ulong GetValueSentFromMe(Wallet wallet)
        {
            // This is tested in WalletTest.
            var v = 0UL;
            foreach (var input in _inputs)
            {
                // This input is taking value from an transaction in our wallet. To discover the value,
                // we must find the connected transaction.
                var connected = input.GetConnectedOutput(wallet.Unspent);
                if (connected == null)
                    connected = input.GetConnectedOutput(wallet.Spent);
                if (connected == null)
                    connected = input.GetConnectedOutput(wallet.Pending);
                if (connected == null)
                    continue;
                // The connected output may be the change to the sender of a previous input sent to this wallet. In this
                // case we ignore it.
                if (!connected.IsMine(wallet))
                    continue;
                v += connected.Value;
            }
            return v;
        }

        internal bool DisconnectInputs()
        {
            var disconnected = false;
            foreach (var input in _inputs)
            {
                disconnected |= input.Disconnect();
            }
            return disconnected;
        }

        /// <summary>
        /// Connects all inputs using the provided transactions. If any input cannot be connected returns that input or
        /// null on success.
        /// </summary>
        internal TransactionInput ConnectForReorganize(IDictionary<Sha256Hash, Transaction> transactions)
        {
            foreach (var input in _inputs)
            {
                // Coinbase transactions, by definition, do not have connectable inputs.
                if (input.IsCoinBase) continue;
                var result = input.Connect(transactions, false);
                // Connected to another tx in the wallet?
                if (result == TransactionInput.ConnectionResult.Success)
                    continue;
                // The input doesn't exist in the wallet, eg because it belongs to somebody else (inbound spend).
                if (result == TransactionInput.ConnectionResult.NoSuchTx)
                    continue;
                // Could not connect this input, so return it and abort.
                return input;
            }
            return null;
        }

        /// <returns>true if every output is marked as spent.</returns>
        public bool IsEveryOutputSpent()
        {
            foreach (var output in _outputs)
            {
                if (output.IsAvailableForSpending)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// These constants are a part of a scriptSig signature on the inputs. They define the details of how a
        /// transaction can be redeemed, specifically, they control how the hash of the transaction is calculated.
        /// </summary>
        /// <remarks>
        /// In the official client, this enum also has another flag, SIGHASH_ANYONECANPAY. In this implementation,
        /// that's kept separate. Only SIGHASH_ALL is actually used in the official client today. The other flags
        /// exist to allow for distributed contracts.
        /// </remarks>
        public enum SigHash
        {
            All, // 1
            None, // 2
            Single, // 3
        }

        /// <exception cref="ProtocolException"/>
        protected override void Parse()
        {
            _version = ReadUint32();
            // First come the inputs.
            var numInputs = ReadVarInt();
            _inputs = new List<TransactionInput>((int) numInputs);
            for (var i = 0UL; i < numInputs; i++)
            {
                var input = new TransactionInput(Params, this, Bytes, Cursor);
                _inputs.Add(input);
                Cursor += input.MessageSize;
            }
            // Now the outputs
            var numOutputs = ReadVarInt();
            _outputs = new List<TransactionOutput>((int) numOutputs);
            for (var i = 0UL; i < numOutputs; i++)
            {
                var output = new TransactionOutput(Params, this, Bytes, Cursor);
                _outputs.Add(output);
                Cursor += output.MessageSize;
            }
            _lockTime = ReadUint32();
        }

        /// <summary>
        /// A coinbase transaction is one that creates a new coin. They are the first transaction in each block and their
        /// value is determined by a formula that all implementations of BitCoin share. In 2011 the value of a coinbase
        /// transaction is 50 coins, but in future it will be less. A coinbase transaction is defined not only by its
        /// position in a block but by the data in the inputs.
        /// </summary>
        public bool IsCoinBase
        {
            get { return _inputs[0].IsCoinBase; }
        }

        /// <returns>A human readable version of the transaction useful for debugging.</returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("  ");
            s.Append(HashAsString);
            s.AppendLine();
            if (IsCoinBase)
            {
                string script;
                string script2;
                try
                {
                    script = _inputs[0].ScriptSig.ToString();
                    script2 = _outputs[0].ScriptPubKey.ToString();
                }
                catch (ScriptException)
                {
                    script = "???";
                    script2 = "???";
                }
                return "     == COINBASE TXN (scriptSig " + script + ")  (scriptPubKey " + script2 + ")";
            }
            foreach (var @in in _inputs)
            {
                s.Append("     ");
                s.Append("from ");

                try
                {
                    s.Append(@in.ScriptSig.FromAddress.ToString());
                }
                catch (Exception e)
                {
                    s.Append("[exception: ").Append(e.Message).Append("]");
                    throw;
                }
                s.AppendLine();
            }
            foreach (var @out in _outputs)
            {
                s.Append("       ");
                s.Append("to ");
                try
                {
                    var toAddr = new Address(Params, @out.ScriptPubKey.PubKeyHash);
                    s.Append(toAddr.ToString());
                    s.Append(" ");
                    s.Append(Utils.BitcoinValueToFriendlyString(@out.Value));
                    s.Append(" BTC");
                }
                catch (Exception e)
                {
                    s.Append("[exception: ").Append(e.Message).Append("]");
                }
                s.AppendLine();
            }
            return s.ToString();
        }

        /// <summary>
        /// Adds an input to this transaction that imports value from the given output. Note that this input is NOT
        /// complete and after every input is added with addInput() and every output is added with addOutput(),
        /// signInputs() must be called to finalize the transaction and finish the inputs off. Otherwise it won't be
        /// accepted by the network.
        /// </summary>
        public void AddInput(TransactionOutput from)
        {
            AddInput(new TransactionInput(Params, this, from));
        }

        /// <summary>
        /// Adds an input directly, with no checking that it's valid.
        /// </summary>
        public void AddInput(TransactionInput input)
        {
            _inputs.Add(input);
        }

        /// <summary>
        /// Adds the given output to this transaction. The output must be completely initialized.
        /// </summary>
        public void AddOutput(TransactionOutput to)
        {
            to.ParentTransaction = this;
            _outputs.Add(to);
        }

        /// <summary>
        /// Once a transaction has some inputs and outputs added, the signatures in the inputs can be calculated. The
        /// signature is over the transaction itself, to prove the redeemer actually created that transaction,
        /// so we have to do this step last.
        /// </summary>
        /// <remarks>
        /// This method is similar to SignatureHash in script.cpp
        /// </remarks>
        /// <param name="hashType">This should always be set to SigHash.ALL currently. Other types are unused. </param>
        /// <param name="wallet">A wallet is required to fetch the keys needed for signing.</param>
        /// <exception cref="ScriptException"/>
        public void SignInputs(SigHash hashType, Wallet wallet)
        {
            Debug.Assert(_inputs.Count > 0);
            Debug.Assert(_outputs.Count > 0);

            // I don't currently have an easy way to test other modes work, as the official client does not use them.
            Debug.Assert(hashType == SigHash.All);

            // The transaction is signed with the input scripts empty except for the input we are signing. In the case
            // where addInput has been used to set up a new transaction, they are already all empty. The input being signed
            // has to have the connected OUTPUT program in it when the hash is calculated!
            //
            // Note that each input may be claiming an output sent to a different key. So we have to look at the outputs
            // to figure out which key to sign with.

            var signatures = new byte[_inputs.Count][];
            var signingKeys = new EcKey[_inputs.Count];
            for (var i = 0; i < _inputs.Count; i++)
            {
                var input = _inputs[i];
                Debug.Assert(input.ScriptBytes.Length == 0, "Attempting to sign a non-fresh transaction");
                // Set the input to the script of its output.
                input.ScriptBytes = input.Outpoint.ConnectedPubKeyScript;
                // Find the signing key we'll need to use.
                var connectedPubKeyHash = input.Outpoint.ConnectedPubKeyHash;
                var key = wallet.FindKeyFromPubHash(connectedPubKeyHash);
                // This assert should never fire. If it does, it means the wallet is inconsistent.
                Debug.Assert(key != null, "Transaction exists in wallet that we cannot redeem: " + Utils.BytesToHexString(connectedPubKeyHash));
                // Keep the key around for the script creation step below.
                signingKeys[i] = key;
                // The anyoneCanPay feature isn't used at the moment.
                const bool anyoneCanPay = false;
                var hash = HashTransactionForSignature(hashType, anyoneCanPay);
                // Set the script to empty again for the next input.
                input.ScriptBytes = TransactionInput.EmptyArray;

                // Now sign for the output so we can redeem it. We use the keypair to sign the hash,
                // and then put the resulting signature in the script along with the public key (below).
                using (var bos = new MemoryStream())
                {
                    bos.Write(key.Sign(hash));
                    bos.Write((byte) (((int) hashType + 1) | (anyoneCanPay ? 0x80 : 0)));
                    signatures[i] = bos.ToArray();
                }
            }

            // Now we have calculated each signature, go through and create the scripts. Reminder: the script consists of
            // a signature (over a hash of the transaction) and the complete public key needed to sign for the connected
            // output.
            for (var i = 0; i < _inputs.Count; i++)
            {
                var input = _inputs[i];
                Debug.Assert(input.ScriptBytes.Length == 0);
                var key = signingKeys[i];
                input.ScriptBytes = Script.CreateInputScript(signatures[i], key.PubKey);
            }

            // Every input is now complete.
        }

        private byte[] HashTransactionForSignature(SigHash type, bool anyoneCanPay)
        {
            using (var bos = new MemoryStream())
            {
                BitcoinSerializeToStream(bos);
                // We also have to write a hash type.
                var hashType = (uint) type + 1;
                if (anyoneCanPay)
                    hashType |= 0x80;
                Utils.Uint32ToByteStreamLe(hashType, bos);
                // Note that this is NOT reversed to ensure it will be signed correctly. If it were to be printed out
                // however then we would expect that it is IS reversed.
                return Utils.DoubleDigest(bos.ToArray());
            }
        }

        /// <exception cref="IOException"/>
        public override void BitcoinSerializeToStream(Stream stream)
        {
            Utils.Uint32ToByteStreamLe(_version, stream);
            stream.Write(new VarInt((ulong) _inputs.Count).Encode());
            foreach (var @in in _inputs)
                @in.BitcoinSerializeToStream(stream);
            stream.Write(new VarInt((ulong) _outputs.Count).Encode());
            foreach (var @out in _outputs)
                @out.BitcoinSerializeToStream(stream);
            Utils.Uint32ToByteStreamLe(_lockTime, stream);
        }

        public override bool Equals(object other)
        {
            if (!(other is Transaction)) return false;
            var t = (Transaction) other;

            return t.Hash.Equals(Hash);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }
    }
}