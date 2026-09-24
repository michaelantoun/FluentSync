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
            set1.Count.ShouldBe(set2.Count);

            // Verify that the 2 sets are different
            (set1 == set2).ShouldBeFalse();

            set1.All(s => set2.Contains(s)).ShouldBeTrue();
        }

        /// <summary>
        /// Verify the 2 dictionaries are equivalent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic1"></param>
        /// <param name="dic2"></param>
        internal static void VerifyDictionariesAreEquivalent<TKey, TValue>(IDictionary<TKey, TValue> dic1, IDictionary<TKey, TValue> dic2)
        {
            dic1.Count.ShouldBe(dic2.Count);

            // Verify that the 2 dictionaries are different
            (dic1 == dic2).ShouldBeFalse();

            foreach (var pair1 in dic1)
            {
                dic2.TryGetValue(pair1.Key, out var value2).ShouldBeTrue();
                pair1.Value.Should().BeEquivalentTo(value2);
            }
        }
    }
}
