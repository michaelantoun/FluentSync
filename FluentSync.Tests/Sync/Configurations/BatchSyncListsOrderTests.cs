using FluentSync.Sync.Configurations;

namespace FluentSync.Tests.Sync.Configurations
{
    public class BatchSyncListsOrderTests
    {
        [Fact]
        public void BatchSyncListsOrderDefaults()
        {
            var syncOperationsOrder = new BatchSyncListsOrder();

            syncOperationsOrder.HasDuplicates().ShouldBeFalse();

            syncOperationsOrder.Order.ShouldNotBeNull();
            syncOperationsOrder.Order.Length.ShouldBe(3);
            syncOperationsOrder.Order[0].ShouldBe(BatchSyncListType.ItemsInSourceOnly);
            syncOperationsOrder.Order[1].ShouldBe(BatchSyncListType.ItemsInDestinationOnly);
            syncOperationsOrder.Order[2].ShouldBe(BatchSyncListType.Matches);

            syncOperationsOrder.ToString().ShouldBe($"First: {BatchSyncListType.ItemsInSourceOnly}, Second: {BatchSyncListType.ItemsInDestinationOnly}, Finally: {BatchSyncListType.Matches}");
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

            syncOperationsOrder.HasDuplicates().ShouldBeTrue();
        }

        [Theory]
        [InlineData(BatchSyncListType.Matches, BatchSyncListType.ItemsInDestinationOnly, BatchSyncListType.ItemsInSourceOnly)]
        public void BatchSyncListsOrderCanHaveCustomOrder(BatchSyncListType firstOperation, BatchSyncListType secondOperation, BatchSyncListType thirdOperation)
        {
            var syncOperationsOrder = new BatchSyncListsOrder();

            syncOperationsOrder.Order[0] = firstOperation;
            syncOperationsOrder.Order[1] = secondOperation;
            syncOperationsOrder.Order[2] = thirdOperation;

            syncOperationsOrder.HasDuplicates().ShouldBeFalse();
        }
    }
}
