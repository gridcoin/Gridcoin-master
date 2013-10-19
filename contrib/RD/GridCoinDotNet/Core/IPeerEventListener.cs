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

namespace BitCoinSharp
{
    /// <summary>
    /// Implementing a PeerEventListener allows you to learn when significant Peer communication
    /// has occurred.
    /// </summary>
    /// <remarks>
    /// Methods are called with the event listener object locked so your
    /// implementation does not have to be thread safe. 
    ///  
    /// @author miron@google.com (Miron Cuperman a.k.a devrandom)
    /// </remarks>
    public interface IPeerEventListener
    {
        /// <summary>
        /// Called on a Peer thread when a block is received.
        /// </summary>
        /// <remarks>
        /// The block may have transactions or may be a header only once getheaders is implemented.
        /// </remarks>
        /// <param name="peer">The peer receiving the block.</param>
        /// <param name="block">The downloaded block.</param>
        /// <param name="blocksLeft">The number of blocks left to download.</param>
        void OnBlocksDownloaded(Peer peer, Block block, int blocksLeft);

        /// <summary>
        /// Called when a download is started with the initial number of blocks to be downloaded.
        /// </summary>
        /// <param name="peer">The peer receiving the block.</param>
        /// <param name="blocksLeft">The number of blocks left to download.</param>
        void OnChainDownloadStarted(Peer peer, int blocksLeft);
    }
}