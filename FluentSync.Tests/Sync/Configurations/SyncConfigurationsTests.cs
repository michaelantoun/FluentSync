using FluentAssertions;
using FluentSync.Sync.Configurations;
using System;
using Xunit;

namespace FluentSync.Tests.Sync.Configurations
{
    public class SyncConfigurationsTests
    {
        [Fact]
        public void SyncConfigurationsShouldHaveDefaultConfigurations()
        {
            var syncConfigurations = new SyncConfigurations();

            syncConfigurations.SyncMode.Should().NotBeNull();
            syncConfigurations.SyncOperationsOrder.Should().NotBeNull();
        }

        [Fact]
        public void SyncConfigurationsWithDuplicateSyncOperationsThrowsException()
        {
            var syncConfigurations = new SyncConfigurations();

            syncConfigurations.SyncOperationsOrder.Order[0] = SyncOperationType.Insert;
            syncConfigurations.SyncOperationsOrder.Order[1] = SyncOperationType.Insert;

            Action act = () => syncConfigurations.Validate();

            act.Should().Throw<ArgumentException>().WithMessage($"Cannot have duplicates in the {nameof(syncConfigurations.SyncOperationsOrder)} array.");
        }
    }
}
