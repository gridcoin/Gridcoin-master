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
using Org.BouncyCastle.Math;

namespace BitCoinSharp
{
    /// <summary>
    /// Parses and generates private keys in the form used by the BitCoin "dumpprivkey" command. This is the private key
    /// bytes with a header byte and 4 checksum bytes at the end.
    /// </summary>
    public class DumpedPrivateKey : VersionedChecksummedBytes
    {
        // Used by EcKey.PrivateKeyEncoded
        internal DumpedPrivateKey(NetworkParameters @params, byte[] keyBytes)
            : base(@params.DumpedPrivateKeyHeader, keyBytes)
        {
            if (keyBytes.Length != 32) // 256 bit keys
                throw new ArgumentException("Keys are 256 bits, so you must provide 32 bytes, got " + keyBytes.Length + " bytes", "keyBytes");
        }

        /// <summary>
        /// Parses the given private key as created by the "dumpprivkey" BitCoin C++ RPC.
        /// </summary>
        /// <param name="params">The expected network parameters of the key. If you don't care, provide null.</param>
        /// <param name="encoded">The base58 encoded string.</param>
        /// <exception cref="AddressFormatException">If the string is invalid or the header byte doesn't match the network params.</exception>
        public DumpedPrivateKey(NetworkParameters @params, string encoded)
            : base(encoded)
        {
            if (@params != null && Version != @params.DumpedPrivateKeyHeader)
                throw new AddressFormatException("Mismatched version number, trying to cross networks? " + Version +
                                                 " vs " + @params.DumpedPrivateKeyHeader);
        }

        /// <summary>
        /// Returns an ECKey created from this encoded private key.
        /// </summary>
        public EcKey Key
        {
            get { return new EcKey(new BigInteger(1, Bytes)); }
        }
    }
}