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
using System.Threading;
using BitCoinSharp.Common;

namespace BitCoinSharp
{
    /// <summary>
    /// Listen to chain download events and print useful informational messages.
    /// </summary>
    /// <remarks>
    /// Progress, StartDownload, DoneDownload maybe be overridden to change the way the user
    /// is notified.
    /// <p/>
    /// Methods are called with the event listener object locked so your
    /// implementation does not have to be thread safe. 
    /// 
    /// @author miron@google.com (Miron Cuperman a.k.a. devrandom)
    /// </remarks>
    public class DownloadListener : AbstractPeerEventListener
    {
        private int _originalBlocksLeft = -1;
        private int _lastPercent;
        //private readonly Semaphore _done = new Semaphore(0, 0);
        private readonly Semaphore _done = new Semaphore(0, 8);
        public override void OnChainDownloadStarted(Peer peer, int blocksLeft)
        {
            StartDownload(blocksLeft);
            _originalBlocksLeft = blocksLeft;
        }

        public override void OnBlocksDownloaded(Peer peer, Block block, int blocksLeft)
        {
            if (blocksLeft == 0)
            {
                DoneDownload();
                _done.Release();
            }

            if (blocksLeft < 0 || _originalBlocksLeft <= 0)
                return;

            var pct = 100.0 - (100.0*(blocksLeft/(double) _originalBlocksLeft));
            if ((int) pct != _lastPercent)
            {
                Progress(pct, UnixTime.FromUnixTime(block.TimeSeconds*1000));
                _lastPercent = (int) pct;
            }
        }

        /// <summary>
        /// Called when download progress is made.
        /// </summary>
        /// <param name="pct">The percentage of chain downloaded, estimated.</param>
        /// <param name="date">The date of the last block downloaded.</param>
        protected void Progress(double pct, DateTime date)
        {
            Console.WriteLine(string.Format("Chain download {0}% done, block date {1}", (int) pct, date));
        }

        /// <summary>
        /// Called when download is initiated.
        /// </summary>
        /// <param name="blocks">The number of blocks to download, estimated.</param>
        protected void StartDownload(int blocks)
        {
            Console.WriteLine("Downloading block chain of size " + blocks + ". " +
                              (blocks > 1000 ? "This may take a while." : ""));
        }

        /// <summary>
        /// Called when we are done downloading the block chain.
        /// </summary>
        protected void DoneDownload()
        {
            Console.WriteLine("Done downloading block chain");
        }

        /// <summary>
        /// Wait for the chain to be downloaded. 
        /// </summary>
        /// <exception cref="ThreadInterruptedException"/>
        public void Await()
        {
            _done.WaitOne();
        }
    }
}