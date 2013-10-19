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
using log4net;

namespace BitCoinSharp.Store
{
    /// <summary>
    /// Stores the block chain to disk but still holds it in memory. This is intended for desktop apps and tests.
    /// Constrained environments like mobile phones probably won't want to or be able to store all the block headers in RAM.
    /// </summary>
    public class DiskBlockStore : IBlockStore
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (DiskBlockStore));

        private FileStream _stream;
        private readonly IDictionary<Sha256Hash, StoredBlock> _blockMap;
        private Sha256Hash _chainHead;
        private readonly NetworkParameters _params;

        /// <exception cref="BlockStoreException"/>
        public DiskBlockStore(NetworkParameters @params, FileInfo file)
        {
            _params = @params;
            _blockMap = new Dictionary<Sha256Hash, StoredBlock>();
            try
            {
                Load(file);
                if (_stream != null)
                {
                    _stream.Dispose();
                }
                _stream = file.Open(FileMode.Append, FileAccess.Write); // Do append.
            }
            catch (IOException e)
            {
                _log.Error("failed to load block store from file", e);
                CreateNewStore(@params, file);
            }
        }

        /// <exception cref="BlockStoreException"/>
        private void CreateNewStore(NetworkParameters @params, FileInfo file)
        {
            // Create a new block store if the file wasn't found or anything went wrong whilst reading.
            _blockMap.Clear();
            try
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                }
                _stream = file.OpenWrite(); // Do not append, create fresh.
                _stream.Write(1); // Version.
            }
            catch (IOException e1)
            {
                // We could not load a block store nor could we create a new one!
                throw new BlockStoreException(e1);
            }
            try
            {
                // Set up the genesis block. When we start out fresh, it is by definition the top of the chain.
                var genesis = @params.GenesisBlock.CloneAsHeader();
                var storedGenesis = new StoredBlock(genesis, genesis.GetWork(), 0);
                _chainHead = storedGenesis.Header.Hash;
                _stream.Write(_chainHead.Bytes);
                Put(storedGenesis);
            }
            catch (IOException e)
            {
                throw new BlockStoreException(e);
            }
        }

        /// <exception cref="IOException"/>
        /// <exception cref="BlockStoreException"/>
        private void Load(FileInfo file)
        {
            _log.InfoFormat("Reading block store from {0}", file);
            using (var input = file.OpenRead())
            {
                // Read a version byte.
                var version = input.Read();
                if (version == -1)
                {
                    // No such file or the file was empty.
                    throw new FileNotFoundException(file.Name + " does not exist or is empty");
                }
                if (version != 1)
                {
                    throw new BlockStoreException("Bad version number: " + version);
                }
                // Chain head pointer is the first thing in the file.
                var chainHeadHash = new byte[32];
                if (input.Read(chainHeadHash) < chainHeadHash.Length)
                    throw new BlockStoreException("Truncated block store: cannot read chain head hash");
                _chainHead = new Sha256Hash(chainHeadHash);
                _log.InfoFormat("Read chain head from disk: {0}", _chainHead);
                var now = Environment.TickCount;
                // Rest of file is raw block headers.
                var headerBytes = new byte[Block.HeaderSize];
                try
                {
                    while (true)
                    {
                        // Read a block from disk.
                        if (input.Read(headerBytes) < 80)
                        {
                            // End of file.
                            break;
                        }
                        // Parse it.
                        var b = new Block(_params, headerBytes);
                        // Look up the previous block it connects to.
                        var prev = Get(b.PrevBlockHash);
                        StoredBlock s;
                        if (prev == null)
                        {
                            // First block in the stored chain has to be treated specially.
                            if (b.Equals(_params.GenesisBlock))
                            {
                                s = new StoredBlock(_params.GenesisBlock.CloneAsHeader(), _params.GenesisBlock.GetWork(), 0);
                            }
                            else
                            {
                                throw new BlockStoreException("Could not connect " + b.Hash + " to " + b.PrevBlockHash);
                            }
                        }
                        else
                        {
                            // Don't try to verify the genesis block to avoid upsetting the unit tests.
                            b.VerifyHeader();
                            // Calculate its height and total chain work.
                            s = prev.Build(b);
                        }
                        // Save in memory.
                        _blockMap[b.Hash] = s;
                    }
                }
                catch (ProtocolException e)
                {
                    // Corrupted file.
                    throw new BlockStoreException(e);
                }
                catch (VerificationException e)
                {
                    // Should not be able to happen unless the file contains bad blocks.
                    throw new BlockStoreException(e);
                }
                var elapsed = Environment.TickCount - now;
                _log.InfoFormat("Block chain read complete in {0}ms", elapsed);
            }
        }

        /// <exception cref="BlockStoreException"/>
        public void Put(StoredBlock block)
        {
            lock (this)
            {
                try
                {
                    var hash = block.Header.Hash;
                    Debug.Assert(!_blockMap.ContainsKey(hash), "Attempt to insert duplicate");
                    // Append to the end of the file. The other fields in StoredBlock will be recalculated when it's reloaded.
                    var bytes = block.Header.BitcoinSerialize();
                    _stream.Write(bytes);
                    _stream.Flush();
                    _blockMap[hash] = block;
                }
                catch (IOException e)
                {
                    throw new BlockStoreException(e);
                }
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

        /// <exception cref="BlockStoreException"/>
        public StoredBlock GetChainHead()
        {
            lock (this)
            {
                StoredBlock block;
                _blockMap.TryGetValue(_chainHead, out block);
                return block;
            }
        }

        /// <exception cref="BlockStoreException"/>
        public void SetChainHead(StoredBlock chainHead)
        {
            lock (this)
            {
                try
                {
                    _chainHead = chainHead.Header.Hash;
                    // Write out new hash to the first 32 bytes of the file past one (first byte is version number).
                    _stream.Seek(1, SeekOrigin.Begin);
                    var bytes = _chainHead.Bytes;
                    _stream.Write(bytes, 0, bytes.Length);
                }
                catch (IOException e)
                {
                    throw new BlockStoreException(e);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        #endregion
    }
}