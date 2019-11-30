namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The configurations of the sync agent.
    /// </summary>
    public interface ISyncConfigurations
    {
        /// <summary>
        /// The sync mode that determines which items will be inserted/updated/deleted in the source/destination.
        /// </summary>
        SyncMode SyncMode { get; }

        /// <summary>
        /// The order of the sync operations: Insert, Update, and Delete.
        /// </summary>
        SyncOperationsOrder SyncOperationsOrder { get; }
    }
}