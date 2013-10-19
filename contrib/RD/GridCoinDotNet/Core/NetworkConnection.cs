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
using System.IO;
using System.Net;
using System.Net.Sockets;
using BitCoinSharp.Common;
using log4net;

namespace BitCoinSharp
{
    /// <summary>
    /// A NetworkConnection handles talking to a remote BitCoin peer at a low level. It understands how to read and write
    /// messages off the network, but doesn't asynchronously communicate with the peer or handle the higher level details
    /// of the protocol. After constructing a NetworkConnection, use a <see cref="Peer"/> to hand off communication to a
    /// background thread.
    /// </summary>
    /// <remarks>
    /// Construction is blocking whilst the protocol version is negotiated.
    /// </remarks>
    public class NetworkConnection : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (NetworkConnection));

        private Socket _socket;
        private Stream _out;
        private Stream _in;
        // The IP address to which we are connecting.
        private readonly IPAddress _remoteIp;
        private readonly NetworkParameters _params;
        private readonly VersionMessage _versionMessage;

        private readonly BitcoinSerializer _serializer;

        public NetworkConnection()
        {
        }

        /// <summary>
        /// Connect to the given IP address using the port specified as part of the network parameters. Once construction
        /// is complete a functioning network channel is set up and running.
        /// </summary>
        /// <param name="peerAddress">IP address to connect to. IPv6 is not currently supported by BitCoin. If port is not positive the default port from params is used.</param>
        /// <param name="params">Defines which network to connect to and details of the protocol.</param>
        /// <param name="bestHeight">How many blocks are in our best chain</param>
        /// <param name="connectTimeout">Timeout in milliseconds when initially connecting to peer</param>
        /// <exception cref="IOException">If there is a network related failure.</exception>
        /// <exception cref="ProtocolException">If the version negotiation failed.</exception>
        public NetworkConnection(PeerAddress peerAddress, NetworkParameters @params, uint bestHeight, int connectTimeout)
        {
            _params = @params;
            _remoteIp = peerAddress.Addr;

            var port = (peerAddress.Port > 0) ? peerAddress.Port : @params.Port;

            var address = new IPEndPoint(_remoteIp, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(address);
            _socket.SendTimeout = _socket.ReceiveTimeout = connectTimeout;

            _out = new NetworkStream(_socket, FileAccess.Write);
            _in = new NetworkStream(_socket, FileAccess.Read);

            // the version message never uses check-summing. Update check-summing property after version is read.
            _serializer = new BitcoinSerializer(@params, false);

            // Announce ourselves. This has to come first to connect to clients beyond v0.30.20.2 which wait to hear
            // from us until they send their version message back.
            WriteMessage(new VersionMessage(@params, bestHeight));
            // When connecting, the remote peer sends us a version message with various bits of
            // useful data in it. We need to know the peer protocol version before we can talk to it.
            _versionMessage = (VersionMessage) ReadMessage();
            // Now it's our turn ...
            // Send an ACK message stating we accept the peers protocol version.
            WriteMessage(new VersionAck());
            // And get one back ...
            ReadMessage();
            // Switch to the new protocol version.
            var peerVersion = _versionMessage.ClientVersion;
            _log.InfoFormat("Connected to peer: version={0}, subVer='{1}', services=0x{2:X}, time={3}, blocks={4}",
                            peerVersion,
                            _versionMessage.SubVer,
                            _versionMessage.LocalServices,
                            UnixTime.FromUnixTime(_versionMessage.Time),
                            _versionMessage.BestHeight
                );
            // BitCoinSharp is a client mode implementation. That means there's not much point in us talking to other client
            // mode nodes because we can't download the data from them we need to find/verify transactions.
            if (!_versionMessage.HasBlockChain())
            {
                // Shut down the socket
                try
                {
                    Shutdown();
                }
                catch (IOException)
                {
                    // ignore exceptions while aborting
                }
                throw new ProtocolException("Peer does not have a copy of the block chain.");
            }
            // newer clients use check-summing
            _serializer.UseChecksumming(peerVersion >= 209);
            // Handshake is done!
        }

        /// <exception cref="IOException"/>
        /// <exception cref="ProtocolException"/>
        public NetworkConnection(IPAddress inetAddress, NetworkParameters @params, uint bestHeight, int connectTimeout)
            : this(new PeerAddress(inetAddress), @params, bestHeight, connectTimeout)
        {
        }

        /// <summary>
        /// Sends a "ping" message to the remote node. The protocol doesn't presently use this feature much.
        /// </summary>
        /// <exception cref="IOException"/>
        public void Ping()
        {
            WriteMessage(new Ping());
        }

        /// <summary>
        /// Shuts down the network socket. Note that there's no way to wait for a socket to be fully flushed out to the
        /// wire, so if you call this immediately after sending a message it might not get sent.
        /// </summary>
        /// <exception cref="IOException"/>
        public virtual void Shutdown()
        {
            _socket.Disconnect(false);
            _socket.Close();
        }

        public override string ToString()
        {
            return "[" + _remoteIp + "]:" + _params.Port + " (" + (_socket.Connected ? "connected" : "disconnected") + ")";
        }

        /// <summary>
        /// Reads a network message from the wire, blocking until the message is fully received.
        /// </summary>
        /// <returns>An instance of a Message subclass</returns>
        /// <exception cref="ProtocolException">If the message is badly formatted, failed checksum or there was a TCP failure.</exception>
        /// <exception cref="IOException"/>
        public virtual Message ReadMessage()
        {
            return _serializer.Deserialize(_in);
        }

        /// <summary>
        /// Writes the given message out over the network using the protocol tag. For a Transaction
        /// this should be "tx" for example. It's safe to call this from multiple threads simultaneously,
        /// the actual writing will be serialized.
        /// </summary>
        /// <exception cref="IOException"/>
        public virtual void WriteMessage(Message message)
        {
            lock (_out)
            {
                _serializer.Serialize(message, _out);
            }
        }

        /// <summary>
        /// Returns the version message received from the other end of the connection during the handshake.
        /// </summary>
        public virtual VersionMessage VersionMessage
        {
            get { return _versionMessage; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_in != null)
            {
                _in.Dispose();
                _in = null;
            }
            if (_out != null)
            {
                _out.Dispose();
                _out = null;
            }
            if (_socket != null)
            {
                ((IDisposable) _socket).Dispose();
                _socket = null;
            }
        }

        #endregion
    }
}