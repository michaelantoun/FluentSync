namespace FluentSync.Comparers
{
    /// <summary>
    /// The comparison result of the source and destination items.
    /// </summary>
    public enum MatchComparisonResultType : int
    {
        /// <summary>
        /// The source item is the same as the destination item.
        /// </summary>
        Same = 0,
        /// <summary>
        /// The source item is newer than the destination item.
        /// </summary>
        NewerSource,
        /// <summary>
        /// The destination item is newer than the source item.
        /// </summary>
        NewerDestination,
        /// <summary>
        /// There is a conflict, cannot determine which item is newer.
        /// </summary>
        Conflict
    }
}
