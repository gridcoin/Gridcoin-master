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
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using log4net;

namespace BitCoinSharp
{
    /// <summary>
    /// A Wallet stores keys and a record of transactions that have not yet been spent. Thus, it is capable of
    /// providing transactions on demand that meet a given combined value.
    /// </summary>
    /// <remarks>
    /// The Wallet is read and written from disk, so be sure to follow the Java serialization versioning rules here. We
    /// use the built in Java serialization to avoid the need to pull in a potentially large (code-size) third party
    /// serialization library.<p/>
    /// </remarks>
    [Serializable]
    public class Wallet
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Wallet));

        // Algorithm for movement of transactions between pools. Outbound tx = us spending coins. Inbound tx = us
        // receiving coins. If a tx is both inbound and outbound (spend with change) it is considered outbound for the
        // purposes of the explanation below.
        //
        // 1. Outbound tx is created by us: ->pending
        // 2. Outbound tx that was broadcast is accepted into the main chain:
        //     <-pending  and
        //       If there is a change output  ->unspent
        //       If there is no change output ->spent
        // 3. Outbound tx that was broadcast is accepted into a side chain:
        //     ->inactive  (remains in pending).
        // 4. Inbound tx is accepted into the best chain:
        //     ->unspent/spent
        // 5. Inbound tx is accepted into a side chain:
        //     ->inactive
        //     Whilst it's also 'pending' in some sense, in that miners will probably try and incorporate it into the
        //     best chain, we don't mark it as such here. It'll eventually show up after a re-org.
        // 6. Outbound tx that is pending shares inputs with a tx that appears in the main chain:
        //     <-pending ->dead
        //
        // Re-orgs:
        // 1. Tx is present in old chain and not present in new chain
        //       <-unspent/spent  ->pending
        //       These newly inactive transactions will (if they are relevant to us) eventually come back via receive()
        //       as miners resurrect them and re-include into the new best chain.
        // 2. Tx is not present in old chain and is present in new chain
        //       <-inactive  and  ->unspent/spent
        // 3. Tx is present in new chain and shares inputs with a pending transaction, including those that were resurrected
        //    due to point (1)
        //       <-pending ->dead
        //
        // Balance:
        // 1. Sum up all unspent outputs of the transactions in unspent.
        // 2. Subtract the inputs of transactions in pending.
        // 3. If requested, re-add the outputs of pending transactions that are mine. This is the estimated balance.

        /// <summary>
        /// Map of txhash-&gt;Transactions that have not made it into the best chain yet. They are eligible to move there but
        /// are waiting for a miner to send a block on the best chain including them. These transactions inputs count as
        /// spent for the purposes of calculating our balance but their outputs are not available for spending yet. This
        /// means after a spend, our balance can actually go down temporarily before going up again!
        /// </summary>
        internal IDictionary<Sha256Hash, Transaction> Pending { get; private set; }

        /// <summary>
        /// Map of txhash-&gt;Transactions where the Transaction has unspent outputs. These are transactions we can use
        /// to pay other people and so count towards our balance. Transactions only appear in this map if they are part
        /// of the best chain. Transactions we have broadcast that are not confirmed yet appear in pending even though they
        /// may have unspent "change" outputs.
        /// </summary>
        /// <remarks>
        /// Note: for now we will not allow spends of transactions that did not make it into the block chain. The code
        /// that handles this in BitCoin C++ is complicated. Satoshi's code will not allow you to spend unconfirmed coins,
        /// however, it does seem to support dependency resolution entirely within the context of the memory pool so
        /// theoretically you could spend zero-conf coins and all of them would be included together. To simplify we'll
        /// make people wait but it would be a good improvement to resolve this in future.
        /// </remarks>
        internal IDictionary<Sha256Hash, Transaction> Unspent { get; private set; }

        /// <summary>
        /// Map of txhash-&gt;Transactions where the Transactions outputs are all fully spent. They are kept separately so
        /// the time to create a spend does not grow infinitely as wallets become more used. Some of these transactions
        /// may not have appeared in a block yet if they were created by us to spend coins and that spend is still being
        /// worked on by miners.
        /// </summary>
        /// <remarks>
        /// Transactions only appear in this map if they are part of the best chain.
        /// </remarks>
        internal IDictionary<Sha256Hash, Transaction> Spent { get; private set; }

        /// <summary>
        /// An inactive transaction is one that is seen only in a block that is not a part of the best chain. We keep it
        /// around in case a re-org promotes a different chain to be the best. In this case some (not necessarily all)
        /// inactive transactions will be moved out to unspent and spent, and some might be moved in.
        /// </summary>
        /// <remarks>
        /// Note that in the case where a transaction appears in both the best chain and a side chain as well, it is not
        /// placed in this map. It's an error for a transaction to be in both the inactive pool and unspent/spent.
        /// </remarks>
        private readonly IDictionary<Sha256Hash, Transaction> _inactive;

        /// <summary>
        /// A dead transaction is one that's been overridden by a double spend. Such a transaction is pending except it
        /// will never confirm and so should be presented to the user in some unique way - flashing red for example. This
        /// should nearly never happen in normal usage. Dead transactions can be "resurrected" by re-orgs just like any
        /// other. Dead transactions are not in the pending pool.
        /// </summary>
        private readonly IDictionary<Sha256Hash, Transaction> _dead;

        /// <summary>
        /// A list of public/private EC keys owned by this user.
        /// </summary>
        public IList<EcKey> Keychain { get; private set; }

        private readonly NetworkParameters _params;

        /// <summary>
        /// Creates a new, empty wallet with no keys and no transactions. If you want to restore a wallet from disk instead,
        /// see loadFromFile.
        /// </summary>
        public Wallet(NetworkParameters @params)
        {
            _params = @params;
            Keychain = new List<EcKey>();
            Unspent = new Dictionary<Sha256Hash, Transaction>();
            Spent = new Dictionary<Sha256Hash, Transaction>();
            _inactive = new Dictionary<Sha256Hash, Transaction>();
            Pending = new Dictionary<Sha256Hash, Transaction>();
            _dead = new Dictionary<Sha256Hash, Transaction>();
        }

        /// <summary>
        /// Uses Java serialization to save the wallet to the given file.
        /// </summary>
        /// <exception cref="IOException"/>
        public void SaveToFile(FileInfo f)
        {
            lock (this)
            {
                using (var stream = f.OpenWrite())
                {
                    SaveToFileStream(stream);
                }
            }
        }

        /// <summary>
        /// Uses Java serialization to save the wallet to the given file stream.
        /// </summary>
        /// <exception cref="IOException"/>
        public void SaveToFileStream(FileStream f)
        {
            lock (this)
            {
                var oos = new BinaryFormatter();
                oos.Serialize(f, this);
            }
        }

        /// <summary>
        /// Returns a wallet deserialized from the given file.
        /// </summary>
        /// <exception cref="IOException"/>
        public static Wallet LoadFromFile(FileInfo f)
        {
            return LoadFromFileStream(f.OpenRead());
        }

        /// <summary>
        /// Returns a wallet deserialized from the given file input stream.
        /// </summary>
        /// <exception cref="IOException"/>
        public static Wallet LoadFromFileStream(FileStream f)
        {
            var ois = new BinaryFormatter();
            return (Wallet) ois.Deserialize(f);
        }

        /// <summary>
        /// Called by the <see cref="BlockChain"/> when we receive a new block that sends coins to one of our addresses or
        /// spends coins from one of our addresses (note that a single transaction can do both).
        /// </summary>
        /// <remarks>
        /// This is necessary for the internal book-keeping Wallet does. When a transaction is received that sends us
        /// coins it is added to a pool so we can use it later to create spends. When a transaction is received that
        /// consumes outputs they are marked as spent so they won't be used in future.<p/>
        /// A transaction that spends our own coins can be received either because a spend we created was accepted by the
        /// network and thus made it into a block, or because our keys are being shared between multiple instances and
        /// some other node spent the coins instead. We still have to know about that to avoid accidentally trying to
        /// double spend.<p/>
        /// A transaction may be received multiple times if is included into blocks in parallel chains. The blockType
        /// parameter describes whether the containing block is on the main/best chain or whether it's on a presently
        /// inactive side chain. We must still record these transactions and the blocks they appear in because a future
        /// block might change which chain is best causing a reorganize. A re-org can totally change our balance!
        /// </remarks>
        /// <exception cref="VerificationException"/>
        /// <exception cref="ScriptException"/>
        internal void Receive(Transaction tx, StoredBlock block, BlockChain.NewBlockType blockType)
        {
            lock (this)
            {
                Receive(tx, block, blockType, false);
            }
        }

        /// <exception cref="VerificationException"/>
        /// <exception cref="ScriptException"/>
        private void Receive(Transaction tx, StoredBlock block, BlockChain.NewBlockType blockType, bool reorg)
        {
            lock (this)
            {
                // Runs in a peer thread.
                var prevBalance = GetBalance();

                var txHash = tx.Hash;

                var bestChain = blockType == BlockChain.NewBlockType.BestChain;
                var sideChain = blockType == BlockChain.NewBlockType.SideChain;

                var valueSentFromMe = tx.GetValueSentFromMe(this);
                var valueSentToMe = tx.GetValueSentToMe(this);
                var valueDifference = (long) (valueSentToMe - valueSentFromMe);

                if (!reorg)
                {
                    _log.InfoFormat("Received tx{0} for {1} BTC: {2}", sideChain ? " on a side chain" : "",
                                    Utils.BitcoinValueToFriendlyString(valueDifference), tx.HashAsString);
                }

                // If this transaction is already in the wallet we may need to move it into a different pool. At the very
                // least we need to ensure we're manipulating the canonical object rather than a duplicate.
                Transaction wtx;
                if (Pending.TryGetValue(txHash, out wtx))
                {
                    Pending.Remove(txHash);
                    _log.Info("  <-pending");
                    // A transaction we created appeared in a block. Probably this is a spend we broadcast that has been
                    // accepted by the network.
                    //
                    // Mark the tx as appearing in this block so we can find it later after a re-org.
                    wtx.AddBlockAppearance(block);
                    if (bestChain)
                    {
                        if (valueSentToMe.Equals(0))
                        {
                            // There were no change transactions so this tx is fully spent.
                            _log.Info("  ->spent");
                            Debug.Assert(!Spent.ContainsKey(wtx.Hash), "TX in both pending and spent pools");
                            Spent[wtx.Hash] = wtx;
                        }
                        else
                        {
                            // There was change back to us, or this tx was purely a spend back to ourselves (perhaps for
                            // anonymization purposes).
                            _log.Info("  ->unspent");
                            Debug.Assert(!Unspent.ContainsKey(wtx.Hash), "TX in both pending and unspent pools");
                            Unspent[wtx.Hash] = wtx;
                        }
                    }
                    else if (sideChain)
                    {
                        // The transaction was accepted on an inactive side chain, but not yet by the best chain.
                        _log.Info("  ->inactive");
                        // It's OK for this to already be in the inactive pool because there can be multiple independent side
                        // chains in which it appears:
                        //
                        //     b1 --> b2
                        //        \-> b3
                        //        \-> b4 (at this point it's already present in 'inactive'
                        if (_inactive.ContainsKey(wtx.Hash))
                            _log.Info("Saw a transaction be incorporated into multiple independent side chains");
                        _inactive[wtx.Hash] = wtx;
                        // Put it back into the pending pool, because 'pending' means 'waiting to be included in best chain'.
                        Pending[wtx.Hash] = wtx;
                    }
                }
                else
                {
                    if (!reorg)
                    {
                        // Mark the tx as appearing in this block so we can find it later after a re-org.
                        tx.AddBlockAppearance(block);
                    }
                    // This TX didn't originate with us. It could be sending us coins and also spending our own coins if keys
                    // are being shared between different wallets.
                    if (sideChain)
                    {
                        _log.Info("  ->inactive");
                        _inactive[tx.Hash] = tx;
                    }
                    else if (bestChain)
                    {
                        ProcessTxFromBestChain(tx);
                    }
                }

                _log.InfoFormat("Balance is now: {0}", Utils.BitcoinValueToFriendlyString(GetBalance()));

                // Inform anyone interested that we have new coins. Note: we may be re-entered by the event listener,
                // so we must not make assumptions about our state after this loop returns! For example,
                // the balance we just received might already be spent!
                if (!reorg && bestChain && valueDifference > 0 && CoinsReceived != null)
                {
                    lock (CoinsReceived)
                    {
                        CoinsReceived(this, new WalletCoinsReceivedEventArgs(tx, prevBalance, GetBalance()));
                    }
                }
            }
        }

        /// <summary>
        /// Handle when a transaction becomes newly active on the best chain, either due to receiving a new block or a
        /// re-org making inactive transactions active.
        /// </summary>
        /// <exception cref="VerificationException"/>
        private void ProcessTxFromBestChain(Transaction tx)
        {
            // This TX may spend our existing outputs even though it was not pending. This can happen in unit
            // tests and if keys are moved between wallets.
            UpdateForSpends(tx);
            if (!tx.GetValueSentToMe(this).Equals(0))
            {
                // It's sending us coins.
                _log.Info("  new tx ->unspent");
                Debug.Assert(!Unspent.ContainsKey(tx.Hash), "TX was received twice");
                Unspent[tx.Hash] = tx;
            }
            else
            {
                // It spent some of our coins and did not send us any.
                _log.Info("  new tx ->spent");
                Debug.Assert(!Spent.ContainsKey(tx.Hash), "TX was received twice");
                Spent[tx.Hash] = tx;
            }
        }

        /// <summary>
        /// Updates the wallet by checking if this TX spends any of our outputs. This is not used normally because
        /// when we receive our own spends, we've already marked the outputs as spent previously (during tx creation) so
        /// there's no need to go through and do it again.
        /// </summary>
        /// <exception cref="VerificationException"/>
        private void UpdateForSpends(Transaction tx)
        {
            // tx is on the best chain by this point.
            foreach (var input in tx.Inputs)
            {
                var result = input.Connect(Unspent, false);
                if (result == TransactionInput.ConnectionResult.NoSuchTx)
                {
                    // Not found in the unspent map. Try again with the spent map.
                    result = input.Connect(Spent, false);
                    if (result == TransactionInput.ConnectionResult.NoSuchTx)
                    {
                        // Doesn't spend any of our outputs or is coinbase.
                        continue;
                    }
                }
                if (result == TransactionInput.ConnectionResult.AlreadySpent)
                {
                    // Double spend! This must have overridden a pending tx, or the block is bad (contains transactions
                    // that illegally double spend: should never occur if we are connected to an honest node).
                    //
                    // Work backwards like so:
                    //
                    //   A  -> spent by B [pending]
                    //     \-> spent by C [chain]
                    var doubleSpent = input.Outpoint.FromTx; // == A
                    Debug.Assert(doubleSpent != null);
                    var index = input.Outpoint.Index;
                    var output = doubleSpent.Outputs[index];
                    var spentBy = output.SpentBy;
                    Debug.Assert(spentBy != null);
                    var connected = spentBy.ParentTransaction;
                    Debug.Assert(connected != null);
                    if (Pending.Remove(connected.Hash))
                    {
                        _log.InfoFormat("Saw double spend from chain override pending tx {0}", connected.HashAsString);
                        _log.Info("  <-pending ->dead");
                        _dead[connected.Hash] = connected;
                        // Now forcibly change the connection.
                        input.Connect(Unspent, true);
                        // Inform the event listeners of the newly dead tx.
                        if (DeadTransaction != null)
                        {
                            lock (DeadTransaction)
                            {
                                DeadTransaction(this, new WalletDeadTransactionEventArgs(connected, tx));
                            }
                        }
                    }
                }
                else if (result == TransactionInput.ConnectionResult.Success)
                {
                    // Otherwise we saw a transaction spend our coins, but we didn't try and spend them ourselves yet.
                    // The outputs are already marked as spent by the connect call above, so check if there are any more for
                    // us to use. Move if not.
                    var connected = input.Outpoint.FromTx;
                    MaybeMoveTxToSpent(connected, "prevtx");
                }
            }
        }

        /// <summary>
        /// If the transactions outputs are all marked as spent, and it's in the unspent map, move it.
        /// </summary>
        private void MaybeMoveTxToSpent(Transaction tx, String context)
        {
            if (tx.IsEveryOutputSpent())
            {
                // There's nothing left I can spend in this transaction.
                if (Unspent.Remove(tx.Hash))
                {
                    if (_log.IsInfoEnabled)
                    {
                        _log.Info("  " + context + " <-unspent");
                        _log.Info("  " + context + " ->spent");
                    }
                    Spent[tx.Hash] = tx;
                }
            }
        }

        /// <summary>
        /// This is called on a Peer thread when a block is received that sends some coins to you. Note that this will
        /// also be called when downloading the block chain as the wallet balance catches up so if you don't want that
        /// register the event listener after the chain is downloaded. It's safe to use methods of wallet during the
        /// execution of this callback.
        /// </summary>
        public event EventHandler<WalletCoinsReceivedEventArgs> CoinsReceived;

        /// <summary>
        /// This is called on a Peer thread when a block is received that triggers a block chain re-organization.
        /// </summary>
        /// <remarks>
        /// A re-organize means that the consensus (chain) of the network has diverged and now changed from what we
        /// believed it was previously. Usually this won't matter because the new consensus will include all our old
        /// transactions assuming we are playing by the rules. However it's theoretically possible for our balance to
        /// change in arbitrary ways, most likely, we could lose some money we thought we had.<p/>
        /// It is safe to use methods of wallet whilst inside this callback.<p/>
        /// TODO: Finish this interface.
        /// </remarks>
        public event EventHandler<EventArgs> Reorganized;

        /// <summary>
        /// This is called on a Peer thread when a transaction becomes <i>dead</i>. A dead transaction is one that has
        /// been overridden by a double spend from the network and so will never confirm no matter how long you wait.
        /// </summary>
        /// <remarks>
        /// A dead transaction can occur if somebody is attacking the network, or by accident if keys are being shared.
        /// You can use this event handler to inform the user of the situation. A dead spend will show up in the BitCoin
        /// C++ client of the recipient as 0/unconfirmed forever, so if it was used to purchase something,
        /// the user needs to know their goods will never arrive.
        /// </remarks>
        public event EventHandler<WalletDeadTransactionEventArgs> DeadTransaction;

        /// <summary>
        /// Call this when we have successfully transmitted the send tx to the network, to update the wallet.
        /// </summary>
        internal void ConfirmSend(Transaction tx)
        {
            lock (this)
            {
                Debug.Assert(!Pending.ContainsKey(tx.Hash), "confirmSend called on the same transaction twice");
                _log.InfoFormat("confirmSend of {0}", tx.HashAsString);
                // Mark the outputs of the used transactions as spent, so we don't try and spend it again.
                foreach (var input in tx.Inputs)
                {
                    var connectedOutput = input.Outpoint.ConnectedOutput;
                    var connectedTx = connectedOutput.ParentTransaction;
                    connectedOutput.MarkAsSpent(input);
                    MaybeMoveTxToSpent(connectedTx, "spent tx");
                }
                // Add to the pending pool. It'll be moved out once we receive this transaction on the best chain.
                Pending[tx.Hash] = tx;
            }
        }

        // This is used only for unit testing, it's an internal API.
        internal enum Pool
        {
            Unspent,
            Spent,
            Pending,
            Inactive,
            Dead,
            All
        }

        internal int GetPoolSize(Pool pool)
        {
            switch (pool)
            {
                case Pool.Unspent:
                    return Unspent.Count;
                case Pool.Spent:
                    return Spent.Count;
                case Pool.Pending:
                    return Pending.Count;
                case Pool.Inactive:
                    return _inactive.Count;
                case Pool.Dead:
                    return _dead.Count;
                case Pool.All:
                    return Unspent.Count + Spent.Count + Pending.Count + _inactive.Count + _dead.Count;
                default:
                    throw new ArgumentOutOfRangeException("pool");
            }
        }

        /// <summary>
        /// Statelessly creates a transaction that sends the given number of nanocoins to address. The change is sent to
        /// the first address in the wallet, so you must have added at least one key.
        /// </summary>
        /// <remarks>
        /// This method is stateless in the sense that calling it twice with the same inputs will result in two
        /// Transaction objects which are equal. The wallet is not updated to track its pending status or to mark the
        /// coins as spent until confirmSend is called on the result.
        /// </remarks>
        internal Transaction CreateSend(Address address, ulong nanocoins)
        {
            lock (this)
            {
                // For now let's just pick the first key in our keychain. In future we might want to do something else to
                // give the user better privacy here, eg in incognito mode.
                Debug.Assert(Keychain.Count > 0, "Can't send value without an address to use for receiving change");
                var first = Keychain[0];
                return CreateSend(address, nanocoins, first.ToAddress(_params));
            }
        }

        /// <summary>
        /// Sends coins to the given address, via the given <see cref="PeerGroup"/>.
        /// Change is returned to the first key in the wallet.
        /// </summary>
        /// <param name="peerGroup">The peer group to send via.</param>
        /// <param name="to">Which address to send coins to.</param>
        /// <param name="nanocoins">How many nanocoins to send. You can use Utils.toNanoCoins() to calculate this.</param>
        /// <returns>
        /// The <see cref="Transaction"/> that was created or null if there was insufficient balance to send the coins.
        /// </returns>
        /// <exception cref="IOException">If there was a problem broadcasting the transaction.</exception>
        public Transaction SendCoins(PeerGroup peerGroup, Address to, ulong nanocoins)
        {
            lock (this)
            {
                var tx = CreateSend(to, nanocoins);
                if (tx == null) // Not enough money! :-(
                    return null;
                if (!peerGroup.BroadcastTransaction(tx))
                {
                    throw new IOException("Failed to broadcast tx to all connected peers");
                }

                // TODO - retry logic
                ConfirmSend(tx);
                return tx;
            }
        }

        /// <summary>
        /// Sends coins to the given address, via the given <see cref="Peer"/>.
        /// Change is returned to the first key in the wallet.
        /// </summary>
        /// <param name="peer">The peer to send via.</param>
        /// <param name="to">Which address to send coins to.</param>
        /// <param name="nanocoins">How many nanocoins to send. You can use Utils.ToNanoCoins() to calculate this.</param>
        /// <returns>The <see cref="Transaction"/> that was created or null if there was insufficient balance to send the coins.</returns>
        /// <exception cref="IOException">If there was a problem broadcasting the transaction.</exception>
        public Transaction SendCoins(Peer peer, Address to, ulong nanocoins)
        {
            lock (this)
            {
                var tx = CreateSend(to, nanocoins);
                if (tx == null) // Not enough money! :-(
                    return null;
                peer.BroadcastTransaction(tx);
                ConfirmSend(tx);
                return tx;
            }
        }

        /// <summary>
        /// Creates a transaction that sends $coins.$cents BTC to the given address.
        /// </summary>
        /// <remarks>
        /// IMPORTANT: This method does NOT update the wallet. If you call createSend again you may get two transactions
        /// that spend the same coins. You have to call confirmSend on the created transaction to prevent this,
        /// but that should only occur once the transaction has been accepted by the network. This implies you cannot have
        /// more than one outstanding sending tx at once.
        /// </remarks>
        /// <param name="address">The BitCoin address to send the money to.</param>
        /// <param name="nanocoins">How much currency to send, in nanocoins.</param>
        /// <param name="changeAddress">
        /// Which address to send the change to, in case we can't make exactly the right value from
        /// our coins. This should be an address we own (is in the keychain).
        /// </param>
        /// <returns>
        /// A new <see cref="Transaction"/> or null if we cannot afford this send.
        /// </returns>
        internal Transaction CreateSend(Address address, ulong nanocoins, Address changeAddress)
        {
            lock (this)
            {
                _log.Info("Creating send tx to " + address + " for " +
                          Utils.BitcoinValueToFriendlyString(nanocoins));
                // To send money to somebody else, we need to do gather up transactions with unspent outputs until we have
                // sufficient value. Many coin selection algorithms are possible, we use a simple but suboptimal one.
                // TODO: Sort coins so we use the smallest first, to combat wallet fragmentation and reduce fees.
                var valueGathered = 0UL;
                var gathered = new LinkedList<TransactionOutput>();
                foreach (var tx in Unspent.Values)
                {
                    foreach (var output in tx.Outputs)
                    {
                        if (!output.IsAvailableForSpending) continue;
                        if (!output.IsMine(this)) continue;
                        gathered.AddLast(output);
                        valueGathered += output.Value;
                    }
                    if (valueGathered >= nanocoins) break;
                }
                // Can we afford this?
                if (valueGathered < nanocoins)
                {
                    _log.Info("Insufficient value in wallet for send, missing " +
                              Utils.BitcoinValueToFriendlyString(nanocoins - valueGathered));
                    // TODO: Should throw an exception here.
                    return null;
                }
                Debug.Assert(gathered.Count > 0);
                var sendTx = new Transaction(_params);
                sendTx.AddOutput(new TransactionOutput(_params, sendTx, nanocoins, address));
                var change = (long) (valueGathered - nanocoins);
                if (change > 0)
                {
                    // The value of the inputs is greater than what we want to send. Just like in real life then,
                    // we need to take back some coins ... this is called "change". Add another output that sends the change
                    // back to us.
                    _log.Info("  with " + Utils.BitcoinValueToFriendlyString((ulong) change) + " coins change");
                    sendTx.AddOutput(new TransactionOutput(_params, sendTx, (ulong) change, changeAddress));
                }
                foreach (var output in gathered)
                {
                    sendTx.AddInput(output);
                }

                // Now sign the inputs, thus proving that we are entitled to redeem the connected outputs.
                sendTx.SignInputs(Transaction.SigHash.All, this);
                _log.InfoFormat("  created {0}", sendTx.HashAsString);
                return sendTx;
            }
        }

        /// <summary>
        /// Adds the given ECKey to the wallet. There is currently no way to delete keys (that would result in coin loss).
        /// </summary>
        public void AddKey(EcKey key)
        {
            lock (this)
            {
                Debug.Assert(!Keychain.Contains(key));
                Keychain.Add(key);
            }
        }

        /// <summary>
        /// Locates a keypair from the keychain given the hash of the public key. This is needed when finding out which
        /// key we need to use to redeem a transaction output.
        /// </summary>
        /// <returns>ECKey object or null if no such key was found.</returns>
        public EcKey FindKeyFromPubHash(byte[] pubkeyHash)
        {
            lock (this)
            {
                foreach (var key in Keychain)
                {
                    if (key.PubKeyHash.SequenceEqual(pubkeyHash)) return key;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns true if this wallet contains a public key which hashes to the given hash.
        /// </summary>
        public bool IsPubKeyHashMine(byte[] pubkeyHash)
        {
            lock (this)
            {
                return FindKeyFromPubHash(pubkeyHash) != null;
            }
        }

        /// <summary>
        /// Locates a keypair from the keychain given the raw public key bytes.
        /// </summary>
        /// <returns>ECKey or null if no such key was found.</returns>
        public EcKey FindKeyFromPubKey(byte[] pubkey)
        {
            lock (this)
            {
                foreach (var key in Keychain)
                {
                    if (key.PubKey.SequenceEqual(pubkey)) return key;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns true if this wallet contains a keypair with the given public key.
        /// </summary>
        public bool IsPubKeyMine(byte[] pubkey)
        {
            lock (this)
            {
                return FindKeyFromPubKey(pubkey) != null;
            }
        }

        /// <summary>
        /// It's possible to calculate a wallets balance from multiple points of view. This enum selects which
        /// getBalance() should use.
        /// </summary>
        /// <remarks>
        /// Consider a real-world example: you buy a snack costing $5 but you only have a $10 bill. At the start you have
        /// $10 viewed from every possible angle. After you order the snack you hand over your $10 bill. From the
        /// perspective of your wallet you have zero dollars (AVAILABLE). But you know in a few seconds the shopkeeper
        /// will give you back $5 change so most people in practice would say they have $5 (ESTIMATED).
        /// </remarks>
        public enum BalanceType
        {
            /// <summary>
            /// Balance calculated assuming all pending transactions are in fact included into the best chain by miners.
            /// This is the right balance to show in user interfaces.
            /// </summary>
            Estimated,

            /// <summary>
            /// Balance that can be safely used to create new spends. This is all confirmed unspent outputs minus the ones
            /// spent by pending transactions, but not including the outputs of those pending transactions.
            /// </summary>
            Available
        }

        /// <summary>
        /// Returns the available balance of this wallet. See <see cref="BalanceType.Available"/> for details on what this
        /// means.
        /// </summary>
        /// <remarks>
        /// Note: the estimated balance is usually the one you want to show to the end user - however attempting to
        /// actually spend these coins may result in temporary failure. This method returns how much you can safely
        /// provide to <see cref="CreateSend(Address, ulong)"/>.
        /// </remarks>
        public ulong GetBalance()
        {
            lock (this)
            {
                return GetBalance(BalanceType.Available);
            }
        }

        /// <summary>
        /// Returns the balance of this wallet as calculated by the provided balanceType.
        /// </summary>
        public ulong GetBalance(BalanceType balanceType)
        {
            lock (this)
            {
                var available = 0UL;
                foreach (var tx in Unspent.Values)
                {
                    foreach (var output in tx.Outputs)
                    {
                        if (!output.IsMine(this)) continue;
                        if (!output.IsAvailableForSpending) continue;
                        available += output.Value;
                    }
                }
                if (balanceType == BalanceType.Available)
                    return available;
                Debug.Assert(balanceType == BalanceType.Estimated);
                // Now add back all the pending outputs to assume the transaction goes through.
                var estimated = available;
                foreach (var tx in Pending.Values)
                {
                    foreach (var output in tx.Outputs)
                    {
                        if (!output.IsMine(this)) continue;
                        estimated += output.Value;
                    }
                }
                return estimated;
            }
        }

        public override string ToString()
        {
            lock (this)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("Wallet containing {0} BTC in:", Utils.BitcoinValueToFriendlyString(GetBalance())).AppendLine();
                builder.AppendFormat("  {0} unspent transactions", Unspent.Count).AppendLine();
                builder.AppendFormat("  {0} spent transactions", Spent.Count).AppendLine();
                builder.AppendFormat("  {0} pending transactions", Pending.Count).AppendLine();
                builder.AppendFormat("  {0} inactive transactions", _inactive.Count).AppendLine();
                builder.AppendFormat("  {0} dead transactions", _dead.Count).AppendLine();
                // Do the keys.
                builder.AppendLine().AppendLine("Keys:");
                foreach (var key in Keychain)
                {
                    builder.Append("  addr:");
                    builder.Append(key.ToAddress(_params));
                    builder.Append(" ");
                    builder.Append(key.ToString());
                    builder.AppendLine();
                }
                // Print the transactions themselves
                if (Unspent.Count > 0)
                {
                    builder.AppendLine().AppendLine("UNSPENT:");
                    foreach (var tx in Unspent.Values) builder.Append(tx);
                }
                if (Spent.Count > 0)
                {
                    builder.AppendLine().AppendLine("SPENT:");
                    foreach (var tx in Spent.Values) builder.Append(tx);
                }
                if (Pending.Count > 0)
                {
                    builder.AppendLine().AppendLine("PENDING:");
                    foreach (var tx in Pending.Values) builder.Append(tx);
                }
                if (_inactive.Count > 0)
                {
                    builder.AppendLine().AppendLine("INACTIVE:");
                    foreach (var tx in _inactive.Values) builder.Append(tx);
                }
                if (_dead.Count > 0)
                {
                    builder.AppendLine().AppendLine("DEAD:");
                    foreach (var tx in _dead.Values) builder.Append(tx);
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Called by the <see cref="BlockChain"/> when the best chain (representing total work done) has changed. In this case,
        /// we need to go through our transactions and find out if any have become invalid. It's possible for our balance
        /// to go down in this case: money we thought we had can suddenly vanish if the rest of the network agrees it
        /// should be so.
        /// </summary>
        /// <remarks>
        /// The oldBlocks/newBlocks lists are ordered height-wise from top first to bottom last.
        /// </remarks>
        /// <exception cref="VerificationException"/>
        internal void Reorganize(IList<StoredBlock> oldBlocks, IList<StoredBlock> newBlocks)
        {
            lock (this)
            {
                // This runs on any peer thread with the block chain synchronized.
                //
                // The reorganize functionality of the wallet is tested in ChainSplitTests.
                //
                // For each transaction we track which blocks they appeared in. Once a re-org takes place we have to find all
                // transactions in the old branch, all transactions in the new branch and find the difference of those sets.
                //
                // receive() has been called on the block that is triggering the re-org before this is called.

                _log.Info("  Old part of chain (top to bottom):");
                foreach (var b in oldBlocks) _log.InfoFormat("    {0}", b.Header.HashAsString);
                _log.InfoFormat("  New part of chain (top to bottom):");
                foreach (var b in newBlocks) _log.InfoFormat("    {0}", b.Header.HashAsString);

                // Transactions that appear in the old chain segment.
                IDictionary<Sha256Hash, Transaction> oldChainTransactions = new Dictionary<Sha256Hash, Transaction>();
                // Transactions that appear in the old chain segment and NOT the new chain segment.
                IDictionary<Sha256Hash, Transaction> onlyOldChainTransactions = new Dictionary<Sha256Hash, Transaction>();
                // Transactions that appear in the new chain segment.
                IDictionary<Sha256Hash, Transaction> newChainTransactions = new Dictionary<Sha256Hash, Transaction>();
                // Transactions that don't appear in either the new or the old section, ie, the shared trunk.
                IDictionary<Sha256Hash, Transaction> commonChainTransactions = new Dictionary<Sha256Hash, Transaction>();

                IDictionary<Sha256Hash, Transaction> all = new Dictionary<Sha256Hash, Transaction>();
                foreach (var pair in Unspent.Concat(Spent).Concat(_inactive))
                {
                    all[pair.Key] = pair.Value;
                }
                foreach (var tx in all.Values)
                {
                    var appearsIn = tx.AppearsIn;
                    Debug.Assert(appearsIn != null);
                    // If the set of blocks this transaction appears in is disjoint with one of the chain segments it means
                    // the transaction was never incorporated by a miner into that side of the chain.
                    var inOldSection = appearsIn.Any(oldBlocks.Contains) || oldBlocks.Any(appearsIn.Contains);
                    var inNewSection = appearsIn.Any(newBlocks.Contains) || newBlocks.Any(appearsIn.Contains);
                    var inCommonSection = !inNewSection && !inOldSection;

                    if (inCommonSection)
                    {
                        Debug.Assert(!commonChainTransactions.ContainsKey(tx.Hash), "Transaction appears twice in common chain segment");
                        commonChainTransactions[tx.Hash] = tx;
                    }
                    else
                    {
                        if (inOldSection)
                        {
                            Debug.Assert(!oldChainTransactions.ContainsKey(tx.Hash), "Transaction appears twice in old chain segment");
                            oldChainTransactions[tx.Hash] = tx;
                            if (!inNewSection)
                            {
                                Debug.Assert(!onlyOldChainTransactions.ContainsKey(tx.Hash), "Transaction appears twice in only-old map");
                                onlyOldChainTransactions[tx.Hash] = tx;
                            }
                        }
                        if (inNewSection)
                        {
                            Debug.Assert(!newChainTransactions.ContainsKey(tx.Hash), "Transaction appears twice in new chain segment");
                            newChainTransactions[tx.Hash] = tx;
                        }
                    }
                }

                // If there is no difference it means we have nothing we need to do and the user does not care.
                var affectedUs = oldChainTransactions.Count != newChainTransactions.Count ||
                                 !oldChainTransactions.All(
                                     item =>
                                     {
                                         Transaction rightValue;
                                         return newChainTransactions.TryGetValue(item.Key, out rightValue) && Equals(item.Value, rightValue);
                                     });
                _log.Info(affectedUs ? "Re-org affected our transactions" : "Re-org had no effect on our transactions");
                if (!affectedUs) return;

                // For simplicity we will reprocess every transaction to ensure it's in the right bucket and has the right
                // connections. Attempting to update each one with minimal work is possible but complex and was leading to
                // edge cases that were hard to fix. As re-orgs are rare the amount of work this implies should be manageable
                // unless the user has an enormous wallet. As an optimization fully spent transactions buried deeper than
                // 1000 blocks could be put into yet another bucket which we never touch and assume re-orgs cannot affect.

                foreach (var tx in onlyOldChainTransactions.Values) _log.InfoFormat("  Only Old: {0}", tx.HashAsString);
                foreach (var tx in oldChainTransactions.Values) _log.InfoFormat("  Old: {0}", tx.HashAsString);
                foreach (var tx in newChainTransactions.Values) _log.InfoFormat("  New: {0}", tx.HashAsString);

                // Break all the existing connections.
                foreach (var tx in all.Values)
                    tx.DisconnectInputs();
                foreach (var tx in Pending.Values)
                    tx.DisconnectInputs();
                // Reconnect the transactions in the common part of the chain.
                foreach (var tx in commonChainTransactions.Values)
                {
                    var badInput = tx.ConnectForReorganize(all);
                    Debug.Assert(badInput == null, "Failed to connect " + tx.HashAsString + ", " + badInput);
                }
                // Recalculate the unspent/spent buckets for the transactions the re-org did not affect.
                Unspent.Clear();
                Spent.Clear();
                _inactive.Clear();
                foreach (var tx in commonChainTransactions.Values)
                {
                    var unspentOutputs = 0;
                    foreach (var output in tx.Outputs)
                    {
                        if (output.IsAvailableForSpending) unspentOutputs++;
                    }
                    if (unspentOutputs > 0)
                    {
                        _log.InfoFormat("  TX {0}: ->unspent", tx.HashAsString);
                        Unspent[tx.Hash] = tx;
                    }
                    else
                    {
                        _log.InfoFormat("  TX {0}: ->spent", tx.HashAsString);
                        Spent[tx.Hash] = tx;
                    }
                }
                // Now replay the act of receiving the blocks that were previously in a side chain. This will:
                //   - Move any transactions that were pending and are now accepted into the right bucket.
                //   - Connect the newly active transactions.
                foreach (var b in newBlocks.Reverse()) // Need bottom-to-top but we get top-to-bottom.
                {
                    _log.InfoFormat("Replaying block {0}", b.Header.HashAsString);
                    ICollection<Transaction> txns = new HashSet<Transaction>();
                    foreach (var tx in newChainTransactions.Values)
                    {
                        if (tx.AppearsIn.Contains(b))
                        {
                            txns.Add(tx);
                            _log.InfoFormat("  containing tx {0}", tx.HashAsString);
                        }
                    }
                    foreach (var t in txns)
                    {
                        Receive(t, b, BlockChain.NewBlockType.BestChain, true);
                    }
                }

                // Find the transactions that didn't make it into the new chain yet. For each input, try to connect it to the
                // transactions that are in {spent,unspent,pending}. Check the status of each input. For inactive
                // transactions that only send us money, we put them into the inactive pool where they sit around waiting for
                // another re-org or re-inclusion into the main chain. For inactive transactions where we spent money we must
                // put them back into the pending pool if we can reconnect them, so we don't create a double spend whilst the
                // network heals itself.
                IDictionary<Sha256Hash, Transaction> pool = new Dictionary<Sha256Hash, Transaction>();
                foreach (var pair in Unspent.Concat(Spent).Concat(Pending))
                {
                    pool[pair.Key] = pair.Value;
                }
                IDictionary<Sha256Hash, Transaction> toReprocess = new Dictionary<Sha256Hash, Transaction>();
                foreach (var pair in onlyOldChainTransactions.Concat(Pending))
                {
                    toReprocess[pair.Key] = pair.Value;
                }
                _log.Info("Reprocessing:");
                // Note, we must reprocess dead transactions first. The reason is that if there is a double spend across
                // chains from our own coins we get a complicated situation:
                //
                // 1) We switch to a new chain (B) that contains a double spend overriding a pending transaction. It goes dead.
                // 2) We switch BACK to the first chain (A). The dead transaction must go pending again.
                // 3) We resurrect the transactions that were in chain (B) and assume the miners will start work on putting them
                //    in to the chain, but it's not possible because it's a double spend. So now that transaction must become
                //    dead instead of pending.
                //
                // This only occurs when we are double spending our own coins.
                foreach (var tx in _dead.Values.ToList())
                {
                    ReprocessTxAfterReorg(pool, tx);
                }
                foreach (var tx in toReprocess.Values)
                {
                    ReprocessTxAfterReorg(pool, tx);
                }

                _log.InfoFormat("post-reorg balance is {0}", Utils.BitcoinValueToFriendlyString(GetBalance()));

                // Inform event listeners that a re-org took place.
                if (Reorganized != null)
                {
                    // Synchronize on the event listener as well. This allows a single listener to handle events from
                    // multiple wallets without needing to worry about being thread safe.
                    lock (Reorganized)
                    {
                        Reorganized(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void ReprocessTxAfterReorg(IDictionary<Sha256Hash, Transaction> pool, Transaction tx)
        {
            _log.InfoFormat("  TX {0}", tx.HashAsString);
            var numInputs = tx.Inputs.Count;
            var noSuchTx = 0;
            var success = 0;
            var isDead = false;
            foreach (var input in tx.Inputs)
            {
                if (input.IsCoinBase)
                {
                    // Input is not in our wallet so there is "no such input tx", bit of an abuse.
                    noSuchTx++;
                    continue;
                }
                var result = input.Connect(pool, false);
                if (result == TransactionInput.ConnectionResult.Success)
                {
                    success++;
                }
                else if (result == TransactionInput.ConnectionResult.NoSuchTx)
                {
                    noSuchTx++;
                }
                else if (result == TransactionInput.ConnectionResult.AlreadySpent)
                {
                    isDead = true;
                    // This transaction was replaced by a double spend on the new chain. Did you just reverse
                    // your own transaction? I hope not!!
                    _log.Info("   ->dead, will not confirm now unless there's another re-org");
                    var doubleSpent = input.GetConnectedOutput(pool);
                    var replacement = doubleSpent.SpentBy.ParentTransaction;
                    _dead[tx.Hash] = tx;
                    Pending.Remove(tx.Hash);
                    // Inform the event listeners of the newly dead tx.
                    if (DeadTransaction != null)
                    {
                        lock (DeadTransaction)
                        {
                            DeadTransaction(this, new WalletDeadTransactionEventArgs(tx, replacement));
                        }
                    }
                    break;
                }
            }
            if (isDead) return;

            if (noSuchTx == numInputs)
            {
                _log.Info("   ->inactive");
                _inactive[tx.Hash] = tx;
            }
            else if (success == numInputs - noSuchTx)
            {
                // All inputs are either valid for spending or don't come from us. Miners are trying to re-include it.
                _log.Info("   ->pending");
                Pending[tx.Hash] = tx;
                _dead.Remove(tx.Hash);
            }
        }

        /// <summary>
        /// Returns an immutable view of the transactions currently waiting for network confirmations.
        /// </summary>
        public ICollection<Transaction> PendingTransactions
        {
            get { return Pending.Values; }
        }
    }

    /// <summary>
    /// This is called on a Peer thread when a block is received that sends some coins to you. Note that this will
    /// also be called when downloading the block chain as the wallet balance catches up so if you don't want that
    /// register the event listener after the chain is downloaded. It's safe to use methods of wallet during the
    /// execution of this callback.
    /// </summary>
    public class WalletCoinsReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The transaction which sent us the coins.
        /// </summary>
        public Transaction Tx { get; private set; }

        /// <summary>
        /// Balance before the coins were received.
        /// </summary>
        public ulong PrevBalance { get; private set; }

        /// <summary>
        /// Current balance of the wallet.
        /// </summary>
        public ulong NewBalance { get; private set; }

        /// <param name="tx">The transaction which sent us the coins.</param>
        /// <param name="prevBalance">Balance before the coins were received.</param>
        /// <param name="newBalance">Current balance of the wallet.</param>
        public WalletCoinsReceivedEventArgs(Transaction tx, ulong prevBalance, ulong newBalance)
        {
            Tx = tx;
            PrevBalance = prevBalance;
            NewBalance = newBalance;
        }
    }

    /// <summary>
    /// This is called on a Peer thread when a transaction becomes <i>dead</i>. A dead transaction is one that has
    /// been overridden by a double spend from the network and so will never confirm no matter how long you wait.
    /// </summary>
    /// <remarks>
    /// A dead transaction can occur if somebody is attacking the network, or by accident if keys are being shared.
    /// You can use this event handler to inform the user of the situation. A dead spend will show up in the BitCoin
    /// C++ client of the recipient as 0/unconfirmed forever, so if it was used to purchase something,
    /// the user needs to know their goods will never arrive.
    /// </remarks>
    public class WalletDeadTransactionEventArgs : EventArgs
    {
        /// <summary>
        /// The transaction that is newly dead.
        /// </summary>
        public Transaction DeadTx { get; private set; }

        /// <summary>
        /// The transaction that killed it.
        /// </summary>
        public Transaction ReplacementTx { get; private set; }

        /// <param name="deadTx">The transaction that is newly dead.</param>
        /// <param name="replacementTx">The transaction that killed it.</param>
        public WalletDeadTransactionEventArgs(Transaction deadTx, Transaction replacementTx)
        {
            DeadTx = deadTx;
            ReplacementTx = replacementTx;
        }
    }
}