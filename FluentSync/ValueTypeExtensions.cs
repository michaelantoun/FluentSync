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
        /// <returns>A value that indicates the relative order of the objects being compared:
        /// less than zero when <paramref name="x"/> precedes <paramref name="y"/> in the sort order,
        /// zero when they occur in the same position, and greater than zero when <paramref name="x"/> follows
        /// <paramref name="y"/>. A null object precedes a non-null one, and two nulls are equal.</returns>
        public static int CompareTo<T>(this T? x, T? y) where T : struct, IComparable<T>
        {
            if (x == null)
                return y == null ? 0 : -1;
            else // x != null
                return y == null ? 1 : x.Value.CompareTo(y.Value);
        }
    }
}
