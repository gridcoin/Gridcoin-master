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

namespace BitCoinSharp.Threading.AtomicTypes
{
    /// <summary>
    /// Provide atomic access to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the instance to be updated atomically.</typeparam>
    /// <author>Kenneth Xu</author>
    internal interface IAtomic<T> //NET_ONLY
    {
        /// <summary> 
        /// Gets and sets the current value.
        /// </summary>
        T Value { get; set; }

        /// <summary> 
        /// Eventually sets to the given value.
        /// </summary>
        /// <param name="newValue">
        /// the new value
        /// </param>
        void LazySet(T newValue);

        /// <summary> 
        /// Atomically sets the value to the <paramref name="newValue"/>
        /// if the current value equals the <paramref name="expectedValue"/>.
        /// </summary>
        /// <param name="expectedValue">
        /// The expected value
        /// </param>
        /// <param name="newValue">
        /// The new value to use of the current value equals the expected value.
        /// </param>
        /// <returns> 
        /// <c>true</c> if the current value equaled the expected value, <c>false</c> otherwise.
        /// </returns>
        bool CompareAndSet(T expectedValue, T newValue);

        /// <summary> 
        /// Atomically sets the value to the <paramref name="newValue"/>
        /// if the current value equals the <paramref name="expectedValue"/>.
        /// May fail spuriously.
        /// </summary>
        /// <param name="expectedValue">
        /// The expected value
        /// </param>
        /// <param name="newValue">
        /// The new value to use of the current value equals the expected value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current value equaled the expected value, <c>false</c> otherwise.
        /// </returns>
        bool WeakCompareAndSet(T expectedValue, T newValue);

        /// <summary> 
        /// Atomically sets to the given value and returns the previous value.
        /// </summary>
        /// <param name="newValue">
        /// The new value for the instance.
        /// </param>
        /// <returns> 
        /// the previous value of the instance.
        /// </returns>
        T Exchange(T newValue);

        /// <summary> 
        /// Returns the String representation of the current value.
        /// </summary>
        /// <returns> 
        /// The String representation of the current value.
        /// </returns>
        string ToString();
    }
}