using FluentSync.Comparers;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using System;
using System.Collections.Generic;

namespace FluentSync.Sync
{
    /// <summary>
    /// The extension methods of the batch sync agent.
    /// </summary>
    public static class BatchSyncAgentExtensions
    {
        /// <summary>
        /// Configures the sync agent.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> Configure<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, Action<BatchSyncConfigurations> configure)
        {
            configure?.Invoke(syncAgent.Configurations);
            return syncAgent;
        }

        /// <summary>
        /// Sets the comparer agent of the sync agent.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetComparerAgent<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IKeyComparerAgent<TKey> comparerAgent)
        {
            syncAgent.ComparerAgent = comparerAgent;
            return syncAgent;
        }

        /// <summary>
        /// Sets the KeySelector function.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="keySelector">The key selector is a function that is executed against the item to return the key.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetKeySelector<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, Func<TItem, TKey> keySelector)
        {
            syncAgent.KeySelector = keySelector;
            return syncAgent;
        }

        /// <summary>
        /// Sets the CompareItemFunc.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="compareItemFunc">The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetCompareItemFunc<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, CompareItemFunc<TItem> compareItemFunc)
        {
            syncAgent.CompareItemFunc = compareItemFunc;
            return syncAgent;
        }

        /// <summary>
        /// Sets the BeforeSyncingAction
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="beforeSyncingAction">An action to be called before syncing the items.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetBeforeSyncingAction<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, Action<ComparisonResult<TItem>> beforeSyncingAction)
        {
            syncAgent.BeforeSyncingAction = beforeSyncingAction;
            return syncAgent;
        }

        /// <summary>
        /// Sets BeforeSyncingKeysAction.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="beforeSyncingKeysAction">An action to be called after comparing the source and destination items by keys and before syncing. This action is called one time before syncing batches.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetBeforeSyncingKeysAction<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, Action<KeysComparisonResult<TKey>> beforeSyncingKeysAction)
        {
            syncAgent.BeforeSyncingKeysAction = beforeSyncingKeysAction;
            return syncAgent;
        }

        /// <summary>
        /// Sets BeforeDeletingItemsFromDestinationAction.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="beforeDeletingItemsFromDestinationAction">An action to be called before deleting a set of items by keys from the destination.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetBeforeDeletingItemsFromDestinationAction<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, Action<List<TKey>> beforeDeletingItemsFromDestinationAction)
        {
            syncAgent.BeforeDeletingItemsFromDestinationAction = beforeDeletingItemsFromDestinationAction;
            return syncAgent;
        }

        /// <summary>
        /// Sets BeforeDeletingItemsFromSourceAction.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="beforeDeletingItemsFromSourceAction">An action to be called before deleting a set of items by keys from the source.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetBeforeDeletingItemsFromSourceAction<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, Action<List<TKey>> beforeDeletingItemsFromSourceAction)
        {
            syncAgent.BeforeDeletingItemsFromSourceAction = beforeDeletingItemsFromSourceAction;
            return syncAgent;
        }

        /// <summary>
        /// Sets the source sync provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="syncProvider">The source sync provider which is used for adding, updating, and deleting items.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IBatchSyncProvider<TKey, TItem> syncProvider)
        {
            syncAgent.SourceProvider = syncProvider;

            return syncAgent;
        }

        /// <summary>
        /// Sets the destination sync provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="syncProvider">The destination sync provider which is used for adding, updating, and deleting items.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IBatchSyncProvider<TKey, TItem> syncProvider)
        {
            syncAgent.DestinationProvider = syncProvider;

            return syncAgent;
        }

        /// <summary>
        /// Sets the source comparer and sync providers.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="sourceProvider">The source key comparer and batch sync providers.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IComparerBatchSyncProvider<TKey, TItem> sourceProvider)
        {
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");

            syncAgent.ComparerAgent.SourceProvider = sourceProvider;
            syncAgent.SourceProvider = sourceProvider;

            return syncAgent;
        }

        /// <summary>
        /// Sets the destination comparer and sync providers.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="destinationProvider">The destination key comparer and batch sync providers.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IComparerBatchSyncProvider<TKey, TItem> destinationProvider)
        {
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");

            syncAgent.ComparerAgent.DestinationProvider = destinationProvider;
            syncAgent.DestinationProvider = destinationProvider;

            return syncAgent;
        }

        /// <summary>
        /// Sets the source items.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="items">The source items.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IDictionary<TKey, TItem> items)
        {
            if (items == null)
                throw new NullReferenceException($"The source {nameof(items)} cannot be null.");
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");
            if (syncAgent.KeySelector == null)
                throw new NullReferenceException($"The {nameof(syncAgent.KeySelector)} must be set first.");

            var syncProvider = new DictionaryBatchSyncProvider<TKey, TItem> { Items = items, KeySelector = syncAgent.KeySelector };
            syncAgent.ComparerAgent.SetSourceProvider(syncProvider);
            syncAgent.SourceProvider = syncProvider;

            return syncAgent;
        }

        /// <summary>
        /// Sets the destination items.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="items">The destination items.</param>
        /// <returns></returns>
        public static IBatchSyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this IBatchSyncAgent<TKey, TItem> syncAgent, IDictionary<TKey, TItem> items)
        {
            if (items == null)
                throw new NullReferenceException($"The destination {nameof(items)} cannot be null.");
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");
            if (syncAgent.KeySelector == null)
                throw new NullReferenceException($"The {nameof(syncAgent.KeySelector)} must be set first.");

            var syncProvider = new DictionaryBatchSyncProvider<TKey, TItem> { Items = items, KeySelector = syncAgent.KeySelector };
            syncAgent.ComparerAgent.SetDestinationProvider(syncProvider);
            syncAgent.DestinationProvider = syncProvider;

            return syncAgent;
        }

    }
}
