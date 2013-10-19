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

namespace BitCoinSharp.Store
{
    /// <summary>
    /// Keeps <see cref="StoredBlock"/>s in memory. Used primarily for unit testing.
    /// </summary>
    public class MemoryBlockStore : IBlockStore
    {
        private readonly IDictionary<Sha256Hash, StoredBlock> _blockMap;
        private StoredBlock _chainHead;

        public MemoryBlockStore(NetworkParameters @params)
        {
            _blockMap = new Dictionary<Sha256Hash, StoredBlock>();
            // Insert the genesis block.
            var genesisHeader = @params.GenesisBlock.CloneAsHeader();
            var storedGenesis = new StoredBlock(genesisHeader, genesisHeader.GetWork(), 0);
            Put(storedGenesis);
            SetChainHead(storedGenesis);
        }

        /// <exception cref="BlockStoreException"/>
        public void Put(StoredBlock block)
        {
            lock (this)
            {
                var hash = block.Header.Hash;
                _blockMap[hash] = block;
            }
        }

        /// <exception cref="BlockStoreException"/>
        public StoredBlock Get(Sha256Hash hash)
        {
            lock (this)
            {
                StoredBlock block;
                _blockMap.TryGetValue(hash, out block);
                return block;
            }
        }

        public StoredBlock GetChainHead()
        {
            return _chainHead;
        }

        /// <exception cref="BlockStoreException"/>
        public void SetChainHead(StoredBlock chainHead)
        {
            _chainHead = chainHead;
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}