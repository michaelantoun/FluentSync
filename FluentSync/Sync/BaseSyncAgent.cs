using FluentSync.Comparers;
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
    /// The base sync agent which has the common logic for the sync agent and batch sync agent.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class BaseSyncAgent<TItem> : IBaseSyncAgent<TItem>
    {
        /// <summary>
        /// Gets the sync configurations of the sync agent.
        /// </summary>
        /// <returns></returns>
        protected abstract ISyncConfigurations GetSyncConfigurations();

        /// <summary>
        /// Validates the sync agent properties and configurations.
        /// </summary>
        protected abstract void Validate();

        /// <summary>
        /// Gets the sync provider of the source.
        /// </summary>
        /// <returns></returns>
        protected abstract ISyncBaseProvider<TItem> GetSourceSyncProvider();

        /// <summary>
        /// Gets the sync provider of the destination.
        /// </summary>
        /// <returns></returns>
        protected abstract ISyncBaseProvider<TItem> GetDestinationSyncProvider();

        /// <summary>
        /// Deletes the items from the source.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        protected abstract Task DeleteFromSourceAsync(List<TItem> items, CancellationToken cancellationToken);

        /// <summary>
        /// Delete the items from the destination.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        protected abstract Task DeleteFromDestinationAsync(List<TItem> items, CancellationToken cancellationToken);

        /// <summary>
        /// An action to be called before syncing the items.
        /// </summary>
        public Action<ComparisonResult<TItem>> BeforeSyncingAction { get; set; }

        /// <summary>
        /// Synchronizes the source and destination items based on the comparison result.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items which is generated from the comparer agent.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public async Task SyncAsync(ComparisonResult<TItem> comparisonResult, CancellationToken cancellationToken)
        {
            Validate();

            var tasks = new List<Task>();
            var syncContext = new SyncContext<TItem>();

            BeforeSyncingAction?.Invoke(comparisonResult);

            // Determine the insert, update, and delete operations for each item/match
            InterpretSingleItemOperations(comparisonResult, syncContext);

            InterpretSameMatchesOperations(comparisonResult, syncContext);
            InterpretConflictMatchesOperations(comparisonResult, syncContext);
            InterpretDifferentMatchesOperations(comparisonResult, syncContext);


            foreach (var syncOperation in GetSyncConfigurations().SyncOperationsOrder.Order)
            {
                switch (syncOperation)
                {
                    case SyncOperationType.Insert:
                        if (syncContext.ItemsToBeInsertedInSource.Any())
                            tasks.Add(GetSourceSyncProvider().AddAsync(syncContext.ItemsToBeInsertedInSource, cancellationToken));
                        if (syncContext.ItemsToBeInsertedInDestination.Any())
                            tasks.Add(GetDestinationSyncProvider().AddAsync(syncContext.ItemsToBeInsertedInDestination, cancellationToken));
                        break;
                    case SyncOperationType.Update:
                        if (syncContext.ItemsToBeUpdatedInSource.Any())
                            tasks.Add(GetSourceSyncProvider().UpdateAsync(syncContext.ItemsToBeUpdatedInSource, cancellationToken));
                        if (syncContext.ItemsToBeUpdatedInDestination.Any())
                            tasks.Add(GetDestinationSyncProvider().UpdateAsync(syncContext.ItemsToBeUpdatedInDestination, cancellationToken));
                        break;
                    case SyncOperationType.Delete:
                        if (syncContext.ItemsToBeDeletedFromSource.Any())
                            tasks.Add(DeleteFromSourceAsync(syncContext.ItemsToBeDeletedFromSource, cancellationToken));
                        if (syncContext.ItemsToBeDeletedFromDestination.Any())
                            tasks.Add(DeleteFromDestinationAsync(syncContext.ItemsToBeDeletedFromDestination, cancellationToken));
                        break;
                    default:
                        throw new NotSupportedException($"Not supported {nameof(SyncOperationType)} '{syncOperation.ToString()}'.");
                }

                if (tasks.Any())
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    tasks.Clear();
                }
            }
        }

        /// <summary>
        /// Determines the sync operation of the single items that exist either in the source or the destination.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items.</param>
        /// <param name="syncContext">The sync context contains the items that will be inserted/updated/deleted in source/destination.</param>
        private void InterpretSingleItemOperations(ComparisonResult<TItem> comparisonResult, SyncContext<TItem> syncContext)
        {
            if (comparisonResult.ItemsInSourceOnly.Count > 0)
            {
                switch (GetSyncConfigurations().SyncMode.ItemsInSourceOnly)
                {
                    case SyncItemOperation.None: // do nothing
                        break;
                    case SyncItemOperation.Add:
                        syncContext.ItemsToBeInsertedInDestination.AddRange(comparisonResult.ItemsInSourceOnly);
                        break;
                    case SyncItemOperation.Delete:
                        syncContext.ItemsToBeDeletedFromSource.AddRange(comparisonResult.ItemsInSourceOnly);
                        break;
                    default:
                        throw new NotSupportedException($"Not supported source {nameof(SyncItemOperation)} '{GetSyncConfigurations().SyncMode.ItemsInSourceOnly.ToString()}'.");
                }
            }

            if (comparisonResult.ItemsInDestinationOnly.Count > 0)
            {
                switch (GetSyncConfigurations().SyncMode.ItemsInDestinationOnly)
                {
                    case SyncItemOperation.None: // do nothing
                        break;
                    case SyncItemOperation.Add:
                        syncContext.ItemsToBeInsertedInSource.AddRange(comparisonResult.ItemsInDestinationOnly);
                        break;
                    case SyncItemOperation.Delete:
                        syncContext.ItemsToBeDeletedFromDestination.AddRange(comparisonResult.ItemsInDestinationOnly);
                        break;
                    default:
                        throw new NotSupportedException($"Not supported destination {nameof(SyncItemOperation)} '{GetSyncConfigurations().SyncMode.ItemsInSourceOnly.ToString()}'.");
                }
            }
        }

        /// <summary>
        /// Determines the sync operation of the items that exist in the source and the destination which are equivalent.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items.</param>
        /// <param name="syncContext">The sync context contains the items that will be inserted/updated/deleted in source/destination.</param>
        private void InterpretSameMatchesOperations(ComparisonResult<TItem> comparisonResult, SyncContext<TItem> syncContext)
        {
            var matches = comparisonResult.Matches.Where(x => x.ComparisonResult == MatchComparisonResultType.Same).ToArray();
            if (matches.Any())
            {
                switch (GetSyncConfigurations().SyncMode.SameMatches)
                {
                    case SyncMatchOperation.None: // Do nothing
                        break;
                    case SyncMatchOperation.UpdateSource:
                        syncContext.ItemsToBeUpdatedInSource.AddRange(matches.Select(x => new MatchValuePair<TItem> { NewValue = x.Destination, CurrentValue = x.Source }));
                        break;
                    case SyncMatchOperation.UpdateDestination:
                        syncContext.ItemsToBeUpdatedInDestination.AddRange(matches.Select(x => new MatchValuePair<TItem> { NewValue = x.Source, CurrentValue = x.Destination }));
                        break;
                    case SyncMatchOperation.UpdateOldItem: // Do nothing, because there is not old item
                    case SyncMatchOperation.UpdateOldDestination:
                    case SyncMatchOperation.UpdateOldSource:
                        break;
                    default:
                        throw new NotSupportedException($"Not supported {nameof(SyncMatchOperation)} '{GetSyncConfigurations().SyncMode.SameMatches.ToString()}' for {MatchComparisonResultType.Same.ToString()} matches.");
                }
            }
        }

        /// <summary>
        /// Determines the sync operation of the items that exist in the source and the destination which are not the same and no item is newer.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items.</param>
        /// <param name="syncContext">The sync context contains the items that will be inserted/updated/deleted in source/destination.</param>
        private void InterpretConflictMatchesOperations(ComparisonResult<TItem> comparisonResult, SyncContext<TItem> syncContext)
        {
            var matches = comparisonResult.Matches.Where(x => x.ComparisonResult == MatchComparisonResultType.Conflict).ToArray();
            if (matches.Any())
            {
                switch (GetSyncConfigurations().SyncMode.ConflictMatches)
                {
                    case SyncMatchOperation.None: // Do nothing
                        break;
                    case SyncMatchOperation.UpdateSource:
                        syncContext.ItemsToBeUpdatedInSource.AddRange(matches.Select(x => new MatchValuePair<TItem> { NewValue = x.Destination, CurrentValue = x.Source }));
                        break;
                    case SyncMatchOperation.UpdateDestination:
                        syncContext.ItemsToBeUpdatedInDestination.AddRange(matches.Select(x => new MatchValuePair<TItem> { NewValue = x.Source, CurrentValue = x.Destination }));
                        break;
                    case SyncMatchOperation.UpdateOldItem:
                    case SyncMatchOperation.UpdateOldDestination:
                    case SyncMatchOperation.UpdateOldSource:
                        throw new Exception($"Conflict matches operation cannot be {GetSyncConfigurations().SyncMode.ConflictMatches.ToString()}.");
                    default:
                        throw new NotSupportedException($"Not supported {nameof(SyncMatchOperation)} '{GetSyncConfigurations().SyncMode.SameMatches.ToString()}' for {MatchComparisonResultType.Same.ToString()} matches.");
                }
            }
        }

        /// <summary>
        /// Determines the sync operation of the items that exist in the source and the destination which one of the pair items is newer.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items.</param>
        /// <param name="syncContext">The sync context contains the items that will be inserted/updated/deleted in source/destination.</param>
        private void InterpretDifferentMatchesOperations(ComparisonResult<TItem> comparisonResult, SyncContext<TItem> syncContext)
        {
            var newerSourceMatches = comparisonResult.Matches.Where(x => x.ComparisonResult == MatchComparisonResultType.NewerSource).ToArray();
            var newerDestinationMatches = comparisonResult.Matches.Where(x => x.ComparisonResult == MatchComparisonResultType.NewerDestination).ToArray();

            if (newerSourceMatches.Any() || newerDestinationMatches.Any())
            {
                switch (GetSyncConfigurations().SyncMode.NewerMatches)
                {
                    case SyncMatchOperation.None: // Do nothing
                        break;
                    case SyncMatchOperation.UpdateSource:
                        syncContext.ItemsToBeUpdatedInSource.AddRange(newerSourceMatches.Union(newerDestinationMatches).Select(x => new MatchValuePair<TItem> { NewValue = x.Destination, CurrentValue = x.Source }));
                        break;
                    case SyncMatchOperation.UpdateDestination:
                        syncContext.ItemsToBeUpdatedInDestination.AddRange(newerSourceMatches.Union(newerDestinationMatches).Select(x => new MatchValuePair<TItem> { NewValue = x.Source, CurrentValue = x.Destination }));
                        break;
                    case SyncMatchOperation.UpdateOldItem:
                        if (newerSourceMatches.Any())
                            syncContext.ItemsToBeUpdatedInDestination.AddRange(newerSourceMatches.Select(x => new MatchValuePair<TItem> { NewValue = x.Source, CurrentValue = x.Destination }));
                        if (newerDestinationMatches.Any())
                            syncContext.ItemsToBeUpdatedInSource.AddRange(newerDestinationMatches.Select(x => new MatchValuePair<TItem> { NewValue = x.Destination, CurrentValue = x.Source }));
                        break;
                    case SyncMatchOperation.UpdateOldDestination:
                        if (newerSourceMatches.Any())
                            syncContext.ItemsToBeUpdatedInDestination.AddRange(newerSourceMatches.Select(x => new MatchValuePair<TItem> { NewValue = x.Source, CurrentValue = x.Destination }));
                        break;
                    case SyncMatchOperation.UpdateOldSource:
                        if (newerDestinationMatches.Any())
                            syncContext.ItemsToBeUpdatedInSource.AddRange(newerDestinationMatches.Select(x => new MatchValuePair<TItem> { NewValue = x.Destination, CurrentValue = x.Source }));
                        break;
                    default:
                        throw new NotSupportedException($"Not supported {nameof(SyncMatchOperation)} '{GetSyncConfigurations().SyncMode.SameMatches.ToString()}' for {MatchComparisonResultType.Same.ToString()} matches.");
                }
            }
        }
    }
}
