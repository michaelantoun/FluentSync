namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The order of the lists to be synced.
    /// </summary>
    public class BatchSyncListsOrder
    {
        /// <summary>
        /// The order of the lists to be synced.
        /// </summary>
        public BatchSyncListType[] Order { get; } = new BatchSyncListType[3] { BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInDestinationOnly, BatchSyncListType.Matches };

        /// <summary>
        /// Checks for duplicates in the order array.
        /// </summary>
        /// <returns></returns>
        public bool HasDuplicates() => Order[0] == Order[1] || Order[0] == Order[2] || Order[1] == Order[2];

        /// <summary>
        /// Returns a string that represents the order of the lists to be synced.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"First: {Order[0]}, Second: {Order[1]}, Finally: {Order[2]}";
    }
}