using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Tests.Internals;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.SyncAgent.LoadTests
{
    public class SyncAgentLoadTestsMirrorToSource : SyncAgentLoadTests
    {
        [Fact]
        public async Task Sync_LoadTest_MirrorToSource_String()
        {
            CreateRandomStringLists(out var sourceItems, out var destinationItems);

            SortedSet<string> expectedSourceList = new SortedSet<string>(destinationItems)
                , expectedDestinationList = new SortedSet<string>(destinationItems);

            await CreateSyncAgent(sourceItems, destinationItems)
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToSource)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            AssertionHelper.VerifySortedSetsAreEquivalent(sourceItems, expectedSourceList);
            AssertionHelper.VerifySortedSetsAreEquivalent(destinationItems, expectedDestinationList);
        }


    }
}
