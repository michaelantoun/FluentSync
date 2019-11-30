using FluentAssertions;
using FluentSync.Sync.Configurations;
using Xunit;

namespace FluentSync.Tests.Sync.Configurations
{
    public class SyncModeTests
    {
        [InlineData(SyncModePreset.None)]
        [InlineData(SyncModePreset.TwoWay)]
        [InlineData(SyncModePreset.MirrorToSource)]
        [InlineData(SyncModePreset.MirrorToDestination)]
        [InlineData(SyncModePreset.UpdateSource)]
        [InlineData(SyncModePreset.UpdateDestination)]
        [Theory]
        public void SyncMode_VerifyGetterAndSetter(SyncModePreset syncModePreset)
        {
            var syncMode = new SyncMode { SyncModePreset = syncModePreset };
            syncMode.SyncModePreset.Should().Be(syncModePreset);
            syncMode.ToString().Should().Be($"{nameof(SyncMode.SyncModePreset)}: {syncMode.SyncModePreset}");
        }

        [Fact]
        public void SyncMode_VerifyCustomPreset()
        {
            var syncMode = new SyncMode { ItemsInSourceOnly = SyncItemOperation.Delete, SameMatches = SyncMatchOperation.UpdateOldItem };
            syncMode.SyncModePreset.Should().Be(SyncModePreset.Custom);
            syncMode.ToString().Should().Be($"{nameof(SyncMode.SyncModePreset)}: {syncMode.SyncModePreset}, {nameof(SyncMode.ItemsInSourceOnly)}: {syncMode.ItemsInSourceOnly}, {nameof(SyncMode.ItemsInDestinationOnly)}: {syncMode.ItemsInDestinationOnly}, {nameof(SyncMode.SameMatches)}: {syncMode.SameMatches}, {nameof(SyncMode.NewerMatches)}: {syncMode.NewerMatches}, {nameof(SyncMode.ConflictMatches)}: {syncMode.ConflictMatches}");
        }
    }
}
