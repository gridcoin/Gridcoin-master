#region License

/*
 * Copyright (C) 2002-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace BitCoinSharp.Threading.Helpers
{
    /// <summary> 
    /// Simple linked list queue used in FIFOSemaphore.
    /// Methods are not locked; they depend on synch of callers.
    /// NOTE: this class is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    [Serializable]
    internal class FIFOWaitQueue : IWaitQueue //BACKPORT_3_1
    {
        [NonSerialized] protected WaitNode _head;
        [NonSerialized] protected WaitNode _tail;

        public int Length
        {
            get
            {
                var count = 0;
                var node = _head;
                while (node != null)
                {
                    if (node.IsWaiting) count++;
                    node = node.NextWaitNode;
                }
                return count;
            }
        }

        public ICollection<Thread> WaitingThreads
        {
            get
            {
                IList<Thread> list = new List<Thread>();
                var node = _head;
                while (node != null)
                {
                    if (node.IsWaiting) list.Add(node.Owner);
                    node = node.NextWaitNode;
                }
                return list;
            }
        }

        public bool HasNodes
        {
            get { return _head != null; }
        }

        public void Enqueue(WaitNode w)
        {
            if (_tail == null)
                _head = _tail = w;
            else
            {
                _tail.NextWaitNode = w;
                _tail = w;
            }
        }

        public WaitNode Dequeue()
        {
            if (_head == null) return null;

            var w = _head;
            _head = w.NextWaitNode;
            if (_head == null) _tail = null;
            w.NextWaitNode = null;
            return w;
        }

        // In backport 3.1 but not used.
        //public void PutBack(WaitNode w)
        //{
        //    w.NextWaitNode = _head;
        //    _head = w;
        //    if (_tail == null)
        //        _tail = w;
        //}

        public bool IsWaiting(Thread thread)
        {
            if (thread == null) throw new ArgumentNullException("thread");
            for (var node = _head; node != null; node = node.NextWaitNode)
            {
                if (node.IsWaiting && node.Owner == thread) return true;
            }
            return false;
        }
    }
}