using System;

namespace BitCoinSharp.Threading
{
    /// <summary>
    /// A never instantiable class that can be used as generic type parameter
    /// to indicate that the type is irrelevant and ignored.
    /// </summary>
    internal sealed class Void // NET_ONLY
    {
        private Void()
        {
            throw new InvalidOperationException();
        }
    }
}