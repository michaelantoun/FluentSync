using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.SyncAgent
{
    public partial class SyncAgentTests
    {
        [Fact]
        public async Task Sync_UpdateSource_Int_NonEmptyLists()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int> { 6, 10, 5 };

            await SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.UpdateSource)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(new List<int> { 5, 4, 9, 6, 10 });
            destination.Should().BeEquivalentTo(new List<int> { 6, 10, 5 });
        }

        [Fact]
        public async Task Sync_UpdateSource_Int_WithEmptyDestination()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int>();

            await SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.UpdateSource)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(new List<int> { 5, 4, 9 });
            destination.Should().BeEmpty();
        }

        [Fact]
        public async Task Sync_UpdateSource_Int_WithEmptySourceList()
        {
            List<int> source = new List<int>()
                , destination = new List<int> { 6, 10, 5 };

            await SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.UpdateSource)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEquivalentTo(new List<int> { 6, 10, 5 });
            destination.Should().BeEquivalentTo(new List<int> { 6, 10, 5 });
        }

        [Fact]
        public async Task Sync_UpdateSource_Int_WithEmptyLists()
        {
            List<int> source = new List<int>()
                , destination = new List<int>();

            await SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.UpdateSource)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Should().BeEmpty();
            destination.Should().BeEmpty();
        }
    }
}
