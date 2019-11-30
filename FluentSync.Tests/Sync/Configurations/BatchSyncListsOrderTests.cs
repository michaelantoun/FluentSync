using FluentAssertions;
using FluentSync.Sync.Configurations;
using Xunit;

namespace FluentSync.Tests.Sync.Configurations
{
    public class BatchSyncListsOrderTests
    {
        [Fact]
        public void BatchSyncListsOrderDefaults()
        {
            var syncOperationsOrder = new BatchSyncListsOrder();

            syncOperationsOrder.HasDuplicates().Should().BeFalse();

            syncOperationsOrder.Order.Should().NotBeNull();
            syncOperationsOrder.Order.Length.Should().Be(3);
            syncOperationsOrder.Order[0].Should().Be(BatchSyncListType.ItemsInSourceOnly);
            syncOperationsOrder.Order[1].Should().Be(BatchSyncListType.ItemsInDestinationOnly);
            syncOperationsOrder.Order[2].Should().Be(BatchSyncListType.Matches);

            syncOperationsOrder.ToString().Should().Be($"First: {BatchSyncListType.ItemsInSourceOnly}, Second: {BatchSyncListType.ItemsInDestinationOnly}, Finally: {BatchSyncListType.Matches}");
        }

        [Theory]
        [InlineData(BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInDestinationOnly)]
        [InlineData(BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInDestinationOnly, BatchSyncListType.ItemsInSourceOnly)]
        [InlineData(BatchSyncListType.ItemsInDestinationOnly, BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInSourceOnly)]
        [InlineData(BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInSourceOnly, BatchSyncListType.ItemsInSourceOnly)]
        public void BatchSyncListsOrderCannotHaveDuplicates(BatchSyncListType firstOperation, BatchSyncListType secondOperation, BatchSyncListType thirdOperation)
        {
            var syncOperationsOrder = new BatchSyncListsOrder();

            syncOperationsOrder.Order[0] = firstOperation;
            syncOperationsOrder.Order[1] = secondOperation;
            syncOperationsOrder.Order[2] = thirdOperation;

            syncOperationsOrder.HasDuplicates().Should().BeTrue();
        }

        [Theory]
        [InlineData(BatchSyncListType.Matches, BatchSyncListType.ItemsInDestinationOnly, BatchSyncListType.ItemsInSourceOnly)]
        public void BatchSyncListsOrderCanHaveCustomOrder(BatchSyncListType firstOperation, BatchSyncListType secondOperation, BatchSyncListType thirdOperation)
        {
            var syncOperationsOrder = new BatchSyncListsOrder();

            syncOperationsOrder.Order[0] = firstOperation;
            syncOperationsOrder.Order[1] = secondOperation;
            syncOperationsOrder.Order[2] = thirdOperation;

            syncOperationsOrder.HasDuplicates().Should().BeFalse();
        }
    }
}
