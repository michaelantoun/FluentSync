using System.Collections.Generic;

namespace FluentSync.Sync
{
    /// <summary>
    /// The sync context contains the items that will be inserted/updated/deleted in source/destination.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class SyncContext<TItem>
    {
        /// <summary>
        /// The items that will be inserted in the source.
        /// </summary>
        public List<TItem> ItemsToBeInsertedInSource { get; } = new List<TItem>();

        /// <summary>
        /// The items that will be deleted from the source.
        /// </summary>
        public List<TItem> ItemsToBeDeletedFromSource { get; } = new List<TItem>();

        /// <summary>
        /// The items that will be inserted in the destination.
        /// </summary>
        public List<TItem> ItemsToBeInsertedInDestination { get; } = new List<TItem>();

        /// <summary>
        /// The items that will be deleted from the destination.
        /// </summary>
        public List<TItem> ItemsToBeDeletedFromDestination { get; } = new List<TItem>();

        /// <summary>
        /// The items that will be updated in the source.
        /// </summary>
        public List<MatchValuePair<TItem>> ItemsToBeUpdatedInSource { get; } = new List<MatchValuePair<TItem>>();

        /// <summary>
        /// The items that will be updated in the destination.
        /// </summary>
        public List<MatchValuePair<TItem>> ItemsToBeUpdatedInDestination { get; } = new List<MatchValuePair<TItem>>();

        /// <summary>
        /// Returns a string that represents the SyncContext.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(ItemsToBeInsertedInSource)}: {ItemsToBeInsertedInSource.Count}, {nameof(ItemsToBeDeletedFromSource)}: {ItemsToBeDeletedFromSource.Count}, {nameof(ItemsToBeUpdatedInSource)}: {ItemsToBeUpdatedInSource.Count}"
                + $"{nameof(ItemsToBeInsertedInDestination)}: {ItemsToBeInsertedInDestination.Count}, {nameof(ItemsToBeDeletedFromDestination)}: {ItemsToBeDeletedFromDestination.Count}, {nameof(ItemsToBeUpdatedInDestination)}: {ItemsToBeUpdatedInDestination.Count}";
        }
    }
}
