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
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace BitCoinSharp
{
    /// <summary>
    /// Represents an elliptic curve keypair that we own and can use for signing transactions. Currently,
    /// Bouncy Castle is used. In future this may become an interface with multiple implementations using different crypto
    /// libraries. The class also provides a static method that can verify a signature with just the public key.
    /// </summary>
    [Serializable]
    public class EcKey
    {
        private static readonly ECDomainParameters _ecParams;

        private static readonly SecureRandom _secureRandom;

        static EcKey()
        {
            // All clients must agree on the curve to use by agreement. BitCoin uses secp256k1.
            var @params = SecNamedCurves.GetByName("secp256k1");
            _ecParams = new ECDomainParameters(@params.Curve, @params.G, @params.N, @params.H);
            _secureRandom = new SecureRandom();
        }

        private readonly BigInteger _priv;
        private readonly byte[] _pub;

        [NonSerialized] private byte[] _pubKeyHash;

        /// <summary>
        /// Generates an entirely new keypair.
        /// </summary>
        public EcKey()
        {
            var generator = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(_ecParams, _secureRandom);
            generator.Init(keygenParams);
            var keypair = generator.GenerateKeyPair();
            var privParams = (ECPrivateKeyParameters) keypair.Private;
            var pubParams = (ECPublicKeyParameters) keypair.Public;
            _priv = privParams.D;
            // The public key is an encoded point on the elliptic curve. It has no meaning independent of the curve.
            _pub = pubParams.Q.GetEncoded();
        }

        /// <summary>
        /// Construct an ECKey from an ASN.1 encoded private key. These are produced by OpenSSL and stored by the BitCoin
        /// reference implementation in its wallet.
        /// </summary>
        public static EcKey FromAsn1(byte[] asn1PrivKey)
        {
            return new EcKey(ExtractPrivateKeyFromAsn1(asn1PrivKey));
        }

        /// <summary>
        /// Output this ECKey as an ASN.1 encoded private key, as understood by OpenSSL or used by the BitCoin reference
        /// implementation in its wallet storage format.
        /// </summary>
        public byte[] ToAsn1()
        {
            using (var baos = new MemoryStream(400))
            {
                using (var encoder = new Asn1OutputStream(baos))
                {
                    // ASN1_SEQUENCE(EC_PRIVATEKEY) = {
                    //   ASN1_SIMPLE(EC_PRIVATEKEY, version, LONG),
                    //   ASN1_SIMPLE(EC_PRIVATEKEY, privateKey, ASN1_OCTET_STRING),
                    //   ASN1_EXP_OPT(EC_PRIVATEKEY, parameters, ECPKPARAMETERS, 0),
                    //   ASN1_EXP_OPT(EC_PRIVATEKEY, publicKey, ASN1_BIT_STRING, 1)
                    // } ASN1_SEQUENCE_END(EC_PRIVATEKEY)
                    var seq = new DerSequenceGenerator(encoder);
                    seq.AddObject(new DerInteger(1)); // version
                    seq.AddObject(new DerOctetString(_priv.ToByteArray()));
                    seq.AddObject(new DerTaggedObject(0, SecNamedCurves.GetByName("secp256k1").ToAsn1Object()));
                    seq.AddObject(new DerTaggedObject(1, new DerBitString(PubKey)));
                    seq.Close();
                }
                return baos.ToArray();
            }
        }

        /// <summary>
        /// Creates an ECKey given only the private key. This works because EC public keys are derivable from their
        /// private keys by doing a multiply with the generator value.
        /// </summary>
        public EcKey(BigInteger privKey)
        {
            _priv = privKey;
            _pub = PublicKeyFromPrivate(privKey);
        }

        /// <summary>
        /// Derive the public key by doing a point multiply of G * priv.
        /// </summary>
        private static byte[] PublicKeyFromPrivate(BigInteger privKey)
        {
            return _ecParams.G.Multiply(privKey).GetEncoded();
        }

        /// <summary>
        /// Gets the hash160 form of the public key (as seen in addresses).
        /// </summary>
        public byte[] PubKeyHash
        {
            get { return _pubKeyHash ?? (_pubKeyHash = Utils.Sha256Hash160(_pub)); }
        }

        /// <summary>
        /// Gets the raw public key value. This appears in transaction scriptSigs. Note that this is <b>not</b> the same
        /// as the pubKeyHash/address.
        /// </summary>
        public byte[] PubKey
        {
            get { return _pub; }
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append("pub:").Append(Utils.BytesToHexString(_pub));
            b.Append(" priv:").Append(Utils.BytesToHexString(_priv.ToByteArray()));
            return b.ToString();
        }

        /// <summary>
        /// Returns the address that corresponds to the public part of this ECKey. Note that an address is derived from
        /// the RIPEMD-160 hash of the public key and is not the public key itself (which is too large to be convenient).
        /// </summary>
        public Address ToAddress(NetworkParameters @params)
        {
            var hash160 = Utils.Sha256Hash160(_pub);
            return new Address(@params, hash160);
        }

        /// <summary>
        /// Calculates an ECDSA signature in DER format for the given input hash. Note that the input is expected to be
        /// 32 bytes long.
        /// </summary>
        public byte[] Sign(byte[] input)
        {
            var signer = new ECDsaSigner();
            var privKey = new ECPrivateKeyParameters(_priv, _ecParams);
            signer.Init(true, privKey);
            var sigs = signer.GenerateSignature(input);
            // What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream
            // of the type used by BitCoin we have to encode them using DER encoding, which is just a way to pack the two
            // components into a structure.
            using (var bos = new MemoryStream())
            {
                var seq = new DerSequenceGenerator(bos);
                seq.AddObject(new DerInteger(sigs[0]));
                seq.AddObject(new DerInteger(sigs[1]));
                seq.Close();
                return bos.ToArray();
            }
        }

        /// <summary>
        /// Verifies the given ASN.1 encoded ECDSA signature against a hash using the public key.
        /// </summary>
        /// <param name="data">Hash of the data to verify.</param>
        /// <param name="signature">ASN.1 encoded signature.</param>
        /// <param name="pub">The public key bytes to use.</param>
        public static bool Verify(byte[] data, byte[] signature, byte[] pub)
        {
            var signer = new ECDsaSigner();
            var @params = new ECPublicKeyParameters(_ecParams.Curve.DecodePoint(pub), _ecParams);
            signer.Init(false, @params);
            DerInteger r;
            DerInteger s;
            using (var decoder = new Asn1InputStream(signature))
            {
                var seq = (DerSequence) decoder.ReadObject();
                r = (DerInteger) seq[0];
                s = (DerInteger) seq[1];
            }
            return signer.VerifySignature(data, r.Value, s.Value);
        }

        /// <summary>
        /// Verifies the given ASN.1 encoded ECDSA signature against a hash using the public key.
        /// </summary>
        /// <param name="data">Hash of the data to verify.</param>
        /// <param name="signature">ASN.1 encoded signature.</param>
        public bool Verify(byte[] data, byte[] signature)
        {
            return Verify(data, signature, _pub);
        }

        private static BigInteger ExtractPrivateKeyFromAsn1(byte[] asn1PrivKey)
        {
            // To understand this code, see the definition of the ASN.1 format for EC private keys in the OpenSSL source
            // code in ec_asn1.c:
            //
            // ASN1_SEQUENCE(EC_PRIVATEKEY) = {
            //   ASN1_SIMPLE(EC_PRIVATEKEY, version, LONG),
            //   ASN1_SIMPLE(EC_PRIVATEKEY, privateKey, ASN1_OCTET_STRING),
            //   ASN1_EXP_OPT(EC_PRIVATEKEY, parameters, ECPKPARAMETERS, 0),
            //   ASN1_EXP_OPT(EC_PRIVATEKEY, publicKey, ASN1_BIT_STRING, 1)
            // } ASN1_SEQUENCE_END(EC_PRIVATEKEY)
            //
            DerOctetString key;
            using (var decoder = new Asn1InputStream(asn1PrivKey))
            {
                var seq = (DerSequence) decoder.ReadObject();
                Debug.Assert(seq.Count == 4, "Input does not appear to be an ASN.1 OpenSSL EC private key");
                Debug.Assert(((DerInteger) seq[0]).Value.Equals(BigInteger.One), "Input is of wrong version");
                key = (DerOctetString) seq[1];
            }
            return new BigInteger(1, key.GetOctets());
        }

        /// <summary>
        /// Returns a 32 byte array containing the private key.
        /// </summary>
        public byte[] GetPrivKeyBytes()
        {
            // Getting the bytes out of a BigInteger gives us an extra zero byte on the end (for signedness)
            // or less than 32 bytes (leading zeros).  Coerce to 32 bytes in all cases.
            var bytes = new byte[32];

            var privArray = _priv.ToByteArray();
            var privStart = (privArray.Length == 33) ? 1 : 0;
            var privLength = Math.Min(privArray.Length, 32);
            Array.Copy(privArray, privStart, bytes, 32 - privLength, privLength);

            return bytes;
        }

        /// <summary>
        /// Exports the private key in the form used by the Satoshi client "dumpprivkey" and "importprivkey" commands. Use
        /// the <see cref="DumpedPrivateKey.ToString"/> method to get the string.
        /// </summary>
        /// <param name="params">The network this key is intended for use on.</param>
        /// <returns>Private key bytes as a <see cref="DumpedPrivateKey"/>.</returns>
        public DumpedPrivateKey GetPrivateKeyEncoded(NetworkParameters @params)
        {
            return new DumpedPrivateKey(@params, GetPrivKeyBytes());
        }
    }
}