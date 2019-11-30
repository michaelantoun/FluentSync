using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using FluentSync.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.BatchSyncAgent
{
    public partial class BatchSyncAgentTests
    {
        private static IDictionary<int?, Event> CreateSourceEventDictionary()
        {
            return new List<Event> {
                new Event { Id = 2, Title = "Birthday", ModifiedDate = new DateTime(2000, 1, 1) }, // Same match
                new Event { Id = 1, Title = "soccer match", ModifiedDate = new DateTime(2000, 1, 2) }, // Older source item
                new Event { Id = 6, Title = "Private", ModifiedDate = null }, // Exists in source only
                new Event { Id = 4, Title = "Hang-out", ModifiedDate = new DateTime(2000, 1, 2) }, // Newer source item
                new Event { Id = 5, Title = "bad", ModifiedDate = new DateTime(2000, 1, 8) } // Conflict match
            }.ToDictionary(x => x.Id);
        }

        private static IDictionary<int?, Event> CreateDestinationEventDictionary()
        {
            return new List<Event> {
                new Event { Id = 1, Title = "Soccer Match", ModifiedDate = new DateTime(2000, 1, 3) }, // Newer destination item
                new Event { Id = 2, Title = "Birthday", ModifiedDate = new DateTime(2000, 1, 1) }, // same match
                new Event { Id = 3, Title = "Free-time", ModifiedDate = null }, // Exists in destination only
                new Event { Id = 4, Title = "hang-out", ModifiedDate = new DateTime(2000, 1, 1) }, // Old destination item
                new Event { Id = 5, Title = "Bad", ModifiedDate = new DateTime(2000, 1, 8) }, // Conflict match
                new Event { Id = 7, Title = "Football Match", ModifiedDate = new DateTime(2000, 1, 4) }, // Exists in destination only
                new Event { Id = 8, Title = "Basketball Match", ModifiedDate = new DateTime(2000, 1, 4) } // Exists in destination only
            }.ToDictionary(x => x.Id);
        }

        private static IBatchSyncAgent<int?, Event> CreateSyncAgent(IDictionary<int?, Event> source, IDictionary<int?, Event> destination)
        {
            return CreateSyncAgent()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination);
        }

        private static IBatchSyncAgent<int?, Event> CreateSyncAgent()
        {
            return BatchSyncAgent<int?, Event>.Create()
                .Configure((c) => c.BatchSize = 3)
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) =>
                {
                    if (s.Title == d.Title && s.ModifiedDate == d.ModifiedDate)
                        return MatchComparisonResultType.Same;
                    else if (s.ModifiedDate < d.ModifiedDate)
                        return MatchComparisonResultType.NewerDestination;
                    else if (s.ModifiedDate > d.ModifiedDate)
                        return MatchComparisonResultType.NewerSource;
                    else
                        return MatchComparisonResultType.Conflict;
                });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public async Task Sync_Class_SyncNone(int batchSize)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.None;
                    c.BatchSize = batchSize;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            source.Should().BeEquivalentTo(expectedSource);
            destination.Should().BeEquivalentTo(expectedDestination);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public async Task Sync_Class_UpdateDestination(int batchSize)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.UpdateDestination;
                    c.BatchSize = batchSize;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            source.Should().BeEquivalentTo(expectedSource);

            expectedDestination.Add(6, expectedSource[6]);
            expectedDestination[4] = expectedSource[4];
            destination.Should().BeEquivalentTo(expectedDestination);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public async Task Sync_Class_MirrorToDestination(int batchSize)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination;
                    c.BatchSize = batchSize;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            source.Should().BeEquivalentTo(expectedSource);

            expectedDestination.Remove(3);
            expectedDestination.Remove(7);
            expectedDestination.Remove(8);
            expectedDestination.Add(6, expectedSource[6]);
            expectedDestination[1] = expectedSource[1];
            expectedDestination[4] = expectedSource[4];
            expectedDestination[5] = expectedSource[5];
            destination.Should().BeEquivalentTo(expectedDestination);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public async Task Sync_Class_TwoWay(int batchSize)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.TwoWay;
                    c.BatchSize = batchSize;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            expectedSource[1] = expectedDestination[1];
            expectedSource.Add(3, expectedDestination[3]);
            expectedSource.Add(7, expectedDestination[7]);
            expectedSource.Add(8, expectedDestination[8]);

            expectedDestination[4] = expectedSource[4];
            expectedDestination.Add(6, expectedSource[6]);

            source.Should().BeEquivalentTo(expectedSource);
            destination.Should().BeEquivalentTo(expectedDestination);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public async Task Sync_Class_UpdateSource(int batchSize)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.UpdateSource;
                    c.BatchSize = batchSize;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            expectedSource[1] = expectedDestination[1];
            expectedSource.Add(3, expectedDestination[3]);
            expectedSource.Add(7, expectedDestination[7]);
            expectedSource.Add(8, expectedDestination[8]);

            source.Should().BeEquivalentTo(expectedSource);
            destination.Should().BeEquivalentTo(expectedDestination);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        public async Task Sync_Class_MirrorToSource(int batchSize)
        {
            IDictionary<int?, Event> source = CreateSourceEventDictionary()
                , destination = CreateDestinationEventDictionary();

            await CreateSyncAgent(source, destination)
                .Configure((c) =>
                {
                    c.SyncMode.SyncModePreset = SyncModePreset.MirrorToSource;
                    c.BatchSize = batchSize;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            expectedSource[1] = expectedDestination[1];
            expectedSource[4] = expectedDestination[4];
            expectedSource[5] = expectedDestination[5];
            expectedSource.Add(3, expectedDestination[3]);
            expectedSource.Add(7, expectedDestination[7]);
            expectedSource.Add(8, expectedDestination[8]);
            expectedSource.Remove(6);

            source.Should().BeEquivalentTo(expectedSource);
            destination.Should().BeEquivalentTo(expectedDestination);
        }

        [Fact]
        public async Task Sync_Class_TwoWayWithSyncProviders()
        {
            var syncAgent = CreateSyncAgent();
            DictionaryBatchSyncProvider<int?, Event> source = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateSourceEventDictionary(), KeySelector = syncAgent.KeySelector }
                , destination = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateDestinationEventDictionary(), KeySelector = syncAgent.KeySelector };

            await syncAgent
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.TwoWay)
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            IDictionary<int?, Event> expectedSource = CreateSourceEventDictionary()
                , expectedDestination = CreateDestinationEventDictionary();

            expectedSource[1] = expectedDestination[1];
            expectedSource.Add(3, expectedDestination[3]);
            expectedSource.Add(7, expectedDestination[7]);
            expectedSource.Add(8, expectedDestination[8]);

            expectedDestination[4] = expectedSource[4];
            expectedDestination.Add(6, expectedSource[6]);

            source.Items.Should().BeEquivalentTo(expectedSource);
            destination.Items.Should().BeEquivalentTo(expectedDestination);

            syncAgent.ToString().Should().Be($"{nameof(syncAgent.Configurations)}: {{{syncAgent.Configurations}}}");
        }

        [Theory]
        [InlineData(SyncModePreset.MirrorToDestination)]
        [InlineData(SyncModePreset.MirrorToSource)]
        public async Task Sync_Class_WithExternalComparerAgent(SyncModePreset syncModePreset)
        {
            var syncAgent = CreateSyncAgent();
            DictionaryBatchSyncProvider<int?, Event> source = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateSourceEventDictionary(), KeySelector = syncAgent.KeySelector }
                , destination = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateDestinationEventDictionary(), KeySelector = syncAgent.KeySelector };

            var keyComparisonResult = await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source.Items.Keys)
                .SetDestinationProvider(destination.Items.Keys)
                .CompareAsync(CancellationToken.None);

            // The sync agent should handle changes to source and destination gracefully.
            if (syncModePreset == SyncModePreset.MirrorToDestination)
                await source.DeleteAsync(new List<int?> { 5 }, CancellationToken.None).ConfigureAwait(false);
            else if (syncModePreset == SyncModePreset.MirrorToSource)
                await destination.DeleteAsync(new List<int?> { 5 }, CancellationToken.None).ConfigureAwait(false);

            await syncAgent
                .Configure((c) => c.SyncMode.SyncModePreset = syncModePreset)
                .SetComparerAgent(null)
                .SetSourceProvider((IBatchSyncProvider<int?, Event>)source)
                .SetDestinationProvider((IBatchSyncProvider<int?, Event>)destination)
                .SyncAsync(keyComparisonResult, CancellationToken.None).ConfigureAwait(false);

            source.Items.Should().BeEquivalentTo(destination.Items);
        }
    }
}
