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

using System.Diagnostics;
using System.IO;
using BitCoinSharp.Collections.Generic;
using BitCoinSharp.IO;
using Org.BouncyCastle.Math;
using log4net;

namespace BitCoinSharp.Store
{
    /// <summary>
    /// Stores the block chain to disk.
    /// </summary>
    /// <remarks>
    /// This implementation is designed to have constant memory usage, regardless of the size of the block chain being
    /// stored. It exploits operating system level buffering and the fact that get() requests are, in normal usage,
    /// localized in chain space.<p/>
    /// Blocks are stored sequentially. Most blocks are fetched out of a small in-memory cache. The slowest part is
    /// traversing difficulty transition points, which requires seeking backwards over around 2000 blocks. On a Google
    /// Nexus S phone this takes a couple of seconds. On a MacBook Pro it takes around 50msec.<p/>
    /// The store has much room for optimization. Expanding the size of the cache will likely allow us to traverse
    /// difficulty transitions without using too much memory and without hitting the disk at all, for the case of initial
    /// block chain download. Storing the hashes on disk would allow us to avoid deserialization and hashing which is
    /// expensive on Android.
    /// </remarks>
    public class BoundedOverheadBlockStore : IBlockStore
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (BoundedOverheadBlockStore));
        private const byte _fileFormatVersion = 1;

        // We keep some recently found blocks in the blockCache. It can help to optimize some cases where we are
        // looking up blocks we recently stored or requested. When the cache gets too big older entries are deleted.
        private readonly OrderedDictionary<Sha256Hash, StoredBlock> _blockCache = new OrderedDictionary<Sha256Hash, StoredBlock>();
        // Use a separate cache to track get() misses. This is to efficiently handle the case of an unconnected block
        // during chain download. Each new block will do a get() on the unconnected block so if we haven't seen it yet we
        // must efficiently respond.
        //
        // We don't care about the value in this cache. It is always notFoundMarker. Unfortunately LinkedHashSet does not
        // provide the removeEldestEntry control.
        private readonly StoredBlock _notFoundMarker = new StoredBlock(null, null, uint.MaxValue);
        private readonly OrderedDictionary<Sha256Hash, StoredBlock> _notFoundCache = new OrderedDictionary<Sha256Hash, StoredBlock>();

        private Sha256Hash _chainHead;
        private readonly NetworkParameters _params;
        private FileStream _channel;

        private class Record
        {
            // A BigInteger representing the total amount of work done so far on this chain. As of May 2011 it takes 8
            // bytes to represent this field, so 16 bytes should be plenty for a long time.
            private const int _chainWorkBytes = 16;
            private static readonly byte[] _emptyBytes = new byte[_chainWorkBytes];

            private uint _height; // 4 bytes
            private readonly byte[] _chainWork; // 16 bytes
            private readonly byte[] _blockHeader; // 80 bytes

            public const int Size = 4 + _chainWorkBytes + Block.HeaderSize;

            public Record()
            {
                _height = 0;
                _chainWork = new byte[_chainWorkBytes];
                _blockHeader = new byte[Block.HeaderSize];
            }

            /// <exception cref="IOException"/>
            public static void Write(Stream channel, StoredBlock block)
            {
                using (var buf = ByteBuffer.Allocate(Size))
                {
                    buf.PutInt((int) block.Height);
                    var chainWorkBytes = block.ChainWork.ToByteArray();
                    Debug.Assert(chainWorkBytes.Length <= _chainWorkBytes, "Ran out of space to store chain work!");
                    if (chainWorkBytes.Length < _chainWorkBytes)
                    {
                        // Pad to the right size.
                        buf.Put(_emptyBytes, 0, _chainWorkBytes - chainWorkBytes.Length);
                    }
                    buf.Put(chainWorkBytes);
                    buf.Put(block.Header.BitcoinSerialize());
                    buf.Position = 0;
                    channel.Position = channel.Length;
                    channel.Write(buf.ToArray());
                    channel.Position = channel.Length - Size;
                }
            }

            /// <exception cref="IOException"/>
            public bool Read(Stream channel, long position, ByteBuffer buffer)
            {
                buffer.Position = 0;
                var originalPos = channel.Position;
                channel.Position = position;
                var data = new byte[buffer.Length];
                var bytesRead = channel.Read(data);
                buffer.Clear();
                buffer.Write(data);
                channel.Position = originalPos;
                if (bytesRead < Size)
                    return false;
                buffer.Position = 0;
                _height = (uint) buffer.GetInt();
                buffer.Get(_chainWork);
                buffer.Get(_blockHeader);
                return true;
            }

            public BigInteger ChainWork
            {
                get { return new BigInteger(1, _chainWork); }
            }

            /// <exception cref="ProtocolException"/>
            public Block GetHeader(NetworkParameters @params)
            {
                return new Block(@params, _blockHeader);
            }

            public uint Height
            {
                get { return _height; }
            }

            /// <exception cref="ProtocolException"/>
            public StoredBlock ToStoredBlock(NetworkParameters @params)
            {
                return new StoredBlock(GetHeader(@params), ChainWork, Height);
            }
        }

        /// <exception cref="BlockStoreException"/>
        public BoundedOverheadBlockStore(NetworkParameters @params, FileInfo file)
        {
            _params = @params;
            try
            {
                Load(file);
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
            _blockCache.Clear();
            try
            {
                if (_channel != null)
                {
                    _channel.Dispose();
                    _channel = null;
                }
                if (file.Exists)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException)
                    {
                        throw new BlockStoreException("Could not delete old store in order to recreate it");
                    }
                }
                _channel = file.Create(); // Create fresh.
                _channel.Write(_fileFormatVersion);
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
                _channel.Write(_chainHead.Bytes);
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
            if (_channel != null)
            {
                _channel.Dispose();
            }
            _channel = file.OpenRead();
            try
            {
                // Read a version byte.
                var version = _channel.Read();
                if (version == -1)
                {
                    // No such file or the file was empty.
                    throw new FileNotFoundException(file.Name + " does not exist or is empty");
                }
                if (version != _fileFormatVersion)
                {
                    throw new BlockStoreException("Bad version number: " + version);
                }
                // Chain head pointer is the first thing in the file.
                var chainHeadHash = new byte[32];
                if (_channel.Read(chainHeadHash) < chainHeadHash.Length)
                    throw new BlockStoreException("Truncated store: could not read chain head hash.");
                _chainHead = new Sha256Hash(chainHeadHash);
                _log.InfoFormat("Read chain head from disk: {0}", _chainHead);
                _channel.Position = _channel.Length - Record.Size;
            }
            catch (IOException)
            {
                _channel.Close();
                throw;
            }
            catch (BlockStoreException)
            {
                _channel.Close();
                throw;
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
                    // Append to the end of the file.
                    Record.Write(_channel, block);
                    _blockCache[hash] = block;
                    while (_blockCache.Count > 2050)
                    {
                        _blockCache.RemoveAt(0);
                    }
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
                // Check the memory cache first.
                StoredBlock fromMem;
                if (_blockCache.TryGetValue(hash, out fromMem))
                {
                    return fromMem;
                }
                if (_notFoundCache.TryGetValue(hash, out fromMem) && fromMem == _notFoundMarker)
                {
                    return null;
                }

                try
                {
                    var fromDisk = GetRecord(hash);
                    StoredBlock block = null;
                    if (fromDisk == null)
                    {
                        _notFoundCache[hash] = _notFoundMarker;
                        while (_notFoundCache.Count > 2050)
                        {
                            _notFoundCache.RemoveAt(0);
                        }
                    }
                    else
                    {
                        block = fromDisk.ToStoredBlock(_params);
                        _blockCache[hash] = block;
                        while (_blockCache.Count > 2050)
                        {
                            _blockCache.RemoveAt(0);
                        }
                    }
                    return block;
                }
                catch (IOException e)
                {
                    throw new BlockStoreException(e);
                }
                catch (ProtocolException e)
                {
                    throw new BlockStoreException(e);
                }
            }
        }

        private ByteBuffer _buf = ByteBuffer.Allocate(Record.Size);

        /// <exception cref="BlockStoreException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="ProtocolException"/>
        private Record GetRecord(Sha256Hash hash)
        {
            var startPos = _channel.Position;
            // Use our own file pointer within the tight loop as updating channel positions is really expensive.
            var pos = startPos;
            var record = new Record();
            do
            {
                if (!record.Read(_channel, pos, _buf))
                    throw new IOException("Failed to read buffer");
                if (record.GetHeader(_params).Hash.Equals(hash))
                {
                    // Found it. Update file position for next time.
                    _channel.Position = pos;
                    return record;
                }
                // Did not find it.
                if (pos == 1 + 32)
                {
                    // At the start so wrap around to the end.
                    pos = _channel.Length - Record.Size;
                }
                else
                {
                    // Move backwards.
                    pos = pos - Record.Size;
                    Debug.Assert(pos >= 1 + 32, pos.ToString());
                }
            } while (pos != startPos);
            // Was never stored.
            _channel.Position = pos;
            return null;
        }

        /// <exception cref="BlockStoreException"/>
        public StoredBlock GetChainHead()
        {
            lock (this)
            {
                var head = Get(_chainHead);
                if (head == null)
                    throw new BlockStoreException("Corrupted block store: chain head not found");
                return head;
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
                    var originalPos = _channel.Position;
                    _channel.Position = 1;
                    var bytes = _chainHead.Bytes;
                    _channel.Write(bytes, 0, bytes.Length);
                    _channel.Position = originalPos;
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
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
            if (_buf != null)
            {
                _buf.Dispose();
                _buf = null;
            }
        }

        #endregion
    }
}