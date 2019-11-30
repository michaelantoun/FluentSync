using FluentSync.Comparers;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using System;
using System.Collections.Generic;

namespace FluentSync.Sync
{
    /// <summary>
    /// The extension methods of the sync agent.
    /// </summary>
    public static class SyncAgentExtensions
    {
        /// <summary>
        /// Configures the sync agent.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns></returns>
        public static ISyncAgent<TKey, TItem> Configure<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, Action<ISyncConfigurations> configure)
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
        public static ISyncAgent<TKey, TItem> SetComparerAgent<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, IComparerAgent<TKey, TItem> comparerAgent)
        {
            syncAgent.ComparerAgent = comparerAgent;
            return syncAgent;
        }

        /// <summary>
        /// Sets the source sync provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="syncProvider">The source sync provider of the sync agent.</param>
        /// <returns></returns>
        public static ISyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, ISyncProvider<TItem> syncProvider)
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
        /// <param name="syncProvider">The destination sync provider of the sync agent.</param>
        /// <returns></returns>
        public static ISyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, ISyncProvider<TItem> syncProvider)
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
        /// <param name="sourceProvider">The source comparer and sync providers.</param>
        /// <returns></returns>
        public static ISyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, IComparerSyncProvider<TItem> sourceProvider)
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
        /// <param name="destinationProvider">The destination comparer and sync providers.</param>
        /// <returns></returns>
        public static ISyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, IComparerSyncProvider<TItem> destinationProvider)
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
        public static ISyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, IList<TItem> items)
        {
            if (items == null)
                throw new NullReferenceException($"The source {nameof(items)} cannot be null.");
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");

            syncAgent.ComparerAgent.SetSourceProvider(items);
            syncAgent.SourceProvider = new ListSyncProvider<TItem> { Items = items };

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
        public static ISyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, IList<TItem> items)
        {
            if (items == null)
                throw new NullReferenceException($"The destination {nameof(items)} cannot be null.");
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");

            syncAgent.ComparerAgent.SetDestinationProvider(items);
            syncAgent.DestinationProvider = new ListSyncProvider<TItem> { Items = items };

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
        public static ISyncAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, SortedSet<TItem> items)
        {
            if (items == null)
                throw new NullReferenceException($"The source {nameof(items)} cannot be null.");
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");

            syncAgent.ComparerAgent.SetSourceProvider(items);
            syncAgent.SourceProvider = new SortedSetSyncProvider<TItem> { Items = items };

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
        public static ISyncAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, SortedSet<TItem> items)
        {
            if (items == null)
                throw new NullReferenceException($"The destination {nameof(items)} cannot be null.");
            if (syncAgent.ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(syncAgent.ComparerAgent)} must be set first.");

            syncAgent.ComparerAgent.SetDestinationProvider(items);
            syncAgent.DestinationProvider = new SortedSetSyncProvider<TItem> { Items = items };

            return syncAgent;
        }

        /// <summary>
        /// Sets the BeforeSyncingAction.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="syncAgent">The sync agent.</param>
        /// <param name="beforeSyncingAction">An action to be called before syncing the items.</param>
        /// <returns></returns>
        public static ISyncAgent<TKey, TItem> SetBeforeSyncingAction<TKey, TItem>(this ISyncAgent<TKey, TItem> syncAgent, Action<ComparisonResult<TItem>> beforeSyncingAction)
        {
            syncAgent.BeforeSyncingAction = beforeSyncingAction;
            return syncAgent;
        }

    }
}
