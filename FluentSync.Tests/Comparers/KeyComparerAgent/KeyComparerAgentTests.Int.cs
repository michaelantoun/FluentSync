namespace FluentSync.Tests.Comparers.KeyComparerAgent
{
    public partial class KeyComparerAgentTests
    {
        [Fact]
        public async Task Compare_Int_NonEmptyLists()
        {
            List<int> source = new List<int> { 10, 5, 20 }
                , destination = new List<int> { 10, 25, 5 };

            var keysComparisonResult = await KeyComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            keysComparisonResult.KeysInSourceOnly.ShouldBeEquivalentToIgnoringOrder(new List<int> { 20 });
            keysComparisonResult.KeysInDestinationOnly.ShouldBeEquivalentToIgnoringOrder(new List<int> { 25 });
            keysComparisonResult.Matches.ShouldBeEquivalentToIgnoringOrder(new List<int> { 5, 10 });
        }

        [Fact]
        public async Task Compare_NullableInt_NonEmptyLists()
        {
            List<int?> source = new List<int?> { 10, 5, 20 }
                , destination = new List<int?> { 10, 25, 5 };

            var keysComparisonResult = await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            keysComparisonResult.KeysInSourceOnly.ShouldBeEquivalentToIgnoringOrder(new List<int?> { 20 });
            keysComparisonResult.KeysInDestinationOnly.ShouldBeEquivalentToIgnoringOrder(new List<int?> { 25 });
            keysComparisonResult.Matches.ShouldBeEquivalentToIgnoringOrder(new List<int?> { 5, 10 });
        }
    }
}
