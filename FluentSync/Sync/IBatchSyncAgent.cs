using FluentSync.Comparers;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync
{
    /// <summary>
    /// The batch sync agent synchronizes the items based on the batch size.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface IBatchSyncAgent<TKey, TItem> : IBaseSyncAgent<TItem>
    {
        /// <summary>
        /// An action to be called before deleting a set of items by keys from the destination.
        /// </summary>
        Action<List<TKey>> BeforeDeletingItemsFromDestinationAction { get; set; }

        /// <summary>
        /// An action to be called before deleting a set of items by keys from the source.
        /// </summary>
        Action<List<TKey>> BeforeDeletingItemsFromSourceAction { get; set; }

        /// <summary>
        /// An action to be called after comparing the source and destination items by keys and before syncing. This action is called one time before syncing batches.
        /// </summary>
        Action<KeysComparisonResult<TKey>> BeforeSyncingKeysAction { get; set; }

        /// <summary>
        /// The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.
        /// </summary>
        CompareItemFunc<TItem> CompareItemFunc { get; set; }

        /// <summary>
        /// The configurations of the batch sync agent.
        /// </summary>
        BatchSyncConfigurations Configurations { get; }

        /// <summary>
        /// The destination sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        IBatchSyncProvider<TKey, TItem> DestinationProvider { get; set; }

        /// <summary>
        /// The comparer agent which compares the source and destination items before syncing them by key.
        /// </summary>
        IKeyComparerAgent<TKey> ComparerAgent { get; set; }

        /// <summary>
        /// The key selector is a function that is executed against the item to return the key.
        /// </summary>
        Func<TItem, TKey> KeySelector { get; set; }

        /// <summary>
        /// The source sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        IBatchSyncProvider<TKey, TItem> SourceProvider { get; set; }

        /// <summary>
        /// Synchronizes the source and destination items.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task SyncAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Synchronizes the source and destination items by using the provided key comparison result.
        /// </summary>
        /// <param name="keysComparisonResult">The comparison result of the source and destination keys.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task SyncAsync(KeysComparisonResult<TKey> keysComparisonResult, CancellationToken cancellationToken);
    }
}