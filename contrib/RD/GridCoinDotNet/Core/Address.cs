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
    /// A BitCoin address is fundamentally derived from an elliptic curve public key and a set of network parameters.
    /// </summary>
    /// <remarks>
    /// It has several possible representations:<p/>
    /// <ol>
    ///   <li>The raw public key bytes themselves.</li>
    ///   <li>RIPEMD160 hash of the public key bytes.</li>
    ///   <li>A base58 encoded "human form" that includes a version and check code, to guard against typos.</li>
    /// </ol><p/>
    /// One may question whether the base58 form is really an improvement over the hash160 form, given
    /// they are both very unfriendly for typists. More useful representations might include QR codes
    /// and identicons.<p/>
    /// Note that an address is specific to a network because the first byte is a discriminator value.
    /// </remarks>
    public class Address : VersionedChecksummedBytes
    {
        /// <summary>
        /// Construct an address from parameters and the hash160 form.
        /// </summary>
        /// <remarks>
        /// Example:<p/>
        /// <pre>new Address(NetworkParameters.prodNet(), Hex.decode("4a22c3c4cbb31e4d03b15550636762bda0baf85a"));</pre>
        /// </remarks>
        public Address(NetworkParameters @params, byte[] hash160)
            : base(@params.AddressHeader, hash160)
        {
            if (hash160.Length != 20) // 160 = 8 * 20
                throw new ArgumentException("Addresses are 160-bit hashes, so you must provide 20 bytes", "hash160");
        }

        /// <summary>
        /// Construct an address from parameters and the standard "human readable" form.
        /// </summary>
        /// <remarks>
        /// Example:<p/>
        /// <pre>new Address(NetworkParameters.prodNet(), "17kzeh4N8g49GFvdDzSf8PjaPfyoD1MndL");</pre>
        /// </remarks>
        /// <exception cref="AddressFormatException"/>
        public Address(NetworkParameters @params, string address)
            : base(address)
        {
            if (Version != @params.AddressHeader)
                throw new AddressFormatException("Mismatched version number, trying to cross networks? " + Version +
                                                 " vs " + @params.AddressHeader);
        }

        /// <summary>
        /// The (big endian) 20 byte hash that is the core of a BitCoin address.
        /// </summary>
        public byte[] Hash160
        {
            get { return Bytes; }
        }
    }
}