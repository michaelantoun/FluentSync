using FluentSync.Sync.Configurations;

namespace FluentSync.Tests.Sync.Configurations
{
    public class SyncOperationsOrderTests
    {
        [Fact]
        public void SyncOperationsOrderDefaults()
        {
            var syncOperationsOrder = new SyncOperationsOrder();

            syncOperationsOrder.HasDuplicates().ShouldBeFalse();

            syncOperationsOrder.Order.ShouldNotBeNull();
            syncOperationsOrder.Order.Length.ShouldBe(3);
            syncOperationsOrder.Order[0].ShouldBe(SyncOperationType.Delete);
            syncOperationsOrder.Order[1].ShouldBe(SyncOperationType.Update);
            syncOperationsOrder.Order[2].ShouldBe(SyncOperationType.Insert);

            syncOperationsOrder.ToString().ShouldBe($"First: {SyncOperationType.Delete}, Second: {SyncOperationType.Update}, Finally: {SyncOperationType.Insert}");
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

            syncOperationsOrder.HasDuplicates().ShouldBeTrue();
        }

        [Theory]
        [InlineData(SyncOperationType.Insert, SyncOperationType.Update, SyncOperationType.Delete)]
        public void SyncOperationsOrderCanHaveCustomOrder(SyncOperationType firstOperation, SyncOperationType secondOperation, SyncOperationType thirdOperation)
        {
            var syncOperationsOrder = new SyncOperationsOrder();

            syncOperationsOrder.Order[0] = firstOperation;
            syncOperationsOrder.Order[1] = secondOperation;
            syncOperationsOrder.Order[2] = thirdOperation;

            syncOperationsOrder.HasDuplicates().ShouldBeFalse();
        }
    }
}
