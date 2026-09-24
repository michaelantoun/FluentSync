using FluentSync.Sync;
using FluentSync.Sync.Configurations;

namespace FluentSync.Tests.Sync.SyncAgent
{
    /// <summary>
    /// The default branches of the sync mode switches are only reachable with an operation value
    /// outside the enum, but their messages still have to name the operation that was rejected.
    /// Each of these asserts the branch reports its own property rather than a neighbouring one.
    /// </summary>
    public partial class SyncAgentTests
    {
        private static ISyncAgent<int, int> CreateAgent(Action<SyncMode> configure)
        {
            var syncAgent = SyncAgent<int>.Create();
            configure(syncAgent.Configurations.SyncMode);

            return syncAgent
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(new List<int>())
                .SetDestinationProvider(new List<int>());
        }

        private static ComparisonResult<int> MatchWith(MatchComparisonResultType comparisonResult)
        {
            var result = new ComparisonResult<int>();
            result.Matches.Add(new MatchComparisonResult<int> { Source = 1, Destination = 1, ComparisonResult = comparisonResult });

            return result;
        }

        [Fact]
        public async Task Sync_UnsupportedItemsInSourceOnlyOperation()
        {
            var syncAgent = CreateAgent(m => m.ItemsInSourceOnly = (SyncItemOperation)99);

            var comparisonResult = new ComparisonResult<int>();
            comparisonResult.ItemsInSourceOnly.Add(7);

            Func<Task> act = async () => await syncAgent.SyncAsync(comparisonResult, CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NotSupportedException>(act)).Message.ShouldBe($"Not supported source {nameof(SyncItemOperation)} '99'.");
        }

        [Fact]
        public async Task Sync_UnsupportedItemsInDestinationOnlyOperation()
        {
            // The destination branch used to report SyncMode.ItemsInSourceOnly, which is None here,
            // so the message named an operation the caller had not set.
            var syncAgent = CreateAgent(m => m.ItemsInDestinationOnly = (SyncItemOperation)99);

            var comparisonResult = new ComparisonResult<int>();
            comparisonResult.ItemsInDestinationOnly.Add(7);

            Func<Task> act = async () => await syncAgent.SyncAsync(comparisonResult, CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NotSupportedException>(act)).Message.ShouldBe($"Not supported destination {nameof(SyncItemOperation)} '99'.");
        }

        [Fact]
        public async Task Sync_UnsupportedSameMatchesOperation()
        {
            var syncAgent = CreateAgent(m => m.SameMatches = (SyncMatchOperation)99);

            Func<Task> act = async () => await syncAgent.SyncAsync(MatchWith(MatchComparisonResultType.Same), CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NotSupportedException>(act))
                .Message.ShouldBe($"Not supported {nameof(SyncMatchOperation)} '99' for {MatchComparisonResultType.Same} matches.");
        }

        [Fact]
        public async Task Sync_UnsupportedConflictMatchesOperation()
        {
            // The conflict branch used to report SameMatches, and to say "for Same matches".
            var syncAgent = CreateAgent(m => m.ConflictMatches = (SyncMatchOperation)99);

            Func<Task> act = async () => await syncAgent.SyncAsync(MatchWith(MatchComparisonResultType.Conflict), CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NotSupportedException>(act))
                .Message.ShouldBe($"Not supported {nameof(SyncMatchOperation)} '99' for {MatchComparisonResultType.Conflict} matches.");
        }

        [Theory]
        [InlineData(MatchComparisonResultType.NewerSource)]
        [InlineData(MatchComparisonResultType.NewerDestination)]
        public async Task Sync_UnsupportedNewerMatchesOperation(MatchComparisonResultType comparisonResult)
        {
            // The newer branch used to report SameMatches, and to say "for Same matches".
            var syncAgent = CreateAgent(m => m.NewerMatches = (SyncMatchOperation)99);

            Func<Task> act = async () => await syncAgent.SyncAsync(MatchWith(comparisonResult), CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NotSupportedException>(act))
                .Message.ShouldBe($"Not supported {nameof(SyncMatchOperation)} '99' for newer matches.");
        }
    }
}
