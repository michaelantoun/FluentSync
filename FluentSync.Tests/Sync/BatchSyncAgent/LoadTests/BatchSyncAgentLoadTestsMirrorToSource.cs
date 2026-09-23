using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Tests.Internals;

namespace FluentSync.Tests.Sync.BatchSyncAgent.LoadTests
{
    public class BatchSyncAgentLoadTestsMirrorToSource : BatchSyncAgentLoadTests
    {
        [Fact]
        public async Task Sync_LoadTest_MirrorToSource()
        {
            CreateRandomHobbies(out var sourceDictionary, out var destinationDictionary);

            Dictionary<int, Hobby> expectedSourceDictionary = new Dictionary<int, Hobby>(destinationDictionary)
                , expectedDestinationDictionary = new Dictionary<int, Hobby>(destinationDictionary);

            await CreateSyncAgent(sourceDictionary, destinationDictionary)
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToSource)
                .SyncAsync(CancellationToken.None);

            AssertionHelper.VerifyDictionariesAreEquivalent(sourceDictionary, expectedSourceDictionary);
            AssertionHelper.VerifyDictionariesAreEquivalent(destinationDictionary, expectedDestinationDictionary);
        }
    }
}
