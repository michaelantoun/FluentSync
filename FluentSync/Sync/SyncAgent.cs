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
    /// The sync agent which synchronizes the source and destination items all at once.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class SyncAgent<TKey, TItem> : BaseSyncAgent<TItem>, ISyncAgent<TKey, TItem>
    {
        /// <summary>
        /// The configurations of the sync agent.
        /// </summary>
        public SyncConfigurations Configurations { get; } = new SyncConfigurations();

        /// <summary>
        /// The comparer agent which compares the source and destination items before syncing them.
        /// </summary>
        public IComparerAgent<TKey, TItem> ComparerAgent { get; set; }

        /// <summary>
        /// The source sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        public ISyncProvider<TItem> SourceProvider { get; set; }

        /// <summary>
        /// The destination sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        public ISyncProvider<TItem> DestinationProvider { get; set; }

        /// <summary>
        /// Creates a new instance of the sync agent.
        /// </summary>
        /// <returns></returns>
        public static SyncAgent<TKey, TItem> Create() => new SyncAgent<TKey, TItem>();

        /// <summary>
        /// Validates the sync agent's configurations and properties.
        /// </summary>
        protected override void Validate()
        {
            Configurations.Validate();

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
            // Validate comparer agent too
            if (ComparerAgent == null)
                throw new NullReferenceException($"The {nameof(ComparerAgent)} cannot be null.");

            Validate();

            var comparisonResult = await ComparerAgent.CompareAsync(cancellationToken).ConfigureAwait(false);

            await SyncAsync(comparisonResult, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a string that represents the sync agent.
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
        protected override Task DeleteFromSourceAsync(List<TItem> items, CancellationToken cancellationToken) => SourceProvider.DeleteAsync(items, cancellationToken);

        /// <summary>
        /// Delete the items from the destination.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        protected override Task DeleteFromDestinationAsync(List<TItem> items, CancellationToken cancellationToken) => DestinationProvider.DeleteAsync(items, cancellationToken);

        #endregion
    }

    /// <summary>
    /// The sync agent which synchronizes the source and destination items all at once.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class SyncAgent<TItem> : SyncAgent<TItem, TItem> { }
}
