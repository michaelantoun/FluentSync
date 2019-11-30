using System;

namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The configurations of the batch sync agent.
    /// </summary>
    public class BatchSyncConfigurations : SyncConfigurations
    {
        private int batchSize = 100;

        /// <summary>
        /// The batch size.
        /// </summary>
        public int BatchSize
        {
            get => batchSize;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Batch size must be greater than 0.");

                batchSize = value;
            }
        }

        /// <summary>
        /// The order of the lists to be synced.
        /// </summary>
        public BatchSyncListsOrder BatchSyncListsOrder { get; } = new BatchSyncListsOrder();

        /// <summary>
        /// Validates the sync configurations.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (BatchSyncListsOrder.HasDuplicates())
                throw new ArgumentException($"Cannot have duplicates in the {nameof(Configurations.BatchSyncListsOrder)} array.");

        }

        /// <summary>
        /// Returns a string that represents the configurations of the batch sync agent.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + $", {nameof(BatchSyncListsOrder)}: {{{BatchSyncListsOrder}}}, {nameof(BatchSize)}: {BatchSize}";
        }
    }
}
