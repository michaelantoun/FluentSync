using System;

namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The configurations of the sync agent.
    /// </summary>
    public class SyncConfigurations : ISyncConfigurations
    {
        /// <summary>
        /// The sync mode that determines which items will be inserted/updated/deleted in the source/destination.
        /// </summary>
        public SyncMode SyncMode { get; } = new SyncMode();

        /// <summary>
        /// The order of the sync operations: Insert, Update, and Delete.
        /// </summary>
        public SyncOperationsOrder SyncOperationsOrder { get; } = new SyncOperationsOrder();

        /// <summary>
        /// Validates the sync configurations.
        /// </summary>
        public virtual void Validate()
        {
            if (SyncOperationsOrder.HasDuplicates())
                throw new ArgumentException($"Cannot have duplicates in the {nameof(Configurations.SyncOperationsOrder)} array.");
        }

        /// <summary>
        /// Returns a string that represents the configurations of the sync agent.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(SyncMode)}: {{{SyncMode}}}, {nameof(SyncOperationsOrder)}: {{{SyncOperationsOrder}}}";
        }
    }
}
