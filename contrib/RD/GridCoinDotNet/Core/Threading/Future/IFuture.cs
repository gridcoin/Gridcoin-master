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

namespace BitCoinSharp.Threading.Future
{
    /// <summary>
    /// A <see cref="IFuture{T}"/> represents the result of an asynchronous
    /// computation. 
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Methods are provided to check if the computation is
    /// complete, to wait for its completion, and to retrieve the result of
    /// the computation. The result can only be retrieved using method
    /// <see cref="GetResult()"/> when the computation has completed, blocking if
    /// necessary until it is ready. Cancellation is performed by the
    /// <see cref="ICancellable.Cancel()"/> method. Additional methods are provided to
    /// determine if the task completed normally or was cancelled. Once a
    /// computation has completed, the computation cannot be cancelled.
    /// If you would like to use a <c>IFuture</c> for the sake
    /// of cancellability but not provide a usable result, you can
    /// declare types of the form <c>IFuture&lt;object&gt;</c> and
    /// return <c>null</c> as a result of the underlying task.
    /// </para>
    /// <example>
    /// Sample Usage (Note that the following classes are all
    /// made-up.) 
    /// <code language="c#">
    /// interface IArchiveSearcher { string Search(string target); }
    /// class App {
    ///   IExecutorService executor = ...
    ///   IArchiveSearcher searcher = ...
    ///   void ShowSearch(string target) {
    ///     IFuture&lt;string&gt; future
    ///       = executor.Submit(delegate {
    ///             return searcher.Search(target);
    ///         });
    ///     DisplayOtherThings(); // do other things while searching
    ///     try {
    ///       DisplayText(future.get()); // use future
    ///     } catch (ExecutionException ex) { Cleanup(); return; }
    ///   }
    /// }
    /// </code>
    /// </example>
    /// The <see cref="FutureTask{T}"/> class is an implementation of 
    /// <see cref="IFuture{T}"/> that implements <see cref="IRunnable"/>, 
    /// and so may be executed by an <see cref="IExecutor"/>.
    /// <example>
    /// For example, the above construction with <c>Submit</c> could be replaced by:
    /// <code language="c#">
    ///     IFutureTask&lt;string&gt; future =
    ///       new FutureTask&lt;string&gt;(delegate {
    ///           return searcher.Search(target);
    ///       });
    ///     executor.Execute(future);
    /// </code>
    /// </example>
    /// <para>
    /// Memory consistency effects: Actions taken by the asynchronous 
    /// computation <i>happen-before</i> actions following the corresponding 
    /// <see cref="GetResult()"/>in another thread.
    /// </para>
    /// </remarks>
    /// <seealso cref="FutureTask{T}"/>
    /// <seealso cref="BitCoinSharp.Threading.IExecutor"/>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    internal interface IFuture<T> : ICancellable //JDK_1_6
    {
        /// <summary>
        /// Waits for computation to complete, then returns its result. 
        /// </summary>
        /// <remarks> 
        /// Waits if necessary for the computation to complete, and then
        /// retrieves its result.
        /// </remarks>
        /// <returns>the computed result</returns>
        /// <exception cref="CancellationException">if the computation was cancelled.</exception>
        /// <exception cref="ExecutionException">if the computation threw an exception.</exception>
        /// <exception cref="System.Threading.ThreadInterruptedException">if the current thread was interrupted while waiting.</exception>
        T GetResult();

        /// <summary>
        /// Waits for the given time span, then returns its result.
        /// </summary>
        /// <remarks> 
        /// Waits, if necessary, for at most the <paramref name="durationToWait"/> for the computation
        /// to complete, and then retrieves its result, if available.
        /// </remarks>
        /// <param name="durationToWait">the <see cref="System.TimeSpan"/> to wait.</param>
        /// <returns>the computed result</returns>
        /// <exception cref="CancellationException">if the computation was cancelled.</exception>
        /// <exception cref="ExecutionException">if the computation threw an exception.</exception>
        /// <exception cref="System.Threading.ThreadInterruptedException">if the current thread was interrupted while waiting.</exception>
        /// <exception cref="TimeoutException">if the computation threw an exception.</exception>
        T GetResult(TimeSpan durationToWait);
    }
}