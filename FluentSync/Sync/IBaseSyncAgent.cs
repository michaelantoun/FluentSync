using FluentSync.Comparers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync
{
    /// <summary>
    /// The base sync agent which has the common logic for the sync agent and batch sync agent.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IBaseSyncAgent<TItem>
    {
        /// <summary>
        /// An action to be called before syncing the items.
        /// </summary>
        Action<ComparisonResult<TItem>> BeforeSyncingAction { get; set; }

        /// <summary>
        /// Synchronizes the source and destination items based on the comparison result.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items which is generated from the comparer agent.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task SyncAsync(ComparisonResult<TItem> comparisonResult, CancellationToken cancellationToken);
    }
}