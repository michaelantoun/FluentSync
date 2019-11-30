using FluentSync.Comparers;
using FluentSync.Comparers.Configurations;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync
{
    /// <summary>
    /// The batch sync agent synchronizes the items based on the batch size.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class BatchSyncAgent<TKey, TItem> : BaseSyncAgent<TItem>, IBatchSyncAgent<TKey, TItem>
    {
        /// <summary>
        /// The configurations of the batch sync agent.
        /// </summary>
        public BatchSyncConfigurations Configurations { get; } = new BatchSyncConfigurations();

        /// <summary>
        /// The comparer agent which compares the source and destination items before syncing them by key.
        /// </summary>
        public IKeyComparerAgent<TKey> ComparerAgent { get; set; }

        /// <summary>
        /// The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.
        /// </summary>
        public CompareItemFunc<TItem> CompareItemFunc { get; set; }

        /// <summary>
        /// The key selector is a function that is executed against the item to return the key.
        /// </summary>
        public Func<TItem, TKey> KeySelector { get; set; }

        /// <summary>
        /// The source sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        public IBatchSyncProvider<TKey, TItem> SourceProvider { get; set; }

        /// <summary>
        /// The destination sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        public IBatchSyncProvider<TKey, TItem> DestinationProvider { get; set; }

        /// <summary>
        /// An action to be called after comparing the source and destination items by keys and before syncing. This action is called one time before syncing batches.
        /// </summary>
        public Action<KeysComparisonResult<TKey>> BeforeSyncingKeysAction { get; set; }

        /// <summary>
        /// An action to be called before deleting a set of items by keys from the source.
        /// </summary>
        public Action<List<TKey>> BeforeDeletingItemsFromSourceAction { get; set; }

        /// <summary>
        /// An action to be called before deleting a set of items by keys from the destination.
        /// </summary>
        public Action<List<TKey>> BeforeDeletingItemsFromDestinationAction { get; set; }

        /// <summary>
        /// Creates a new instance of the batch sync agent.
        /// </summary>
        /// <returns></returns>
        public static BatchSyncAgent<TKey, TItem> Create() => new BatchSyncAgent<TKey, TItem>();

        /// <summary>
        /// Validates the batch sync agent's configurations and properties.
        /// </summary>
        protected override void Validate()
        {
            Configurations.Validate();

            if (CompareItemFunc == null)
                throw new NullReferenceException($"The {nameof(CompareItemFunc)} cannot be null.");

            if (KeySelector == null)
                throw new NullReferenceException($"The {nameof(KeySelector)} cannot be null.");

            if (SourceProvider == null)
                throw new NullReferenceException($"The {nameof(SourceProvider)} cannot be null.");

            if (DestinationProvider == null)
                throw new NullReferenceException($"The {nameof(DestinationProvider)} cannot be null.");
        }

        /// <summary>
        /// Synchronizes the source and destination items.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public async Task SyncAsync(CancellationToken cancellationToken)
        {
            // Validate batch comparer agent too
            if (ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(ComparerAgent)} cannot be null.");

            Validate();

            var keysComparisonResult = await ComparerAgent.CompareAsync(cancellationToken).ConfigureAwait(false);

            await SyncAsync(keysComparisonResult, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronizes the source and destination items by using the provided key comparison result.
        /// </summary>
        /// <param name="keysComparisonResult">The comparison result of the source and destination keys.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public async Task SyncAsync(KeysComparisonResult<TKey> keysComparisonResult, CancellationToken cancellationToken)
        {
            Validate();

            BeforeSyncingKeysAction?.Invoke(keysComparisonResult);

            foreach (var batchSyncOperation in Configurations.BatchSyncListsOrder.Order)
            {
                switch (batchSyncOperation)
                {
                    case BatchSyncListType.ItemsInSourceOnly:
                        if (Configurations.SyncMode.ItemsInSourceOnly != SyncItemOperation.None)
                            await SyncBatchesAsync(Configurations.BatchSize, keysComparisonResult.KeysInSourceOnly, SyncItemsInSourceOnlyBatchAsync, cancellationToken).ConfigureAwait(false);
                        break;
                    case BatchSyncListType.ItemsInDestinationOnly:
                        if (Configurations.SyncMode.ItemsInDestinationOnly != SyncItemOperation.None)
                            await SyncBatchesAsync(Configurations.BatchSize, keysComparisonResult.KeysInDestinationOnly, SyncItemsInDestinationOnlyBatchAsync, cancellationToken).ConfigureAwait(false);
                        break;
                    case BatchSyncListType.Matches:
                        await SyncBatchesAsync(Configurations.BatchSize, keysComparisonResult.Matches, SyncMatchesBatchAsync, cancellationToken).ConfigureAwait(false);
                        break;
                    default:
                        throw new NotSupportedException($"Not supported {nameof(BatchSyncListType)} '{batchSyncOperation.ToString()}'.");
                }
            }
        }

        /// <summary>
        /// Loops through the keys and synchronize them in batches.
        /// </summary>
        /// <param name="batchSize">The size of the batch.</param>
        /// <param name="keys">The list of the keys.</param>
        /// <param name="syncItemsBatchFunc">A method to be called to synchronize a batch.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        private static async Task SyncBatchesAsync(int batchSize, IEnumerable<TKey> keys, Func<List<TKey>, CancellationToken, Task> syncItemsBatchFunc, CancellationToken cancellationToken)
        {
            List<TKey> batchKeys = new List<TKey>();

            foreach (var item in keys)
            {
                batchKeys.Add(item);
                if (batchKeys.Count == batchSize)
                {
                    await syncItemsBatchFunc.Invoke(batchKeys, cancellationToken).ConfigureAwait(false);
                    batchKeys.Clear();
                }
            }

            if (batchKeys.Any())
            {
                await syncItemsBatchFunc.Invoke(batchKeys, cancellationToken).ConfigureAwait(false);
                batchKeys.Clear();
            }
        }

        /// <summary>
        /// Synchronizes the items that exist in the source only.
        /// </summary>
        /// <param name="batchKeys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        private async Task SyncItemsInSourceOnlyBatchAsync(List<TKey> batchKeys, CancellationToken cancellationToken)
        {
            if (!batchKeys.Any())
                return;

            switch (Configurations.SyncMode.ItemsInSourceOnly)
            {
                case SyncItemOperation.None:  // do nothing
                    break;
                case SyncItemOperation.Add:
                    var comparisonResult = new ComparisonResult<TItem>();
                    comparisonResult.ItemsInSourceOnly.AddRange(await SourceProvider.GetAsync(batchKeys, cancellationToken).ConfigureAwait(false));
                    await SyncAsync(comparisonResult, cancellationToken).ConfigureAwait(false);
                    break;
                case SyncItemOperation.Delete:
                    BeforeDeletingItemsFromSourceAction?.Invoke(batchKeys);
                    await SourceProvider.DeleteAsync(batchKeys, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new NotSupportedException($"Not supported source {nameof(SyncItemOperation)} '{Configurations.SyncMode.ItemsInSourceOnly.ToString()}'.");
            }
        }

        /// <summary>
        /// Synchronizes the items that exist in the destination only.
        /// </summary>
        /// <param name="batchKeys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        private async Task SyncItemsInDestinationOnlyBatchAsync(List<TKey> batchKeys, CancellationToken cancellationToken)
        {
            if (!batchKeys.Any())
                return;

            switch (Configurations.SyncMode.ItemsInDestinationOnly)
            {
                case SyncItemOperation.None:  // do nothing
                    break;
                case SyncItemOperation.Add:
                    var comparisonResult = new ComparisonResult<TItem>();
                    comparisonResult.ItemsInDestinationOnly.AddRange(await DestinationProvider.GetAsync(batchKeys, cancellationToken).ConfigureAwait(false));
                    await SyncAsync(comparisonResult, cancellationToken).ConfigureAwait(false);
                    break;
                case SyncItemOperation.Delete:
                    BeforeDeletingItemsFromDestinationAction?.Invoke(batchKeys);
                    await DestinationProvider.DeleteAsync(batchKeys, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new NotSupportedException($"Not supported destination {nameof(SyncItemOperation)} '{Configurations.SyncMode.ItemsInDestinationOnly.ToString()}'.");
            }
        }

        /// <summary>
        /// Synchronizes the items that exist in the source and destination.
        /// </summary>
        /// <param name="batchKeys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        private async Task SyncMatchesBatchAsync(List<TKey> batchKeys, CancellationToken cancellationToken)
        {
            var srcTask = SourceProvider.GetAsync(batchKeys, cancellationToken);
            var dstTask = DestinationProvider.GetAsync(batchKeys, cancellationToken);

            await Task.WhenAll(srcTask, dstTask);

            // Compare
            var comparisonResult = await ComparerAgent<TKey, TItem>.Create()
                .Configure((c) =>
                {
                    c.AllowDuplicateItems = RuleAllowanceType.None;
                    c.AllowDuplicateKeys = RuleAllowanceType.None;
                    c.AllowNullableItems = RuleAllowanceType.None;
                })
                .SetKeySelector(KeySelector)
                .SetCompareItemFunc(CompareItemFunc)
                .SetSourceProvider(srcTask.Result)
                .SetDestinationProvider(dstTask.Result)
                .CompareAsync(cancellationToken).ConfigureAwait(false);

            // Sync
            await SyncAsync(comparisonResult, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a string that represents the batch sync agent.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Configurations)}: {{{Configurations}}}";
        }

        #region BaseSyncAgent<TItem>

        /// <summary>
        /// Gets the sync configurations of the sync agent.
        /// </summary>
        /// <returns></returns>
        protected override ISyncConfigurations GetSyncConfigurations() => Configurations;

        /// <summary>
        /// Gets the sync provider of the source.
        /// </summary>
        /// <returns></returns>
        protected override ISyncBaseProvider<TItem> GetSourceSyncProvider() => SourceProvider;

        /// <summary>
        /// Gets the sync provider of the destination.
        /// </summary>
        /// <returns></returns>
        protected override ISyncBaseProvider<TItem> GetDestinationSyncProvider() => DestinationProvider;

        /// <summary>
        /// Deletes the items from the source.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        protected override async Task DeleteFromSourceAsync(List<TItem> items, CancellationToken cancellationToken)
        {
            if (items == null || !items.Any())
                return;

            await SourceProvider.DeleteAsync(items.Select(x => KeySelector(x)).ToList(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the items from the destination.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        protected override async Task DeleteFromDestinationAsync(List<TItem> items, CancellationToken cancellationToken)
        {
            if (items == null || !items.Any())
                return;

            await DestinationProvider.DeleteAsync(items.Select(x => KeySelector(x)).ToList(), cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
