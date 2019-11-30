using FluentAssertions;
using FluentSync.Sync.Configurations;
using Xunit;

namespace FluentSync.Tests.Sync.Configurations
{
    public class SyncOperationsOrderTests
    {
        [Fact]
        public void SyncOperationsOrderDefaults()
        {
            var syncOperationsOrder = new SyncOperationsOrder();

            syncOperationsOrder.HasDuplicates().Should().BeFalse();

            syncOperationsOrder.Order.Should().NotBeNull();
            syncOperationsOrder.Order.Length.Should().Be(3);
            syncOperationsOrder.Order[0].Should().Be(SyncOperationType.Delete);
            syncOperationsOrder.Order[1].Should().Be(SyncOperationType.Update);
            syncOperationsOrder.Order[2].Should().Be(SyncOperationType.Insert);

            syncOperationsOrder.ToString().Should().Be($"First: {SyncOperationType.Delete}, Second: {SyncOperationType.Update}, Finally: {SyncOperationType.Insert}");
        }

        [Theory]
        [InlineData(SyncOperationType.Delete, SyncOperationType.Delete, SyncOperationType.Update)]
        [InlineData(SyncOperationType.Delete, SyncOperationType.Update, SyncOperationType.Delete)]
        [InlineData(SyncOperationType.Update, SyncOperationType.Delete, SyncOperationType.Delete)]
        [InlineData(SyncOperationType.Delete, SyncOperationType.Delete, SyncOperationType.Delete)]
        public void SyncOperationsOrderCannotHaveDuplicates(SyncOperationType firstOperation, SyncOperationType secondOperation, SyncOperationType thirdOperation)
        {
            var syncOperationsOrder = new SyncOperationsOrder();

            syncOperationsOrder.Order[0] = firstOperation;
            syncOperationsOrder.Order[1] = secondOperation;
            syncOperationsOrder.Order[2] = thirdOperation;

            syncOperationsOrder.HasDuplicates().Should().BeTrue();
        }

        [Theory]
        [InlineData(SyncOperationType.Insert, SyncOperationType.Update, SyncOperationType.Delete)]
        public void SyncOperationsOrderCanHaveCustomOrder(SyncOperationType firstOperation, SyncOperationType secondOperation, SyncOperationType thirdOperation)
        {
            var syncOperationsOrder = new SyncOperationsOrder();

            syncOperationsOrder.Order[0] = firstOperation;
            syncOperationsOrder.Order[1] = secondOperation;
            syncOperationsOrder.Order[2] = thirdOperation;

            syncOperationsOrder.HasDuplicates().Should().BeFalse();
        }
    }
}
