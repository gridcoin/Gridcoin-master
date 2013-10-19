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

namespace BitCoinSharp
{
    /// <summary>
    /// Thrown when a problem occurs in communicating with a peer, and we should
    /// retry.
    /// </summary>
    public class PeerException : Exception
    {
        public PeerException()
        {
        }

        public PeerException(string message)
            : base(message)
        {
        }

        public PeerException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public PeerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}