namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The sync operation of an item that has a match.
    /// </summary>
    public enum SyncMatchOperation : int
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        None = 0,
        /// <summary>
        /// Updates the source items with the destination items regardless which item is newer.
        /// </summary>
        UpdateSource,
        /// <summary>
        /// Updates the destination items with the source items regardless which item is newer.
        /// </summary>
        UpdateDestination,
        /// <summary>
        /// Updates the old items with the new items in either the source or destination.
        /// </summary>
        UpdateOldItem,
        /// <summary>
        /// Updates the destination items with the source items if they are old.
        /// </summary>
        UpdateOldDestination,
        /// <summary>
        /// Updates the source items with the destination items if they are old.
        /// </summary>
        UpdateOldSource,
    }
}
