using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Comparers.Configurations;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using FluentSync.Tests.Internals;
using FluentSync.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.SyncAgent
{
    public partial class SyncAgentTests
    {
        private static List<Event> CreateSourceEventList() => new List<Event> {
            new Event{Id = 2, Title ="Birthday", ModifiedDate = new DateTime(2000, 1, 1)}, // Same match
            new Event{Id = 1, Title ="soccer match", ModifiedDate = new DateTime(2000, 1, 2)}, // Older source item
            new Event{Id = null, Title ="Private", ModifiedDate = null }, // Exists in source only
            new Event(), // Exists in source only
            new Event{Id = 4, Title ="Hang-out", ModifiedDate = new DateTime(2000, 1, 2)}, // Newer source item
            new Event{Id = 5, Title ="bad", ModifiedDate = new DateTime(2000, 1, 8)} // Conflict match
        };

        private static List<Event> CreateDestinationEventList() => new List<Event> {
            new Event{Id = 1, Title ="Soccer Match", ModifiedDate = new DateTime(2000, 1, 3) }, // Newer destination item
            new Event{Id = 2, Title ="Birthday", ModifiedDate = new DateTime(2000, 1, 1)}, // same match
            new Event{Id = 3, Title ="Free-time", ModifiedDate = null }, // Exists in source only
            new Event{Id = 4, Title ="hang-out", ModifiedDate = new DateTime(2000, 1, 1)}, // Old destination item
            new Event{Id = 5, Title ="Bad", ModifiedDate = new DateTime(2000, 1, 8)} // Conflict match
        };

        private static SortedSet<Event> CreateSourceEventSortedSet() => new SortedSet<Event> {
            new Event{Id = 2, Title ="Birthday", ModifiedDate = new DateTime(2000, 1, 1)}, // Same match
            new Event{Id = 1, Title ="soccer match", ModifiedDate = new DateTime(2000, 1, 2)}, // Older source item
            new Event{Id = 4, Title ="Hang-out", ModifiedDate = new DateTime(2000, 1, 2)}, // Newer source item
            new Event{Id = 5, Title ="bad", ModifiedDate = new DateTime(2000, 1, 8)} // Conflict match
        };

        private static SortedSet<Event> CreateDestinationEventSortedSet() => new SortedSet<Event> {
            new Event{Id = 1, Title ="Soccer Match", ModifiedDate = new DateTime(2000, 1, 3) }, // Newer destination item
            new Event{Id = 2, Title ="Birthday", ModifiedDate = new DateTime(2000, 1, 1)}, // same match
            new Event{Id = 3, Title ="Free-time", ModifiedDate = null }, // Exists in source only
            new Event{Id = 4, Title ="hang-out", ModifiedDate = new DateTime(2000, 1, 1)}, // Old destination item
            new Event{Id = 5, Title ="Bad", ModifiedDate = new DateTime(2000, 1, 8)} // Conflict match
        };

        private static ISyncAgent<int?, Event> CreateSyncAgent()
        {
            return SyncAgent<int?, Event>.Create()
                .SetComparerAgent(ComparerAgent<int?, Event>.Create()
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
                }));
        }

        private static ISyncAgent<int?, Event> CreateSyncAgent(List<Event> source, List<Event> destination)
        {
            return CreateSyncAgent()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination);
        }

        #region SyncMode - ItemsInSourceOnly

        [Fact]
        public async Task Sync_Custom_ItemsInSourceOnly_Add_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ItemsInSourceOnly = SyncItemOperation.Add)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList().Union(CreateSourceEventList().Where(x => !x.Id.HasValue)));
        }

        [Fact]
        public async Task Sync_Custom_ItemsInSourceOnly_Delete_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ItemsInSourceOnly = SyncItemOperation.Delete)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList().Where(x => x.Id.HasValue));
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        #endregion

        #region SyncMode - ItemsInDestinationOnly

        [Fact]
        public async Task Sync_Custom_ItemsInDestinationOnly_Add_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ItemsInDestinationOnly = SyncItemOperation.Add)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var originalDestinationList = CreateDestinationEventList();
            source.Should().BeEquivalentTo(CreateSourceEventList().Union(new[] { originalDestinationList[2] }));
            destination.Should().BeEquivalentTo(originalDestinationList);
        }

        [Fact]
        public async Task Sync_Custom_ItemsInDestinationOnly_Delete_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ItemsInDestinationOnly = SyncItemOperation.Delete)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList().Where(x => x.Id != 3));
        }

        #endregion

        #region SyncMode - Same Matches

        [Fact]
        public async Task Sync_Custom_SameMatches_None_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SameMatches = SyncMatchOperation.None)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_SameMatches_UpdateDestination_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SameMatches = SyncMatchOperation.UpdateDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_SameMatches_UpdateOldDestination_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SameMatches = SyncMatchOperation.UpdateOldDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_SameMatches_UpdateOldItem_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SameMatches = SyncMatchOperation.UpdateOldItem)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_SameMatches_UpdateSource_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SameMatches = SyncMatchOperation.UpdateSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_SameMatches_UpdateOldSource_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SameMatches = SyncMatchOperation.UpdateOldSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        #endregion

        #region SyncMode - Conflict Matches

        [Fact]
        public async Task Sync_Custom_ConflictMatches_None_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ConflictMatches = SyncMatchOperation.None)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_ConflictMatches_UpdateDestination_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ConflictMatches = SyncMatchOperation.UpdateDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());

            var expectedDestinationResult = CreateDestinationEventList();
            expectedDestinationResult.First(x => x.Id == 5).Title = "bad";
            destination.Should().BeEquivalentTo(expectedDestinationResult);
        }

        [Fact]
        public void Sync_Custom_ConflictMatches_UpdateOldDestination_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            Func<Task> act = async () => await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ConflictMatches = SyncMatchOperation.UpdateOldDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<Exception>().WithMessage($"Conflict matches operation cannot be set to {SyncMatchOperation.UpdateOldDestination.ToString()}.");
        }

        [Fact]
        public void Sync_Custom_ConflictMatches_UpdateOldItem_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            Func<Task> act = async () => await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ConflictMatches = SyncMatchOperation.UpdateOldItem)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<Exception>().WithMessage($"Conflict matches operation cannot be set to {SyncMatchOperation.UpdateOldItem.ToString()}.");
        }

        [Fact]
        public async Task Sync_Custom_ConflictMatches_UpdateSource_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ConflictMatches = SyncMatchOperation.UpdateSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceResult = CreateSourceEventList();
            expectedSourceResult.First(x => x.Id == 5).Title = "Bad";
            source.Should().BeEquivalentTo(expectedSourceResult);
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public void Sync_Custom_ConflictMatches_UpdateSourceIfOld_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            Func<Task> act = async () => await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.ConflictMatches = SyncMatchOperation.UpdateOldSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<Exception>().WithMessage($"Conflict matches operation cannot be set to {SyncMatchOperation.UpdateOldSource.ToString()}.");
        }

        #endregion

        #region SyncMode - Newer Matches

        [Fact]
        public async Task Sync_Custom_NewerMatches_None_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.None)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(CreateSourceEventList());
            destination.Should().BeEquivalentTo(CreateDestinationEventList());
        }

        [Fact]
        public async Task Sync_Custom_NewerMatches_UpdateDestination_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.UpdateDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceList = CreateSourceEventList();
            var expectedDestinationList = CreateDestinationEventList();

            expectedDestinationList.RemoveAll(x => new int?[] { 1, 4 }.Contains(x.Id));
            expectedDestinationList.AddRange(expectedSourceList.Where(x => new int?[] { 1, 4 }.Contains(x.Id)));

            source.Should().BeEquivalentTo(expectedSourceList);
            destination.Should().BeEquivalentTo(expectedDestinationList);
        }

        [Fact]
        public async Task Sync_Custom_NewerMatches_UpdateDestination_Class_NonEmptySortedSets()
        {
            SortedSet<Event> source = CreateSourceEventSortedSet(), destination = CreateDestinationEventSortedSet();

            await CreateSyncAgent()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.UpdateDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceList = CreateSourceEventSortedSet();
            var expectedDestinationList = CreateDestinationEventSortedSet();

            expectedDestinationList.RemoveWhere(x => new int?[] { 1, 4 }.Contains(x.Id));
            expectedSourceList.Where(x => new int?[] { 1, 4 }.Contains(x.Id))
                .ToList().ForEach(x => expectedDestinationList.Add(x));


            AssertionHelper.VerifySortedSetsAreEquivalent(source, expectedSourceList);
            AssertionHelper.VerifySortedSetsAreEquivalent(destination, expectedDestinationList);
        }

        [Fact]
        public async Task Sync_Custom_NewerMatches_UpdateOldDestination_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.UpdateOldDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceList = CreateSourceEventList();
            var expectedDestinationList = CreateDestinationEventList();

            expectedDestinationList.RemoveAll(x => x.Id == 4);
            expectedDestinationList.AddRange(expectedSourceList.Where(x => x.Id == 4));

            source.Should().BeEquivalentTo(expectedSourceList);
            destination.Should().BeEquivalentTo(expectedDestinationList);
        }

        [Fact]
        public async Task Sync_Custom_NewerMatches_UpdateOldItem_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.UpdateOldItem)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceList = CreateSourceEventList();
            var expectedDestinationList = CreateDestinationEventList();

            expectedSourceList.RemoveAll(x => x.Id == 1);
            expectedSourceList.Add(expectedDestinationList.First(x => x.Id == 1));

            expectedDestinationList.RemoveAll(x => x.Id == 4);
            expectedDestinationList.Add(expectedSourceList.First(x => x.Id == 4));

            source.Should().BeEquivalentTo(expectedSourceList);
            destination.Should().BeEquivalentTo(expectedDestinationList);
        }

        [Fact]
        public async Task Sync_Custom_NewerMatches_UpdateSource_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.UpdateSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceList = CreateSourceEventList();
            var expectedDestinationList = CreateDestinationEventList();

            expectedSourceList.RemoveAll(x => new int?[] { 1, 4 }.Contains(x.Id));
            expectedSourceList.AddRange(expectedDestinationList.Where(x => new int?[] { 1, 4 }.Contains(x.Id)));

            source.Should().BeEquivalentTo(expectedSourceList);
            destination.Should().BeEquivalentTo(expectedDestinationList);
        }

        [Fact]
        public async Task Sync_Custom_NewerMatches_UpdateOldSource_Class_NonEmptyLists()
        {
            List<Event> source = CreateSourceEventList(), destination = CreateDestinationEventList();

            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.NewerMatches = SyncMatchOperation.UpdateOldSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            var expectedSourceList = CreateSourceEventList();
            var expectedDestinationList = CreateDestinationEventList();

            expectedSourceList.RemoveAll(x => x.Id == 1);
            expectedSourceList.Add(expectedDestinationList.First(x => x.Id == 1));

            source.Should().BeEquivalentTo(expectedSourceList);
            destination.Should().BeEquivalentTo(expectedDestinationList);
        }

        #endregion

        [Fact]
        public async Task Sync_Class_BeforeSyncingActionShouldBeCalled()
        {
            List<Event> source = CreateSourceEventList()
                , destination = CreateDestinationEventList();

            int actionCalledCount = 0;
            await CreateSyncAgent(source, destination)
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination)
                .SetBeforeSyncingAction((cr) =>
                {
                    if (cr.ItemsInSourceOnly.Any() || cr.ItemsInDestinationOnly.Any() || cr.Matches.Any())
                        actionCalledCount++;
                })
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            actionCalledCount.Should().Be(1);
        }

        [Theory]
        [InlineData(SyncModePreset.MirrorToDestination)]
        [InlineData(SyncModePreset.MirrorToSource)]
        public async Task SyncWithExternalComparerAgent(SyncModePreset syncModePreset)
        {
            var syncAgent = CreateSyncAgent();
            SortedSetSyncProvider<Event> source = new SortedSetSyncProvider<Event> { Items = CreateSourceEventSortedSet() }
                , destination = new SortedSetSyncProvider<Event> { Items = CreateDestinationEventSortedSet() };

            var comparisonResult = await ComparerAgent<int?, Event>.Create()
                .Configure((c) =>
                {
                    c.AllowDuplicateKeys = RuleAllowanceType.None;
                    c.AllowDuplicateItems = RuleAllowanceType.None;
                    c.AllowNullableItems = RuleAllowanceType.None;
                })
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
                })
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            await syncAgent
                .Configure((c) => c.SyncMode.SyncModePreset = syncModePreset)
                .SetComparerAgent(null)
                .SetSourceProvider((ISyncProvider<Event>)source)
                .SetDestinationProvider((ISyncProvider<Event>)destination)
                .SyncAsync(comparisonResult, CancellationToken.None).ConfigureAwait(false);

            source.Items.Should().BeEquivalentTo(destination.Items);
        }

    }
}
