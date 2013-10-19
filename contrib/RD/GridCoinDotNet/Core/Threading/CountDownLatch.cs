#region License

/*
 * Copyright (C) 2002-2005 the original author or authors.
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

namespace BitCoinSharp.Threading
{
    /// <summary> 
    /// A synchronization aid that allows one or more threads to wait until
    /// a set of operations being performed in other threads completes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="CountDownLatch"/> is initialized with a given <b>count</b>.
    /// The <see cref="Await()"/> and <see cref="Await(TimeSpan)"/> methods 
    /// block until the current <see cref="Count"/> reaches zero due to 
    /// invocations of the <see cref="CountDown()"/> method, after which all 
    /// waiting threads are released and any subsequent invocations of 
    /// <see cref="Await()"/> or <see cref="Await(TimeSpan)"/> return 
    /// immediately. This is a one-shot phenomenon -- the count cannot be reset.
    /// </para>
    /// <para>
    /// A <see cref="CountDownLatch"/> is a versatile synchronization tool
    /// and can be used for a number of purposes. A <see cref="CountDownLatch"/> 
    /// initialized with a count of one serves as a simple on/off latch, or 
    /// gate: all threads invoking <see cref="Await()"/> or <see cref="Await(TimeSpan)"/>
    /// wait at the gate until it is opened by a thread invoking <see cref="CountDown()"/>.
    /// A <see cref="CountDownLatch"/> initialized to <b>N</b> can be used to 
    /// make one thread wait until <b>N</b> threads have completed some action, 
    /// or some action has been completed <b>N</b> times.
    /// </para>
    /// <para>
    /// A useful property of a <see cref="CountDownLatch"/> is that it doesn't 
    /// require that threads calling <see cref="CountDown()"/> wait for
    /// the count to reach zero before proceeding, it simply prevents any
    /// thread from proceeding past an <see cref="Await()"/> or 
    /// <see cref="Await(TimeSpan)"/> until all threads could pass.
    /// </para>
    /// <para>
    /// <b>Memory consistency effects</b>: Actions in a thread prior to calling
    /// <see cref="CountDown"/> <i>happen-before</i> actions following a 
    /// successful return from a corresponding <see cref="Await()"/> in another 
    /// thread.
    /// </para>
    /// <example>
    /// <b>Sample usage:</b> 
    /// <br/>
    /// Here is a pair of classes in which a group of worker threads use two 
    /// countdown latches:
    /// <list type="bullet">
    /// <item>
    /// The first is a start signal that prevents any worker from proceeding
    /// until the driver is ready for them to proceed.
    /// </item>
    /// <item>
    /// The second is a completion signal that allows the driver to wait
    /// until all workers have completed.
    /// </item>
    /// </list>
    /// 
    /// <code>
    /// internal class Driver { // ...
    ///        void Main() {
    ///            CountDownLatch startSignal = new CountDownLatch(1);
    ///            CountDownLatch doneSignal = new CountDownLatch(N);
    /// 
    ///            for (int i = 0; i &lt; N; ++i)
    ///                new Thread(new Worker(startSignal, doneSignal).Run).Start();
    /// 
    ///         doSomethingElse();            // don't let run yet
    ///            startSignal.CountDown();      // let all threads proceed
    ///         doSomethingElse();
    ///         doneSignal.Await();           // wait for all to finish
    ///     }
    /// }
    /// 
    /// internal class Worker {
    ///        private CountDownLatch startSignal;
    ///     private CountDownLatch doneSignal;
    ///     Worker(CountDownLatch startSignal, CountDownLatch doneSignal) {
    ///         this.startSignal = startSignal;
    ///         this.doneSignal = doneSignal;
    ///     }
    ///     public void Run() {
    ///         try {
    ///             startSignal.Await();
    ///             DoWork();
    ///             doneSignal.CountDown();
    ///         } catch (ThreadInterruptedException ex) {} // return;
    ///     }
    /// 
    ///     void DoWork() { ... }
    /// }
    /// 
    /// </code>
    /// Another typical usage would be to divide a problem into N parts,
    /// describe each part with a worker that executes that portion and counts 
    /// down on the latch, and queue all the <see cref="IRunnable"/>s to an
    /// <see cref="IExecutor"/>. When all sub-parts are complete, the 
    /// coordinating thread will be able to pass through await.
    /// 
    /// <code>
    /// internal class Driver2 { // ...
    ///        void Main() {
    ///            CountDownLatch doneSignal = new CountDownLatch(N);
    ///            Executor e = ...
    /// 
    ///         for (int i = 0; i &lt; N; ++i) // create and start threads
    ///                 e.execute(new WorkerRunnable(doneSignal, i));
    /// 
    ///            doneSignal.await();           // wait for all to finish
    ///        }
    /// }
    /// 
    /// internal class WorkerRunnable : IRunnable {
    ///        private CountDownLatch doneSignal;
    ///     private int i;
    ///     WorkerRunnable(CountDownLatch doneSignal, int i) {
    ///         this.doneSignal = doneSignal;
    ///         this.i = i;
    ///     }
    ///     public void Run() {
    ///         try {
    ///             DoWork(i);
    ///             doneSignal.CountDown();
    ///         } catch (ThreadInterruptedException ex) {} // return;
    ///     }
    ///     
    ///        void DoWork() { ... }
    /// }
    /// 
    /// </code>
    /// </example>
    /// </remarks>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    internal class CountDownLatch //BACKPORT_3_1
    {
        private int _count;

        /// <summary> 
        /// Returns the current count.
        /// </summary>
        /// <remarks>
        /// This method is typically used for debugging and testing purposes.
        /// </remarks>
        /// <returns>the current count.</returns>
        public long Count
        {
            get { return _count; }
        }

        /// <summary> 
        /// Constructs a <see cref="CountDownLatch"/> initialized with the given
        /// <paramref name="count"/>.
        /// </summary>
        /// <param name="count">the number of times <see cref="CountDown"/> must 
        /// be invoked before threads can pass through <see cref="Await()"/>.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <paramref name="count"/> is less than 0.
        /// </exception>
        public CountDownLatch(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(
                    "count", count, "Count must be greater than 0.");
            _count = count;
        }

        /// <summary> 
        /// Causes the current thread to wait until the latch has counted down 
        /// to zero, unless <see cref="Thread.Interrupt()"/> is called on the 
        /// thread.
        /// </summary>    
        /// <remarks>
        /// <para>
        /// If the current <see cref="Count"/> is zero then this method returns 
        /// immediately.
        /// </para>
        /// <para>
        /// If the current <see cref="Count"/> is greater than zero then the 
        /// current thread becomes disabled for thread scheduling purposes and 
        /// lies dormant until the count reaches zero due to invocations of the
        /// <see cref="CountDown()"/> method or some other thread calls 
        /// <see cref="Thread.Interrupt()"/> on the current thread.
        /// </para>
        /// </remarks>
        /// <exception cref="ThreadInterruptedException">
        /// If the current thread has its interrupted status set on entry to
        /// this method or is interrupted while waiting.
        /// </exception>
        public void Await()
        {
            lock (this)
            {
                while (_count > 0)
                    Monitor.Wait(this);
            }
        }

        /// <summary> 
        /// Causes the current thread to wait until the latch has counted down 
        /// to zero, unless <see cref="Thread.Interrupt()"/> is called on the 
        /// thread or the specified <paramref name="duration"/> elapses.
        /// </summary>    
        /// <remarks> 
        /// <para>
        /// If the current <see cref="Count"/>  is zero then this method
        /// returns immediately.
        /// </para>
        /// <para>
        /// If the current <see cref="Count"/> is greater than zero then the 
        /// current thread becomes disabled for thread scheduling purposes and 
        /// lies dormant until the count reaches zero due to invocations of the
        /// <see cref="CountDown()"/> method or some other thread calls 
        /// <see cref="Thread.Interrupt()"/> on the current thread.
        /// </para>
        /// <para>
        /// A <see cref="ThreadInterruptedException"/> is thrown if the thread 
        /// is interrupted.
        /// </para>
        /// <para>
        /// If the specified <paramref name="duration"/> elapses then the value
        /// <c>false</c> is returned. If the time is less than or equal
        /// to zero, the method will not wait at all.
        /// </para>
        /// </remarks>
        /// <param name="duration">The maximum time to wait.</param>
        /// <returns>
        /// <c>true</c> if the count reached zero and <c>false</c>
        /// if the waiting time elapsed before the count reached zero.
        /// </returns>
        /// <exception cref="ThreadInterruptedException">
        /// If the current thread is interrupted.
        /// </exception>
        public bool Await(TimeSpan duration)
        {
            var deadline = DateTime.UtcNow.Add(duration);
            lock (this)
            {
                if (_count <= 0)
                    return true;
                else if (duration.Ticks <= 0)
                    return false;
                else
                {
                    for (;;)
                    {
                        Monitor.Wait(this, duration);
                        if (_count <= 0)
                            return true;
                        else
                        {
                            duration = deadline.Subtract(DateTime.UtcNow);
                            if (duration.Ticks <= 0)
                                return false;
                        }
                    }
                }
            }
        }

        /// <summary> 
        /// Decrements the count of the latch, releasing all waiting threads if
        /// the count reaches zero.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the current <see cref="Count"/> is greater than zero then it is 
        /// decremented. If the new count is zero then all waiting threads are 
        /// re-enabled for thread scheduling purposes. 
        /// </para>
        /// <para>
        /// If the current <see cref="Count"/> equals zero then nothing happens.
        /// </para>
        /// </remarks>
        public void CountDown()
        {
            lock (this)
            {
                if (_count == 0)
                    return;
                if (--_count == 0)
                    Monitor.PulseAll(this);
            }
        }

        /// <summary> 
        /// Returns a string identifying this latch, as well as its state.
        /// </summary>
        /// <remarks>
        /// The state, in brackets, includes the string &quot;Count =&quot; 
        /// followed by the current count.
        /// </remarks>
        /// <returns>
        /// A string identifying this latch, as well as its state.
        /// </returns>
        public override String ToString()
        {
            return base.ToString() + "[Count = " + Count + "]";
        }
    }
}