using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Tests.Internals;

namespace FluentSync.Tests.Sync.SyncAgent.LoadTests
{
    public class SyncAgentLoadTestsTwoWay : SyncAgentLoadTests
    {
        [Fact]
        public async Task Sync_LoadTest_TwoWay_String()
        {
            CreateRandomStringLists(out var sourceItems, out var destinationItems);

            await CreateSyncAgent(sourceItems, destinationItems)
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.TwoWay)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            AssertionHelper.VerifySortedSetsAreEquivalent(sourceItems, destinationItems);
        }
    }
}
