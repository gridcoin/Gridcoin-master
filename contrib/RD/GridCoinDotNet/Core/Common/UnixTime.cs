// Original source: http://oauth-dot-net.googlecode.com/svn/trunk/1.0a/OAuth.Net.Common/UnixTime.cs

// Copyright (c) 2008 Madgex
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// OAuth.net uses the Common Service Locator interface, released under the MS-PL
// license. See "CommonServiceLocator License.txt" in the Licenses folder.
//
// The examples and test cases use the Windsor Container from the Castle Project
// and Common Service Locator Windsor adaptor, released under the Apache License,
// Version 2.0. See "Castle Project License.txt" in the Licenses folder.
//
// XRDS-Simple.net uses the HTMLAgility Pack. See "HTML Agility Pack License.txt"
// in the Licenses folder.
//
// Authors: Bruce Boughton, Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System;
using System.Diagnostics.CodeAnalysis;

namespace BitCoinSharp.Common
{
    /// <summary>
    /// Class for converting to and from Unix (POSIX) time. Unix time is the number of seconds
    /// since the Unix epoch, 1970-1-1 0:0:00.0.
    /// </summary>
    internal static class UnixTime
    {
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a DateTime to unix time. Unix time is the number of seconds
        /// between 1970-1-1 0:0:0.0 (unix epoch) and the time (UTC).
        /// </summary>
        /// <param name="time">The date time to convert to unix time</param>
        /// <returns>The number of seconds between Unix epoch and the input time</returns>
        public static ulong ToUnixTime(DateTime time)
        {
            return (ulong) (time.ToUniversalTime() - _unixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Converts a long representation of a unix time into a DateTime. Unix time is
        /// the number of seconds between 1970-1-1 0:0:0.0 (unix epoch) and the time (UTC).
        /// </summary>
        /// <param name="unixTime">The number of seconds since Unix epoch (must be >= 0)</param>
        /// <returns>A DateTime object representing the unix time</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "unix", Justification = "UNIX is a domain term")]
        public static DateTime FromUnixTime(ulong unixTime)
        {
            return _unixEpoch.AddSeconds(unixTime);
        }
    }
}