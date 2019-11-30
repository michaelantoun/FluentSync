using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Tests.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.BatchSyncAgent
{
    public partial class BatchSyncAgentTests
    {
        [Theory]
        [InlineData(1, 5)]
        [InlineData(100, 2)]
        public async Task Sync_Class_BeforeSyncingActionShouldBeCalled(int batchSize, int expectedActionCalledCount)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            int actionCalledCount = 0;
            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination;
                    c.BatchSize = batchSize;
                })
                .SetBeforeSyncingAction((cr) =>
                {
                    if (cr.ItemsInSourceOnly.Any() || cr.ItemsInDestinationOnly.Any() || cr.Matches.Any())
                        actionCalledCount++;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            actionCalledCount.Should().Be(expectedActionCalledCount);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 1)]
        public async Task Sync_Class_BeforeSyncingKeysActionShouldBeCalledOnce(int batchSize, int expectedActionCalledCount)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            int actionCalledCount = 0;
            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination;
                    c.BatchSize = batchSize;
                })
                .SetBeforeSyncingKeysAction((cr) =>
                {
                    if (cr.KeysInSourceOnly.Any() || cr.KeysInDestinationOnly.Any() || cr.Matches.Any())
                        actionCalledCount++;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            actionCalledCount.Should().Be(expectedActionCalledCount);
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task Sync_Class_BeforeDeletingItemsFromSourceActionShouldBeCalled(int batchSize, int expectedActionCalledCount)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            int actionCalledCount = 0;
            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.MirrorToSource;
                    c.BatchSize = batchSize;
                })
                .SetBeforeDeletingItemsFromSourceAction((items) =>
                {
                    if (items.Any())
                        actionCalledCount++;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            actionCalledCount.Should().Be(expectedActionCalledCount);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(100, 1)]
        public async Task Sync_Class_BeforeDeletingItemsFromDestinationActionShouldBeCalled(int batchSize, int expectedActionCalledCount)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            int actionCalledCount = 0;
            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination;
                    c.BatchSize = batchSize;
                })
                .SetBeforeDeletingItemsFromDestinationAction((items) =>
                {
                    if (items.Any())
                        actionCalledCount++;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            actionCalledCount.Should().Be(expectedActionCalledCount);
        }
    }
}
