namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The order of the sync operations: Insert, Update, and Delete.
    /// </summary>
    public class SyncOperationsOrder
    {
        /// <summary>
        /// The order of the sync operations: Insert, Update, and Delete.
        /// </summary>
        public SyncOperationType[] Order { get; } = new SyncOperationType[3] { SyncOperationType.Delete, SyncOperationType.Update, SyncOperationType.Insert };

        /// <summary>
        /// Checks for duplicates in the order array.
        /// </summary>
        /// <returns></returns>
        public bool HasDuplicates() => Order[0] == Order[1] || Order[0] == Order[2] || Order[1] == Order[2];

        /// <summary>
        /// Returns a string that represents the order of the sync operations: Insert, Update, and Delete.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"First: {Order[0]}, Second: {Order[1]}, Finally: {Order[2]}";
    }
}