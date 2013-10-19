using System.Collections;
using System.Collections.Generic;

namespace BitCoinSharp.Threading
{
    internal class SynchronizedHashSet<T> : ICollection<T>
    {
        private readonly HashSet<T> _inner = new HashSet<T>();

        #region ICollection Members

        public void Add(T item)
        {
            lock (_inner)
            {
                _inner.Add(item);
            }
        }

        public void Clear()
        {
            lock (_inner)
            {
                _inner.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_inner)
            {
                return _inner.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_inner)
            {
                _inner.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_inner)
            {
                return _inner.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (_inner)
                {
                    return _inner.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator<T> GetEnumerator()
        {
            lock (_inner)
            {
                return _inner.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}