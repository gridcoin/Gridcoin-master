#region License

/*
* Copyright (C) 2002-2009 the original author or authors.
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
using BitCoinSharp.Threading.Execution;

namespace BitCoinSharp.Threading
{
    /// <summary> 
    /// A task that returns a result and may throw an exception.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementors define a single method with no arguments called
    /// <see cref="Call()"/>.
    /// </para>
    /// <para>
    /// The <c>ICallable</c> interface is similar to <see cref="IRunnable"/>, 
    /// in that both are designed for classes whose
    /// instances are potentially executed by another thread. A
    /// <c>IRunnable</c>, however, does not return a result.
    /// </para>
    /// <para>
    /// The <see cref="Executors"/> class contains utility methods to
    /// convert from other common forms to <c>ICallable</c> classes.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">
    /// The result type of method <see cref="Call()"/>.
    /// </typeparam>
    /// <seealso cref="Func{TResult}"/>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    internal interface ICallable<T> //JDK_1_6
    {
        /// <summary>
        /// Computes a result, or throws an exception if unable to do so.
        /// </summary>
        /// <returns>Computed result.</returns>
        T Call();
    }
}