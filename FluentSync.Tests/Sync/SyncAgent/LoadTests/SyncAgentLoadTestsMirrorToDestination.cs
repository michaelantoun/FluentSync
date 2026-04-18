using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Tests.Internals;

namespace FluentSync.Tests.Sync.SyncAgent.LoadTests
{
    public class SyncAgentLoadTestsMirrorToDestination : SyncAgentLoadTests
    {
        [Fact]
        public async Task Sync_LoadTest_MirrorToDestination_String()
        {
            CreateRandomStringLists(out var sourceItems, out var destinationItems);

            SortedSet<string> expectedSourceList = new SortedSet<string>(sourceItems)
                , expectedDestinationList = new SortedSet<string>(sourceItems);

            await CreateSyncAgent(sourceItems, destinationItems)
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            // Very slow, interesting!!!
            //sourceItems.Should().BeEquivalentTo(expectedSourceList);
            //destinationItems.Should().BeEquivalentTo(expectedDestinationList);

            AssertionHelper.VerifySortedSetsAreEquivalent(sourceItems, expectedSourceList);
            AssertionHelper.VerifySortedSetsAreEquivalent(destinationItems, expectedDestinationList);
        }
    }
}
