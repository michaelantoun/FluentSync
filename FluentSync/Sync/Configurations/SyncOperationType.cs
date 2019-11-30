namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The type of the sync operation.
    /// </summary>
    public enum SyncOperationType : int
    {
        /// <summary>
        /// The insert operation.
        /// </summary>
        Insert = 1,
        /// <summary>
        /// The update operation.
        /// </summary>
        Update,
        /// <summary>
        /// The delete operation.
        /// </summary>
        Delete
    }
}
