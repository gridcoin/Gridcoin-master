using System;
using BitCoinSharp.Threading.Future;

namespace BitCoinSharp.Threading.Execution
{
    /// <summary> 
    /// A service that decouples the production of new asynchronous tasks from 
    /// the consumption of the results of completed tasks. 
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Producers submit tasks for execution. Consumers take completed tasks 
    /// and process their results in the order they complete. A 
    /// <see cref="ICompletionService{T}"/> can for example be used to manage 
    /// asynchronous IO, in which tasks that perform reads are submitted in one 
    /// part of a program or system, and then acted upon in a different part 
    /// of the program when the reads complete, possibly in a different order 
    /// than they were requested.
    /// </para>
    /// <para>
    /// Typically, a <see cref="ICompletionService{T}"/> relies on a separate 
    /// <see cref="IExecutor"/> to actually execute the tasks, in which case the
    /// <see cref="ICompletionService{T}"/> only manages an internal completion
    /// queue. The <see cref="ExecutorCompletionService{T}"/> class provides an
    /// implementation of this approach.
    /// </para>
    /// <para>
    /// Memory consistency effects: Actions in a thread prior to submitting a 
    /// task to a <see cref="ICompletionService{T}"/> <i>happen-before</i> 
    /// actions taken by that task, which in turn <i>happen-before</i> actions 
    /// following a successful return from the corresponding <see cref="Take"/>.
    /// </para>
    /// </remarks>
    internal interface ICompletionService<T> //JDK_1_6
    {
        /// <summary> 
        ///    Submits a value-returning task for execution and returns an instance 
        /// of <see cref="IFuture{T}"/> representing the pending results of the 
        /// task. The future's <see cref="IFuture{T}.GetResult()"/> method will 
        /// return the callable's result upon successful completion.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you would like to immediately block waiting for a callable, you 
        /// can use constructions of the form
        /// <code language="c#">
        ///   result = exec.Submit(aCallable).GetResult();
        /// </code>
        /// </para>
        /// </remarks>
        /// <param name="callable">The task to submit.</param>
        /// <returns>
        /// A <see cref="IFuture{T}"/> representing pending completion of the 
        /// task.
        /// </returns>
        /// <exception cref="RejectedExecutionException">
        /// If the task cannot be accepted for execution.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the command is null.
        /// </exception>
        IFuture<T> Submit(ICallable<T> callable);

        /// <summary> 
        ///    Submits a value-returning task for execution and returns an instance 
        /// of <see cref="IFuture{T}"/> representing the pending results of the 
        /// task. The future's <see cref="IFuture{T}.GetResult()"/> method will 
        /// return the callable's result upon successful completion.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you would like to immediately block waiting for a callable, you 
        /// can use constructions of the form
        /// <code language="c#">
        ///   result = exec.Submit(aCallable).GetResult();
        /// </code>
        /// </para>
        /// </remarks>
        /// <param name="call">The task to submit.</param>
        /// <returns>
        /// A <see cref="IFuture{T}"/> representing pending completion of the 
        /// task.
        /// </returns>
        /// <exception cref="RejectedExecutionException">
        /// If the task cannot be accepted for execution.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the command is null.
        /// </exception>
        IFuture<T> Submit(Func<T> call);

        /// <summary> 
        /// Submits a <see cref="IRunnable"/> task for execution and returns a 
        /// <see cref="IFuture{T}"/> representing that task. Upon completion, 
        /// this task may be taken or polled.
        /// </summary>
        /// <param name="runnable">The task to submit.</param>
        /// <param name="result">
        /// The result to return upon successful completion.
        /// </param>
        /// <returns>
        /// A <see cref="IFuture{T}"/> representing pending completion of the 
        /// task, and whose <see cref="IFuture{T}.GetResult()"/> method will 
        /// return the given result value upon completion.
        /// </returns>
        /// <exception cref="RejectedExecutionException">
        /// If the task cannot be accepted for execution.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the command is null.
        /// </exception>
        IFuture<T> Submit(IRunnable runnable, T result);

        /// <summary> 
        /// Submits a <see cref="Action"/> task for execution and returns a 
        /// <see cref="IFuture{T}"/> representing that task. Upon completion, 
        /// this task may be taken or polled.
        /// </summary>
        /// <param name="action">The task to submit.</param>
        /// <param name="result">
        /// The result to return upon successful completion.
        /// </param>
        /// <returns>
        /// A <see cref="IFuture{T}"/> representing pending completion of the 
        /// task, and whose <see cref="IFuture{T}.GetResult()"/> method will 
        /// return the given result value upon completion.
        /// </returns>
        /// <exception cref="RejectedExecutionException">
        /// If the task cannot be accepted for execution.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the command is null.
        /// </exception>
        IFuture<T> Submit(Action action, T result);

        /// <summary> 
        /// Submits a <see cref="IRunnable"/> task for execution and returns 
        /// a <see cref="IFuture{T}"/> representing that task. The future's
        /// <see cref="IFuture{T}.GetResult()"/> will return <c>default(T)</c>
        /// upon successful completion.
        /// </summary>
        /// <param name="runnable">The task to submit.</param>
        /// <returns>
        /// A <see cref="IFuture{T}"/> representing pending completion of the 
        /// task, and whose <see cref="IFuture{T}.GetResult()"/> method will 
        /// return the given result value upon completion.
        /// </returns>
        /// <exception cref="RejectedExecutionException">
        /// If the task cannot be accepted for execution.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the command is null.
        /// </exception>
        IFuture<T> Submit(IRunnable runnable);

        /// <summary> 
        /// Submits a <see cref="Action"/> task for execution and returns a 
        /// <see cref="IFuture{T}"/> representing that task. The future's
        /// <see cref="IFuture{T}.GetResult()"/> will return <c>default(T)</c>
        /// upon successful completion.
        /// </summary>
        /// <param name="action">The task to submit.</param>
        /// <returns>
        /// A <see cref="IFuture{T}"/> representing pending completion of the 
        /// task, and whose <see cref="IFuture{T}.GetResult()"/> method will 
        /// return the given result value upon completion.
        /// </returns>
        /// <exception cref="RejectedExecutionException">
        /// If the task cannot be accepted for execution.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the command is null.
        /// </exception>
        IFuture<T> Submit(Action action);

        /// <summary> 
        /// Retrieves and removes the <see cref="IFuture{T}"/> representing 
        /// the next completed task, waiting if none are yet present.
        /// </summary>
        /// <returns>
        /// The <see cref="IFuture{T}"/> representing the next completed task.
        /// </returns>
        IFuture<T> Take();

        /// <summary> 
        /// Retrieves and removes the <see cref="IFuture{T}"/> representing 
        /// the next completed task or <c>null</c> if none are present.
        /// </summary>
        /// <returns>
        /// The <see cref="IFuture{T}"/> representing the next completed task, 
        /// or <c>null</c> if none are present.
        /// </returns>
        IFuture<T> Poll();

        /// <summary> 
        /// Retrieves and removes the <see cref="IFuture{T}"/> representing the 
        /// next completed task, waiting, if necessary, up to the specified 
        /// duration if none are yet present.
        /// </summary>
        /// <param name="durationToWait">
        /// Duration to wait if no completed task is present yet.
        /// </param>
        /// <returns> 
        /// the <see cref="IFuture{T}"/> representing the next completed task or
        /// <c>null</c> if the specified waiting time elapses before one
        /// is present.
        /// </returns>
        IFuture<T> Poll(TimeSpan durationToWait);
    }
}