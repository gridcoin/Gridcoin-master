using System;

namespace Spring.Utility
{
    internal static class Error
    {
        internal static ArrayTypeMismatchException NewArrayTypeMismatchException(Exception innerException)
        {
            return new ArrayTypeMismatchException(
                "Attempted to access an element as a type incompatible with the array.",
                innerException);
        }
    }
}