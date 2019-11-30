using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Tests.Internals;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.BatchSyncAgent.LoadTests
{
    public class BatchSyncAgentLoadTestsTwoWay : BatchSyncAgentLoadTests
    {
        [Fact]
        public async Task Sync_LoadTest_TwoWay()
        {
            CreateRandomHobbies(out var sourceDictionary, out var destinationDictionary);

            await CreateSyncAgent(sourceDictionary, destinationDictionary)
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.TwoWay)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            AssertionHelper.VerifyDictionariesAreEquivalent(sourceDictionary, destinationDictionary);
        }
    }
}
