using FluentSync.Comparers;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync
{
    /// <summary>
    /// The sync agent which synchronizes the source and destination items all at once.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface ISyncAgent<TKey, TItem>
    {
        /// <summary>
        /// An action to be called before syncing the items.
        /// </summary>
        Action<ComparisonResult<TItem>> BeforeSyncingAction { get; set; }

        /// <summary>
        /// The comparer agent which compares the source and destination items before syncing them.
        /// </summary>
        IComparerAgent<TKey, TItem> ComparerAgent { get; set; }

        /// <summary>
        /// The configurations of the sync agent.
        /// </summary>
        SyncConfigurations Configurations { get; }

        /// <summary>
        /// The destination sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        ISyncProvider<TItem> DestinationProvider { get; set; }

        /// <summary>
        /// The source sync provider which is used for adding, updating, and deleting items.
        /// </summary>
        ISyncProvider<TItem> SourceProvider { get; set; }

        /// <summary>
        /// Synchronizes the source and destination items.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task SyncAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Synchronizes the source and destination items based on the comparison result.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items which is generated from the comparer agent.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task SyncAsync(ComparisonResult<TItem> comparisonResult, CancellationToken cancellationToken);
    }
}