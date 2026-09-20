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
        /// <returns>true if the same value appears more than once in the order array; otherwise, false.</returns>
        public bool HasDuplicates() => Order[0] == Order[1] || Order[0] == Order[2] || Order[1] == Order[2];

        /// <summary>
        /// Returns a string that represents the order of the lists to be synced.
        /// </summary>
        /// <returns>A string that represents the order of the lists to be synced.</returns>
        public override string ToString() => $"First: {Order[0]}, Second: {Order[1]}, Finally: {Order[2]}";
    }
}