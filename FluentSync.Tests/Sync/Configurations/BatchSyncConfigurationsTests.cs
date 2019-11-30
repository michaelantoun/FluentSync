using FluentAssertions;
using FluentSync.Sync.Configurations;
using System;
using Xunit;

namespace FluentSync.Tests.Sync.Configurations
{
    public class BatchSyncConfigurationsTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void BatchSyncConfigurationsMustHaveBatchSizeGreaterThanZero(int batchSize)
        {
            var syncConfigurations = new BatchSyncConfigurations();

            Action act = () => syncConfigurations.BatchSize = batchSize;

            act.Should().Throw<ArgumentException>().WithMessage("Batch size must be greater than 0.");
        }

        [Fact]
        public void BatchSyncConfigurationsWithDuplicateSyncOperationsThrowsException()
        {
            var syncConfigurations = new BatchSyncConfigurations();

            syncConfigurations.SyncOperationsOrder.Order[0] = SyncOperationType.Insert;
            syncConfigurations.SyncOperationsOrder.Order[1] = SyncOperationType.Insert;

            Action act = () => syncConfigurations.Validate();

            act.Should().Throw<ArgumentException>().WithMessage($"Cannot have duplicates in the {nameof(syncConfigurations.SyncOperationsOrder)} array.");
        }

        [Fact]
        public void BatchSyncConfigurationsWithDuplicateSyncListsThrowsException()
        {
            var syncConfigurations = new BatchSyncConfigurations();

            syncConfigurations.BatchSyncListsOrder.Order[0] = BatchSyncListType.ItemsInDestinationOnly;
            syncConfigurations.BatchSyncListsOrder.Order[1] = BatchSyncListType.ItemsInDestinationOnly;

            Action act = () => syncConfigurations.Validate();

            act.Should().Throw<ArgumentException>().WithMessage($"Cannot have duplicates in the {nameof(syncConfigurations.BatchSyncListsOrder)} array.");
        }

        [Fact]
        public void BatchSyncConfigurationsShouldHaveDefaultConfigurations()
        {
            var syncConfigurations = new BatchSyncConfigurations();

            syncConfigurations.SyncMode.Should().NotBeNull();
            syncConfigurations.SyncOperationsOrder.Should().NotBeNull();
            syncConfigurations.BatchSyncListsOrder.Should().NotBeNull();
            syncConfigurations.BatchSize.Should().BeGreaterThan(0);

            syncConfigurations.ToString().Should().Be($"{nameof(syncConfigurations.SyncMode)}: {{{syncConfigurations.SyncMode}}}, {nameof(syncConfigurations.SyncOperationsOrder)}: {{{syncConfigurations.SyncOperationsOrder}}}, {nameof(syncConfigurations.BatchSyncListsOrder)}: {{{syncConfigurations.BatchSyncListsOrder}}}, {nameof(syncConfigurations.BatchSize)}: {syncConfigurations.BatchSize}");
        }
    }
}
