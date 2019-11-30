using System.Collections.Generic;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The comparison result of the source and destination items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class ComparisonResult<TItem>
    {
        /// <summary>
        /// The items that exist only in the source.
        /// </summary>
        public List<TItem> ItemsInSourceOnly { get; } = new List<TItem>();

        /// <summary>
        /// The items that exist only in the destination.
        /// </summary>
        public List<TItem> ItemsInDestinationOnly { get; } = new List<TItem>();

        /// <summary>
        /// The items that exist in the source and destination.
        /// </summary>
        public List<MatchComparisonResult<TItem>> Matches { get; } = new List<MatchComparisonResult<TItem>>();

        /// <summary>
        /// Returns a string that represents the comparison result of the source and destination items.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(ItemsInSourceOnly)}: {ItemsInSourceOnly.Count}, {nameof(ItemsInDestinationOnly)}: {ItemsInDestinationOnly.Count}, {nameof(Matches)}: {Matches.Count}";
        }
    }
}
