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
using BitCoinSharp.Store;
using Org.BouncyCastle.Math;

namespace BitCoinSharp
{
    /// <summary>
    /// Wraps a <see cref="Block"/> object with extra data that can be derived from the block chain but is slow or inconvenient to
    /// calculate. By storing it alongside the block header we reduce the amount of work required significantly.
    /// Recalculation is slow because the fields are cumulative - to find the chainWork you have to iterate over every
    /// block in the chain back to the genesis block, which involves lots of seeking/loading etc. So we just keep a
    /// running total: it's a disk space vs CPU/IO tradeoff.
    /// </summary>
    /// <remarks>
    /// StoredBlocks are put inside a <see cref="IBlockStore"/> which saves them to memory or disk.
    /// </remarks>
    [Serializable]
    public class StoredBlock
    {
        private readonly Block _header;
        private readonly BigInteger _chainWork;
        private readonly uint _height;

        public StoredBlock(Block header, BigInteger chainWork, uint height)
        {
            _header = header;
            _chainWork = chainWork;
            _height = height;
        }

        /// <summary>
        /// The block header this object wraps. The referenced block object must not have any transactions in it.
        /// </summary>
        public Block Header
        {
            get { return _header; }
        }

        /// <summary>
        /// The total sum of work done in this block, and all the blocks below it in the chain. Work is a measure of how
        /// many tries are needed to solve a block. If the target is set to cover 10% of the total hash value space,
        /// then the work represented by a block is 10.
        /// </summary>
        public BigInteger ChainWork
        {
            get { return _chainWork; }
        }

        /// <summary>
        /// Position in the chain for this block. The genesis block has a height of zero.
        /// </summary>
        public uint Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Returns true if this objects chainWork is higher than the others.
        /// </summary>
        public bool MoreWorkThan(StoredBlock other)
        {
            return _chainWork.CompareTo(other._chainWork) > 0;
        }

        public override bool Equals(object other)
        {
            if (!(other is StoredBlock)) return false;
            var o = (StoredBlock) other;
            return o._header.Equals(_header) && o._chainWork.Equals(_chainWork) && o._height == _height;
        }

        public override int GetHashCode()
        {
            // A better hashCode is possible, but this works for now.
            return _header.GetHashCode() ^ _chainWork.GetHashCode() ^ (int) _height;
        }

        /// <summary>
        /// Creates a new StoredBlock, calculating the additional fields by adding to the values in this block.
        /// </summary>
        /// <exception cref="VerificationException"/>
        public StoredBlock Build(Block block)
        {
            // Stored blocks track total work done in this chain, because the canonical chain is the one that represents
            // the largest amount of work done not the tallest.
            var chainWork = _chainWork.Add(block.GetWork());
            var height = _height + 1;
            return new StoredBlock(block.CloneAsHeader(), chainWork, height);
        }

        /// <summary>
        /// Given a block store, looks up the previous block in this chain. Convenience method for doing
        /// <tt>store.get(this.getHeader().getPrevBlockHash())</tt>.
        /// </summary>
        /// <returns>The previous block in the chain or null if it was not found in the store.</returns>
        /// <exception cref="BlockStoreException"/>
        public StoredBlock GetPrev(IBlockStore store)
        {
            return store.Get(Header.PrevBlockHash);
        }

        public override string ToString()
        {
            return string.Format("Block {0} at height {1}: {2}",
                                 Header.HashAsString, Height, Header);
        }
    }
}