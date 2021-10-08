using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using FluentSync.Tests.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.BatchSyncAgent
{
    public partial class BatchSyncAgentTests
    {
        [Fact]
        public void Sync_Class_NullableKeySelector()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.KeySelector)} cannot be null.");
        }

        [Fact]
        public void Sync_Class_NullableCompareItemFunc()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetKeySelector(x => x.Id)
                .SetSourceProvider(CreateSourceEventDictionary())
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.CompareItemFunc)} cannot be null.");
        }

        [Fact]
        public void Sync_Class_SourceProviderIsNotSet()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.SourceProvider)} cannot be null.");
        }

        [Fact]
        public void Sync_Class_DestinationProviderIsNotSet()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetSourceProvider(CreateSourceEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.DestinationProvider)} cannot be null.");
        }

        [Fact]
        public void Sync_Class_SourceProviderIsSetBeforeKeySelector()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetSourceProvider(CreateSourceEventDictionary())
                .SetKeySelector(x => x.Id)
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.KeySelector)} must be set first.");
        }

        [Fact]
        public void Sync_Class_SourceProviderIsSetBeforeComparerAgent()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetSourceProvider(CreateSourceEventDictionary())
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_Class_DestinationProviderIsSetBeforeKeySelector()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetSourceProvider(CreateSourceEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.KeySelector)} must be set first.");
        }

        [Fact]
        public void Sync_Class_DestinationProviderIsSetBeforeComparerAgent()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetSourceProvider(CreateSourceEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_Class_SourceProviderIsSetToNull()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetSourceProvider((IDictionary<int?, Event>)null)
                .SetDestinationProvider(CreateDestinationEventDictionary())
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage("The source items cannot be null.");
        }

        [Fact]
        public void Sync_Class_DestinationProviderIsSetToNull()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SetComparerAgent(KeyComparerAgent<int?>.Create())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => MatchComparisonResultType.Conflict)
                .SetSourceProvider(CreateSourceEventDictionary())
                .SetDestinationProvider((IDictionary<int?, Event>)null)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage("The destination items cannot be null.");
        }

        [Fact]
        public void Sync_Class_NullableComparerAgent()
        {
            Func<Task> act = async () => await BatchSyncAgent<int?, Event>.Create()
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.ComparerAgent)} cannot be null.");
        }

        [Fact]
        public void Sync_Class_SourceComparerBatchSyncProviderIsSetWithoutSettingComparerAgent()
        {
            var syncAgent = CreateSyncAgent();
            DictionaryBatchSyncProvider<int?, Event> source = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateSourceEventDictionary(), KeySelector = syncAgent.KeySelector }
                , destination = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateDestinationEventDictionary(), KeySelector = syncAgent.KeySelector };

            Func<Task> act = async () => await syncAgent
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.TwoWay)
                .SetComparerAgent(null)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_Class_DestinationComparerBatchSyncProviderIsSetWithoutSettingComparerAgent()
        {
            var syncAgent = CreateSyncAgent();
            DictionaryBatchSyncProvider<int?, Event> source = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateSourceEventDictionary(), KeySelector = syncAgent.KeySelector }
                , destination = new DictionaryBatchSyncProvider<int?, Event> { Items = CreateDestinationEventDictionary(), KeySelector = syncAgent.KeySelector };

            Func<Task> act = async () => await syncAgent
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.TwoWay)
                .SetComparerAgent(null)
                .SetDestinationProvider(destination)
                .SetSourceProvider(source)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(BatchSyncAgent<int?, Event>.ComparerAgent)} must be set first.");
        }
    }
}
