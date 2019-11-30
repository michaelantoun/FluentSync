using System.Collections.Generic;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The comparison result of the source and destination keys.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class KeysComparisonResult<TKey>
    {
        /// <summary>
        /// The keys that exist only in the source.
        /// </summary>
        public SortedSet<TKey> KeysInSourceOnly { get; } = new SortedSet<TKey>();

        /// <summary>
        /// The keys that exist only in the destination.
        /// </summary>
        public SortedSet<TKey> KeysInDestinationOnly { get; } = new SortedSet<TKey>();

        /// <summary>
        /// The keys that exist in the source and destination.
        /// </summary>
        public SortedSet<TKey> Matches { get; } = new SortedSet<TKey>();

        /// <summary>
        /// Returns a string that represents the comparison result of the source and destination keys.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(KeysInSourceOnly)}: {KeysInSourceOnly.Count}, {nameof(KeysInDestinationOnly)}: {KeysInDestinationOnly.Count}, {nameof(Matches)}: {Matches.Count}";
        }
    }
}
