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
using System.Diagnostics;
using System.Linq;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace BitCoinSharp
{
    /// <summary>
    /// A Sha256Hash just wraps a byte[] so that equals and hashcode work correctly, allowing it to be used as keys in a
    /// map. It also checks that the length is correct and provides a bit more type safety.
    /// </summary>
    [Serializable]
    public class Sha256Hash
    {
        private readonly byte[] _bytes;

        public static readonly Sha256Hash ZeroHash = new Sha256Hash(new byte[32]);

        /// <summary>
        /// Creates a Sha256Hash by wrapping the given byte array. It must be 32 bytes long.
        /// </summary>
        public Sha256Hash(byte[] bytes)
        {
            Debug.Assert(bytes.Length == 32);
            _bytes = bytes;
        }

        /// <summary>
        /// Creates a Sha256Hash by decoding the given hex string. It must be 64 characters long.
        /// </summary>
        public Sha256Hash(string @string)
        {
            Debug.Assert(@string.Length == 64);
            _bytes = Hex.Decode(@string);
        }

        /// <summary>
        /// Returns true if the hashes are equal.
        /// </summary>
        public override bool Equals(object other)
        {
            if (!(other is Sha256Hash)) return false;
            return _bytes.SequenceEqual(((Sha256Hash) other)._bytes);
        }

        /// <summary>
        /// Hash code of the byte array as calculated by <see cref="object.GetHashCode"/>. Note the difference between a SHA256
        /// secure bytes and the type of quick/dirty bytes used by the Java hashCode method which is designed for use in
        /// bytes tables.
        /// </summary>
        public override int GetHashCode()
        {
            return _bytes != null ? _bytes.Aggregate(1, (current, element) => 31*current + element) : 0;
        }

        public override string ToString()
        {
            return Utils.BytesToHexString(_bytes);
        }

        /// <summary>
        /// Returns the bytes interpreted as a positive integer.
        /// </summary>
        public BigInteger ToBigInteger()
        {
            return new BigInteger(1, _bytes);
        }

        public byte[] Bytes
        {
            get { return _bytes; }
        }

        public Sha256Hash Duplicate()
        {
            return new Sha256Hash(_bytes);
        }
    }
}