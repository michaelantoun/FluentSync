using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace FluentSync.Tests.Internals
{
    internal static class AssertionHelper
    {
        /// <summary>
        /// Verify the 2 sorted sets are equivalent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        internal static void VerifySortedSetsAreEquivalent<T>(SortedSet<T> set1, SortedSet<T> set2)
        {
            set1.Count.Should().Be(set2.Count);

            // Verify that the 2 sets are different
            (set1 == set2).Should().BeFalse();

            set1.All(s => set2.Contains(s)).Should().BeTrue();
        }

        /// <summary>
        /// Verify the 2 dictionaries are equivalent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic1"></param>
        /// <param name="dic2"></param>
        internal static void VerifyDictionariesAreEquivalent<TKey, TValue>(IDictionary<TKey, TValue> dic1, IDictionary<TKey, TValue> dic2)
        {
            dic1.Count.Should().Be(dic2.Count);

            // Verify that the 2 dictionaries are different
            (dic1 == dic2).Should().BeFalse();

            foreach (var pair1 in dic1)
            {
                dic2.TryGetValue(pair1.Key, out var value2).Should().BeTrue();
                pair1.Value.Should().BeEquivalentTo(value2);
            }
        }
    }
}
