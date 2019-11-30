namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The sync operation of an item that does not have a match.
    /// </summary>
    public enum SyncItemOperation : int
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        None = 0,
        /// <summary>
        /// Adds items to other list.
        /// </summary>
        Add,
        /// <summary>
        /// Deletes items from the current list.
        /// </summary>
        Delete
    }
}
