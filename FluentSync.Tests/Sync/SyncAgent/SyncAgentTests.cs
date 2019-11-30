using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.SyncAgent
{
    public partial class SyncAgentTests
    {
        [Fact]
        public void Sync_ComparerAgentNotSet()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} cannot be null.");
        }

        [Fact]
        public void Sync_SourceSyncProviderNotSet()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.SourceProvider)} cannot be null.");
        }

        [Fact]
        public void Sync_DestinationSyncProviderNotSet()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.DestinationProvider)} cannot be null.");
        }

        [Fact]
        public void Sync_SourceComparerSyncProviderIsSetWithoutSettingComparerAgent()
        {
            ListSyncProvider<int> source = new ListSyncProvider<int> { Items = new List<int> { 5, 4, 9 } }
                , destination = new ListSyncProvider<int> { Items = new List<int> { 6, 10, 5 } };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_DestinationComparerSyncProviderIsSetWithoutSettingComparerAgent()
        {
            ListSyncProvider<int> source = new ListSyncProvider<int> { Items = new List<int> { 5, 4, 9 } }
                , destination = new ListSyncProvider<int> { Items = new List<int> { 6, 10, 5 } };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination)
                .SetDestinationProvider(destination)
                .SetSourceProvider(source)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} must be set first.");
        }

        #region Set Source/Destination lists

        [Fact]
        public void Sync_List_SourceSyncProviderIsSetToNullableList()
        {
            List<int> source = null
                , destination = new List<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage("The source items cannot be null.");
        }

        [Fact]
        public void Sync_List_SourceSyncProviderMustBeSetAfterSettingComparer()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetSourceProvider(source)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_List_DestinationSyncProviderMustBeSetAfterSettingComparerAgent()
        {
            List<int> source = new List<int> { 5, 4, 9 }
                , destination = new List<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetDestinationProvider(destination)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_List_DestinationSyncProviderIsSetToNullableList()
        {
            List<int> source = new List<int> { 6, 10, 5 }
                , destination = null;

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage("The destination items cannot be null.");
        }

        [Fact]
        public async Task SyncAgentWithValidSyncProvidersShouldSync()
        {
            ListSyncProvider<int> source = new ListSyncProvider<int> { Items = new List<int> { 5, 4, 9 } }
                , destination = new ListSyncProvider<int> { Items = new List<int> { 6, 10, 5 } };

            var syncAgent = SyncAgent<int>.Create()
                .Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination);

            await syncAgent.SyncAsync(CancellationToken.None).ConfigureAwait(false);

            source.Items.Should().BeEquivalentTo(new List<int> { 5, 4, 9 });
            destination.Items.Should().BeEquivalentTo(source.Items);

            syncAgent.ToString().Should().Be($"{nameof(syncAgent.Configurations)}: {{{syncAgent.Configurations}}}");
        }

        #endregion

        #region Set Source/Destination SortedSets

        [Fact]
        public void Sync_SortedSet_SourceSyncProviderIsSetToNullableSet()
        {
            SortedSet<int> source = null
                , destination = new SortedSet<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage("The source items cannot be null.");
        }

        [Fact]
        public void Sync_SortedSet_SourceSyncProviderMustBeSetAfterSettingComparer()
        {
            SortedSet<int> source = new SortedSet<int> { 5, 4, 9 }
                , destination = new SortedSet<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetSourceProvider(source)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_SortedSet_DestinationSyncProviderMustBeSetAfterSettingComparerAgent()
        {
            SortedSet<int> source = new SortedSet<int> { 5, 4, 9 }
                , destination = new SortedSet<int> { 6, 10, 5 };

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetDestinationProvider(destination)
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage($"The {nameof(SyncAgent<int>.ComparerAgent)} must be set first.");
        }

        [Fact]
        public void Sync_SortedSet_DestinationSyncProviderIsSetToNullableSet()
        {
            SortedSet<int> source = new SortedSet<int> { 6, 10, 5 }
                , destination = null;

            Func<Task> act = async () => await SyncAgent<int>.Create()
                .SetComparerAgent(ComparerAgent<int>.Create())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .SyncAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<NullReferenceException>().WithMessage("The destination items cannot be null.");
        }

        #endregion

    }
}
