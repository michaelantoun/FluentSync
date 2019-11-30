namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The sync lists.
    /// </summary>
    public enum BatchSyncListType : int
    {
        /// <summary>
        /// The list of items that exist only in the source.
        /// </summary>
        ItemsInSourceOnly = 1,
        /// <summary>
        /// The list of items that exist only in the destination.
        /// </summary>
        ItemsInDestinationOnly,
        /// <summary>
        /// The list of items that exist in the source and destination.
        /// </summary>
        Matches
    }
}
