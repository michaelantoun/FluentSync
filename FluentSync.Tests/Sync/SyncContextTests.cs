using FluentAssertions;
using FluentSync.Sync;
using System.Collections.Generic;
using Xunit;

namespace FluentSync.Tests.Sync
{
    public class SyncContextTests
    {
        [Fact]
        public void SyncContextShouldHaveAValidString()
        {
            var syncContext = new SyncContext<int>();

            syncContext.ItemsToBeUpdatedInDestination.Add(new MatchValuePair<int>());
            AddItemsToList(syncContext.ItemsToBeDeletedFromDestination, 2);
            AddItemsToList(syncContext.ItemsToBeDeletedFromSource, 3);
            AddItemsToList(syncContext.ItemsToBeInsertedInDestination, 4);
            AddItemsToList(syncContext.ItemsToBeInsertedInSource, 5);

            syncContext.ToString().Should().Be($"{nameof(syncContext.ItemsToBeInsertedInSource)}: {syncContext.ItemsToBeInsertedInSource.Count}, {nameof(syncContext.ItemsToBeDeletedFromSource)}: {syncContext.ItemsToBeDeletedFromSource.Count}, {nameof(syncContext.ItemsToBeUpdatedInSource)}: {syncContext.ItemsToBeUpdatedInSource.Count}"
                + $"{nameof(syncContext.ItemsToBeInsertedInDestination)}: {syncContext.ItemsToBeInsertedInDestination.Count}, {nameof(syncContext.ItemsToBeDeletedFromDestination)}: {syncContext.ItemsToBeDeletedFromDestination.Count}, {nameof(syncContext.ItemsToBeUpdatedInDestination)}: {syncContext.ItemsToBeUpdatedInDestination.Count}");
        }

        private void AddItemsToList(List<int> list, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }
        }
    }
}
