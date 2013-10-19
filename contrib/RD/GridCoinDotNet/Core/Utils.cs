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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BitCoinSharp.IO;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace BitCoinSharp
{
    /// <summary>
    /// A collection of various utility methods that are helpful for working with the BitCoin protocol.
    /// To enable debug logging from the library, run with -Dbitcoinj.logging=true on your command line.
    /// </summary>
    public static class Utils
    {
        // TODO: Replace this nanocoins business with something better.

        /// <summary>
        /// How many "nanocoins" there are in a BitCoin.
        /// </summary>
        /// <remarks>
        /// A nanocoin is the smallest unit that can be transferred using BitCoin.
        /// The term nanocoin is very misleading, though, because there are only 100 million
        /// of them in a coin (whereas one would expect 1 billion.
        /// </remarks>
        public const ulong Coin = 100000000;

        /// <summary>
        /// How many "nanocoins" there are in 0.01 BitCoins.
        /// </summary>
        /// <remarks>
        /// A nanocoin is the smallest unit that can be transferred using BitCoin.
        /// The term nanocoin is very misleading, though, because there are only 100 million
        /// of them in a coin (whereas one would expect 1 billion).
        /// </remarks>
        public const ulong Cent = 1000000;

        /// <summary>
        /// Convert an amount expressed in the way humans are used to into nanocoins.
        /// </summary>
        public static ulong ToNanoCoins(uint coins, uint cents)
        {
            Debug.Assert(cents < 100);
            var bi = coins*Coin;
            bi += cents*Cent;
            return bi;
        }

        /// <summary>
        /// Convert an amount expressed in the way humans are used to into nanocoins.
        /// </summary>
        /// <remarks>
        /// This takes string in a format understood by <see cref="System.Double.Parse(string)"/>,
        /// for example "0", "1", "0.10", "1.23E3", "1234.5E-5".
        /// </remarks>
        /// <exception cref="ArithmeticException">If you try to specify fractional nanocoins.</exception>
        public static ulong ToNanoCoins(string coins)
        {
            var value = decimal.Parse(coins, NumberStyles.Float)*Coin;
            if (value != Math.Round(value))
            {
                throw new ArithmeticException();
            }
            return checked((ulong) value);
        }

        public static void Uint32ToByteArrayBe(uint val, byte[] @out, int offset)
        {
            @out[offset + 0] = (byte) (val >> 24);
            @out[offset + 1] = (byte) (val >> 16);
            @out[offset + 2] = (byte) (val >> 8);
            @out[offset + 3] = (byte) (val >> 0);
        }

        public static void Uint32ToByteArrayLe(uint val, byte[] @out, int offset)
        {
            @out[offset + 0] = (byte) (val >> 0);
            @out[offset + 1] = (byte) (val >> 8);
            @out[offset + 2] = (byte) (val >> 16);
            @out[offset + 3] = (byte) (val >> 24);
        }

        /// <exception cref="IOException"/>
        public static void Uint32ToByteStreamLe(uint val, Stream stream)
        {
            stream.Write((byte) (val >> 0));
            stream.Write((byte) (val >> 8));
            stream.Write((byte) (val >> 16));
            stream.Write((byte) (val >> 24));
        }

        /// <exception cref="IOException"/>
        public static void Uint64ToByteStreamLe(ulong val, Stream stream)
        {
            var bytes = BitConverter.GetBytes(val);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes);
        }

        /// <summary>
        /// See <see cref="DoubleDigest(byte[], int, int)"/>.
        /// </summary>
        public static byte[] DoubleDigest(byte[] input)
        {
            return DoubleDigest(input, 0, input.Length);
        }

        /// <summary>
        /// Calculates the SHA-256 hash of the given byte range, and then hashes the resulting hash again. This is
        /// standard procedure in BitCoin. The resulting hash is in big endian form.
        /// </summary>
        public static byte[] DoubleDigest(byte[] input, int offset, int length)
        {
            var algorithm = new SHA256Managed();
            var first = algorithm.ComputeHash(input, offset, length);
            return algorithm.ComputeHash(first);
        }

        /// <summary>
        /// Calculates SHA256(SHA256(byte range 1 + byte range 2)).
        /// </summary>
        public static byte[] DoubleDigestTwoBuffers(byte[] input1, int offset1, int length1, byte[] input2, int offset2, int length2)
        {
            var algorithm = new SHA256Managed();
            var buffer = new byte[length1 + length2];
            Array.Copy(input1, offset1, buffer, 0, length1);
            Array.Copy(input2, offset2, buffer, length1, length2);
            var first = algorithm.ComputeHash(buffer, 0, buffer.Length);
            return algorithm.ComputeHash(first);
        }

        /// <summary>
        /// Returns the given byte array hex encoded.
        /// </summary>
        public static string BytesToHexString(byte[] bytes)
        {
            var buf = new StringBuilder(bytes.Length*2);
            foreach (var b in bytes)
            {
                var s = b.ToString("x");
                if (s.Length < 2)
                    buf.Append('0');
                buf.Append(s);
            }
            return buf.ToString();
        }

        /// <summary>
        /// Returns a copy of the given byte array in reverse order.
        /// </summary>
        public static byte[] ReverseBytes(byte[] bytes)
        {
            // We could use the XOR trick here but it's easier to understand if we don't. If we find this is really a
            // performance issue the matter can be revisited.
            var buf = new byte[bytes.Length];
            for (var i = 0; i < bytes.Length; i++)
                buf[i] = bytes[bytes.Length - 1 - i];
            return buf;
        }

        public static uint ReadUint32(byte[] bytes, int offset)
        {
            return (((uint) bytes[offset + 0]) << 0) |
                   (((uint) bytes[offset + 1]) << 8) |
                   (((uint) bytes[offset + 2]) << 16) |
                   (((uint) bytes[offset + 3]) << 24);
        }

        public static uint ReadUint32Be(byte[] bytes, int offset)
        {
            return (((uint) bytes[offset + 0]) << 24) |
                   (((uint) bytes[offset + 1]) << 16) |
                   (((uint) bytes[offset + 2]) << 8) |
                   (((uint) bytes[offset + 3]) << 0);
        }

        public static ushort ReadUint16Be(byte[] bytes, int offset)
        {
            return (ushort) ((bytes[offset] << 8) | bytes[offset + 1]);
        }

        /// <summary>
        /// Calculates RIPEMD160(SHA256(input)). This is used in Address calculations.
        /// </summary>
        public static byte[] Sha256Hash160(byte[] input)
        {
            var sha256 = new SHA256Managed().ComputeHash(input);
            var digest = new RipeMD160Digest();
            digest.BlockUpdate(sha256, 0, sha256.Length);
            var @out = new byte[20];
            digest.DoFinal(@out, 0);
            return @out;
        }

        /// <summary>
        /// Returns the given value in nanocoins as a 0.12 type string.
        /// </summary>
        public static string BitcoinValueToFriendlyString(long value)
        {
            var negative = value < 0;
            if (negative)
                value = -value;
            var coins = value/(long) Coin;
            var cents = value%(long) Coin;
            return string.Format("{0}{1}.{2:00}", negative ? "-" : "", coins, cents/(long) Cent);
        }

        /// <summary>
        /// Returns the given value in nanocoins as a 0.12 type string.
        /// </summary>
        public static string BitcoinValueToFriendlyString(ulong value)
        {
            var coins = value/Coin;
            var cents = value%Coin;
            return string.Format("{0}.{1:00}", coins, cents/Cent);
        }

        /// <summary>
        /// MPI encoded numbers are produced by the OpenSSL BN_bn2mpi function. They consist of
        /// a 4 byte big endian length field, followed by the stated number of bytes representing
        /// the number in big endian format.
        /// </summary>
        private static BigInteger DecodeMpi(byte[] mpi)
        {
            var length = ReadUint32Be(mpi, 0);
            var buf = new byte[length];
            Array.Copy(mpi, 4, buf, 0, (int) length);
            return new BigInteger(1, buf);
        }

        // The representation of nBits uses another home-brew encoding, as a way to represent a large
        // hash value in only 32 bits.
        internal static BigInteger DecodeCompactBits(long compact)
        {
            var size = (byte) (compact >> 24);
            var bytes = new byte[4 + size];
            bytes[3] = size;
            if (size >= 1) bytes[4] = (byte) (compact >> 16);
            if (size >= 2) bytes[5] = (byte) (compact >> 8);
            if (size >= 3) bytes[6] = (byte) (compact >> 0);
            return DecodeMpi(bytes);
        }
    }
}