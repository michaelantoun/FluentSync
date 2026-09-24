using System.Text.Json;

namespace FluentSync.Tests.Internals
{
    internal static class ShouldlyExtensions
    {
        /// <summary>
        /// Verify the 2 collections contain equivalent items, regardless of their order.
        /// </summary>
        /// <remarks>
        /// Items are compared member by member using Shouldly's <c>ShouldBeEquivalentTo</c>,
        /// so the item types don't need to override <see cref="object.Equals(object)"/>.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        internal static void ShouldBeEquivalentToIgnoringOrder<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            if (!TryMatchIgnoringOrder(actual, expected, out var failure))
                throw new ShouldAssertException(failure);
        }

        /// <summary>
        /// Verify the 2 collections don't contain equivalent items, regardless of their order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        internal static void ShouldNotBeEquivalentToIgnoringOrder<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            if (TryMatchIgnoringOrder(actual, expected, out _))
                throw new ShouldAssertException($"Expected the collection not to be equivalent to {Format(expected)}, but it was.");
        }

        private static bool TryMatchIgnoringOrder<T>(IEnumerable<T> actual, IEnumerable<T> expected, out string failure)
        {
            actual.ShouldNotBeNull();
            ArgumentNullException.ThrowIfNull(expected);

            var actualItems = actual.ToList();
            var expectedItems = expected.ToList();

            if (actualItems.Count != expectedItems.Count)
            {
                failure = $"Expected {expectedItems.Count} items {Format(expectedItems)}, but found {actualItems.Count} items {Format(actualItems)}.";
                return false;
            }

            var unmatchedItems = new List<T>(actualItems);

            foreach (var expectedItem in expectedItems)
            {
                var index = unmatchedItems.FindIndex(item => IsEquivalent(item, expectedItem));

                if (index < 0)
                {
                    failure = $"Expected an item equivalent to {Format(expectedItem)}, but none was found in {Format(actualItems)}.";
                    return false;
                }

                unmatchedItems.RemoveAt(index);
            }

            failure = null;
            return true;
        }

        private static bool IsEquivalent<T>(T actual, T expected)
        {
            try
            {
                actual.ShouldBeEquivalentTo(expected);
                return true;
            }
            catch (ShouldAssertException)
            {
                return false;
            }
        }

        private static string Format(object value) => JsonSerializer.Serialize(value);
    }
}
