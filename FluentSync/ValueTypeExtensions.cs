using System;

namespace FluentSync
{
    /// <summary>
    /// The extension methods of the value types.
    /// </summary>
    public static class ValueTypeExtensions
    {
        /// <summary>
        /// Compares the null-able x object with the null-able y object.
        /// </summary>
        /// <typeparam name="T">The type of the x and y objects.</typeparam>
        /// <param name="x">The x object.</param>
        /// <param name="y">The y object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.</returns>
        public static int CompareTo<T>(this T? x, T? y) where T : struct, IComparable<T>
        {
            if (x == null)
                return y == null ? 0 : -1;
            else // x != null
                return y == null ? 1 : x.Value.CompareTo(y.Value);
        }
    }
}
