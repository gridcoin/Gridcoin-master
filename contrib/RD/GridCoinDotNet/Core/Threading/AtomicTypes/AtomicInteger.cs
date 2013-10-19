#region License

/*
 * Copyright 2002-2008 the original author or authors.
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
using System.Threading;

#pragma warning disable 420

namespace BitCoinSharp.Threading.AtomicTypes
{
    /// <summary> 
    /// An <see cref="int"/> value that may be updated atomically.
    /// An <see cref="AtomicInteger"/> is used in applications such as atomically
    /// incremented counters, and cannot be used as a replacement for an
    /// <see cref="int"/>. 
    /// <p/>
    /// Based on the on the back port of JCP JSR-166.
    /// </summary>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Andreas Doehring (.NET)</author>
    /// <author>Kenneth Xu (Interlocked)</author>
    [Serializable]
    internal class AtomicInteger : IAtomic<int> //JDK_1_6
    {
        private volatile int _integerValue;

        /// <summary> 
        /// Creates a new <see cref="AtomicInteger"/> with a value of <paramref name="initialValue"/>.
        /// </summary>
        /// <param name="initialValue">
        /// The initial value
        /// </param>
        public AtomicInteger(int initialValue)
        {
            _integerValue = initialValue;
        }

        /// <summary> 
        /// Creates a new <see cref="AtomicInteger"/> with initial value 0.
        /// </summary>
        public AtomicInteger()
        {
        }

        /// <summary> 
        /// Gets and sets the current value.
        /// </summary>
        /// <returns>
        /// The current value
        /// </returns>
        public int Value
        {
            get { return _integerValue; }
            set { _integerValue = value; }
        }

        /// <summary> 
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns> 
        /// The previous value
        /// </returns>
        public int ReturnValueAndIncrement()
        {
            return Interlocked.Increment(ref _integerValue) - 1;
        }

        /// <summary> 
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns> 
        /// The previous value
        /// </returns>
        public int ReturnValueAndDecrement()
        {
            return Interlocked.Decrement(ref _integerValue) + 1;
        }

        /// <summary> 
        /// Eventually sets to the given value.
        /// </summary>
        /// <param name="newValue">
        /// The new value
        /// </param>
        public void LazySet(int newValue)
        {
            _integerValue = newValue;
        }

        /// <summary> 
        /// Atomically sets value to <paramref name="newValue"/> and returns the old value.
        /// </summary>
        /// <param name="newValue">
        /// The new value
        /// </param>
        /// <returns> 
        /// The previous value
        /// </returns>
        public int Exchange(int newValue)
        {
            return Interlocked.Exchange(ref _integerValue, newValue);
        }

        /// <summary> 
        /// Atomically sets the value to <paramref name="newValue"/>
        /// if the current value == <paramref name="expectedValue"/>
        /// </summary>
        /// <param name="expectedValue">
        /// The expected value
        /// </param>
        /// <param name="newValue">
        /// The new value
        /// </param>
        /// <returns> <c>true</c> if successful. <c>false</c> return indicates that
        /// the actual value was not equal to the expected value.
        /// </returns>
        public bool CompareAndSet(int expectedValue, int newValue)
        {
            return expectedValue == Interlocked.CompareExchange(
                ref _integerValue, newValue, expectedValue);
        }

        /// <summary> 
        /// Atomically sets the value to <paramref name="newValue"/>
        /// if the current value == <paramref name="expectedValue"/>
        /// </summary>
        /// <param name="expectedValue">
        /// The expected value
        /// </param>
        /// <param name="newValue">
        /// The new value
        /// </param>
        /// <returns> <c>true</c> if successful. <c>false</c> return indicates that
        /// the actual value was not equal to the expected value.
        /// </returns>
        public virtual bool WeakCompareAndSet(int expectedValue, int newValue)
        {
            return expectedValue == Interlocked.CompareExchange(
                ref _integerValue, newValue, expectedValue);
        }

        /// <summary> 
        /// Atomically adds <paramref name="deltaValue"/> to the current value.
        /// </summary>
        /// <param name="deltaValue">
        /// The value to add
        /// </param>
        /// <returns> 
        /// The previous value
        /// </returns>
        public int AddDeltaAndReturnPreviousValue(int deltaValue)
        {
            return Interlocked.Add(ref _integerValue, deltaValue) - deltaValue;
        }

        /// <summary> 
        /// Atomically adds <paramref name="deltaValue"/> to the current value.
        /// </summary>
        /// <param name="deltaValue">
        /// The value to add
        /// </param>
        /// <returns> 
        /// The updated value
        /// </returns>
        public int AddDeltaAndReturnNewValue(int deltaValue)
        {
            return Interlocked.Add(ref _integerValue, deltaValue);
        }

        /// <summary> 
        /// Atomically increments the current value by one.
        /// </summary>
        /// <returns> 
        /// The updated value
        /// </returns>
        public int IncrementValueAndReturn()
        {
            return Interlocked.Increment(ref _integerValue);
        }

        /// <summary> 
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns> 
        /// The updated value
        /// </returns>
        public int DecrementValueAndReturn()
        {
            return Interlocked.Decrement(ref _integerValue);
        }

        /// <summary> 
        /// Returns the String representation of the current value.
        /// </summary>
        /// <returns> 
        /// The String representation of the current value.
        /// </returns>
        public override String ToString()
        {
            return _integerValue.ToString();
        }

        /// <summary>
        /// Implicit converts <see cref="AtomicInteger"/> to int.
        /// </summary>
        /// <param name="atomicInteger">
        /// Instance of <see cref="AtomicInteger"/>.
        /// </param>
        /// <returns>
        /// The converted int value of <paramref name="atomicInteger"/>.
        /// </returns>
        public static implicit operator int(AtomicInteger atomicInteger)
        {
            return atomicInteger.Value;
        }
    }
}

#pragma warning restore 420