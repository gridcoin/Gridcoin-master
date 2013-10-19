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

namespace BitCoinSharp.Threading.Future
{
    /// <summary> 
    /// A <see cref="IFuture{T}"/> that is a <see cref="IRunnable"/>. 
    /// </summary>
    /// <remarks>
    /// Successful execution of the <see cref="IRunnable.Run()"/> method causes 
    /// completion of the <see cref="IFuture{T}"/> and allows access to its results.
    /// </remarks>
    /// <seealso cref="FutureTask{T}"/>
    /// <seealso cref="IExecutor"/>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    internal interface IRunnableFuture<T> : IRunnable, IFuture<T> //JDK_1_6
    {
    }
}