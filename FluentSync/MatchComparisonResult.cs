using FluentSync.Comparers;

namespace FluentSync
{
    /// <summary>
    /// The comparison result of the two items that have the same key.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class MatchComparisonResult<TItem>
    {
        /// <summary>
        /// The source item.
        /// </summary>
        public TItem Source { get; set; }

        /// <summary>
        /// The destination item.
        /// </summary>
        public TItem Destination { get; set; }

        /// <summary>
        /// The comparison result of the two items.
        /// </summary>
        public MatchComparisonResultType ComparisonResult { get; set; }

        /// <summary>
        /// Returns a string that represents the MatchComparisonResult.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(ComparisonResult)}: {ComparisonResult}";
        }
    }
}
