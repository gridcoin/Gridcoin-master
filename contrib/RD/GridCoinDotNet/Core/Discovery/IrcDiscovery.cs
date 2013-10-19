/*
 * Copyright 2011 John Sample
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace BitCoinSharp.Discovery
{
    /// <summary>
    /// IrcDiscovery provides a way to find network peers by joining a pre-agreed rendevouz point on the LFnet IRC network.
    /// </summary>
    public class IrcDiscovery : IPeerDiscovery
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (IrcDiscovery));

        private readonly string _channel;
        private readonly int _port;
        private readonly string _server;

        /// <summary>
        /// Finds a list of peers by connecting to an IRC network, joining a channel, decoding the nicks and then
        /// disconnecting.
        /// </summary>
        /// <param name="channel">The IRC channel to join, either "#bitcoin" or "#bitcoinTEST" for the production and test networks respectively.</param>
        /// <param name="server">Name or textual IP address of the IRC server to join.</param>
        /// <param name="port">The port of the IRC server to join.</param>
        public IrcDiscovery(string channel, string server = "irc.lfnet.org", int port = 6667)
        {
            _channel = channel;
            _server = server;
            _port = port;
        }

        protected virtual void OnIrcSend(string message)
        {
            if (Send != null)
            {
                Send(this, new IrcDiscoveryEventArgs(message));
            }
        }

        protected virtual void OnIrcReceive(string message)
        {
            if (Receive != null)
            {
                Receive(this, new IrcDiscoveryEventArgs(message));
            }
        }

        public event EventHandler<IrcDiscoveryEventArgs> Send;
        public event EventHandler<IrcDiscoveryEventArgs> Receive;

        /// <summary>
        /// Returns a list of peers that were found in the IRC channel. Note that just because a peer appears in the list
        /// does not mean it is accepting connections.
        /// </summary>
        /// <exception cref="PeerDiscoveryException"/>
        public IEnumerable<EndPoint> GetPeers()
        {
            var addresses = new List<EndPoint>();
            using (var connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    connection.Connect(_server, _port);
                    using (var writer = new StreamWriter(new NetworkStream(connection, FileAccess.Write)))
                    using (var reader = new StreamReader(new NetworkStream(connection, FileAccess.Read)))
                    {
                        // Generate a random nick for the connection. This is chosen to be clearly identifiable as coming from
                        // BitCoinSharp but not match the standard nick format, so full peers don't try and connect to us.
                        var nickRnd = string.Format("bcs{0}", new Random().Next(int.MaxValue));
                        var command = "NICK " + nickRnd;
                        LogAndSend(writer, command);
                        // USER <user> <mode> <unused> <realname> (RFC 2812)
                        command = "USER " + nickRnd + " 8 *: " + nickRnd;
                        LogAndSend(writer, command);
                        writer.Flush();

                        // Wait to be logged in. Worst case we end up blocked until the server PING/PONGs us out.
                        string currLine;
                        while ((currLine = reader.ReadLine()) != null)
                        {
                            OnIrcReceive(currLine);
                            // 004 tells us we are connected
                            // TODO: add common exception conditions (nick already in use, etc..)
                            // these aren't bullet proof checks but they should do for our purposes.
                            if (CheckLineStatus("004", currLine))
                            {
                                break;
                            }
                        }

                        // Join the channel.
                        LogAndSend(writer, "JOIN " + _channel);
                        // List users in channel.
                        LogAndSend(writer, "NAMES " + _channel);
                        writer.Flush();

                        // A list of the users should be returned. Look for code 353 and parse until code 366.
                        while ((currLine = reader.ReadLine()) != null)
                        {
                            OnIrcReceive(currLine);
                            if (CheckLineStatus("353", currLine))
                            {
                                // Line contains users. List follows ":" (second ":" if line starts with ":")
                                var subIndex = 0;
                                if (currLine.StartsWith(":"))
                                {
                                    subIndex = 1;
                                }

                                var spacedList = currLine.Substring(currLine.IndexOf(":", subIndex));
                                addresses.AddRange(ParseUserList(spacedList.Substring(1).Split(' ')));
                            }
                            else if (CheckLineStatus("366", currLine))
                            {
                                // End of user list.
                                break;
                            }
                        }

                        // Quit the server.
                        LogAndSend(writer, "PART " + _channel);
                        LogAndSend(writer, "QUIT");
                        writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    // Throw the original error wrapped in the discovery error.
                    throw new PeerDiscoveryException(e.Message, e);
                }
            }
            return addresses.ToArray();
        }

        private void LogAndSend(TextWriter writer, string command)
        {
            OnIrcSend(command);
            writer.WriteLine(command);
        }

        // Visible for testing.
        internal static IList<EndPoint> ParseUserList(IEnumerable<string> userNames)
        {
            var addresses = new List<EndPoint>();
            foreach (var user in userNames)
            {
                // All BitCoin peers start their nicknames with a 'u' character.
                if (!user.StartsWith("u"))
                {
                    continue;
                }

                // After "u" is stripped from the beginning array contains unsigned chars of:
                // 4 byte IP address, 2 byte port, 4 byte hash check (ipv4)

                byte[] addressBytes;
                try
                {
                    // Strip off the "u" before decoding. Note that it's possible for anyone to join these IRC channels and
                    // so simply beginning with "u" does not imply this is a valid BitCoin encoded address.
                    //
                    // decodeChecked removes the checksum from the returned bytes.
                    addressBytes = Base58.DecodeChecked(user.Substring(1));
                }
                catch (AddressFormatException)
                {
                    _log.WarnFormat("IRC nick does not parse as base58: {0}", user);
                    continue;
                }

                // TODO: Handle IPv6 if one day the official client uses it. It may be that IRC discovery never does.
                if (addressBytes.Length != 6)
                {
                    continue;
                }

                var ipBytes = new[] {addressBytes[0], addressBytes[1], addressBytes[2], addressBytes[3]};
                var port = Utils.ReadUint16Be(addressBytes, 4);

                var ip = new IPAddress(ipBytes);

                var address = new IPEndPoint(ip, port);
                addresses.Add(address);
            }

            return addresses;
        }

        private static bool CheckLineStatus(string statusCode, string response)
        {
            // Lines can either start with the status code or an optional :<source>
            //
            // All the testing shows the servers for this purpose use :<source> but plan for either.
            // TODO: Consider whether regex would be worth it here.
            if (response.StartsWith(":"))
            {
                // Look for first space.
                var startIndex = response.IndexOf(" ") + 1;
                // Next part should be status code.
                return response.IndexOf(statusCode + " ", startIndex) == startIndex;
            }
            return response.StartsWith(statusCode + " ");
        }
    }

    public class IrcDiscoveryEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public IrcDiscoveryEventArgs(string message)
        {
            Message = message;
        }
    }
}