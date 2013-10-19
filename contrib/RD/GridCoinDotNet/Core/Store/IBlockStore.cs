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

namespace BitCoinSharp.Store
{
    /// <summary>
    /// An implementor of BlockStore saves StoredBlock objects to disk. Different implementations store them in
    /// different ways. An in-memory implementation (MemoryBlockStore) exists for unit testing but real apps will want to
    /// use implementations that save to disk.
    /// </summary>
    /// <remarks>
    /// A BlockStore is a map of hashes to StoredBlock. The hash is the double digest of the BitCoin serialization
    /// of the block header, <b>not</b> the header with the extra data as well.<p/>
    /// BlockStores are thread safe.
    /// </remarks>
    public interface IBlockStore : IDisposable
    {
        /// <summary>
        /// Saves the given block header+extra data. The key isn't specified explicitly as it can be calculated from the
        /// StoredBlock directly. Can throw if there is a problem with the underlying storage layer such as running out of
        /// disk space.
        /// </summary>
        /// <exception cref="BlockStoreException"/>
        void Put(StoredBlock block);

        /// <summary>
        /// Returns the StoredBlock given a hash. The returned values block.getHash() method will be equal to the
        /// parameter. If no such block is found, returns null.
        /// </summary>
        /// <exception cref="BlockStoreException"/>
        StoredBlock Get(Sha256Hash hash);

        /// <summary>
        /// Returns the <see cref="StoredBlock"/> that represents the top of the chain of greatest total work.
        /// </summary>
        /// <exception cref="BlockStoreException"/>
        StoredBlock GetChainHead();

        /// <summary>
        /// Sets the <see cref="StoredBlock"/> that represents the top of the chain of greatest total work.
        /// </summary>
        /// <exception cref="BlockStoreException"/>
        void SetChainHead(StoredBlock chainHead);
    }
}