namespace FluentSync.Tests.Comparers.BatchComparerAgent
{
    public partial class KeyComparerAgentTests
    {
        #region Missing configurations

        [Fact]
        public async Task Compare_Int_NoSourceProvider()
        {
            List<int> destination = new List<int>();

            Func<Task> act = async () => await KeyComparerAgent<int>.Create()
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NullReferenceException>(act)).Message.ShouldBe($"The {nameof(ComparerAgent<int>.SourceProvider)} cannot be null.");
        }

        [Fact]
        public async Task Compare_Int_NoDestinationProvider()
        {
            List<int> source = new List<int>();

            Func<Task> act = async () => await KeyComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NullReferenceException>(act)).Message.ShouldBe($"The {nameof(ComparerAgent<int>.DestinationProvider)} cannot be null.");
        }

        #endregion

        #region Compare null-able/empty lists

        [Fact]
        public async Task Compare_Int_EmptyLists()
        {
            List<int> source = new List<int>()
                , destination = new List<int>();

            var comparisonResult = await KeyComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            comparisonResult.KeysInSourceOnly.ShouldBeEmpty();
            comparisonResult.KeysInDestinationOnly.ShouldBeEmpty();
            comparisonResult.Matches.ShouldBeEmpty();
        }

        [Fact]
        public async Task Compare_Int_EmptySourceList()
        {
            List<int> source = new List<int>()
                , destination = new List<int> { 1 };

            var comparisonResult = await KeyComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            comparisonResult.KeysInSourceOnly.ShouldBeEmpty();
            comparisonResult.KeysInDestinationOnly.ShouldBeEquivalentToIgnoringOrder(new List<int> { 1 });
            comparisonResult.Matches.ShouldBeEmpty();
        }

        [Fact]
        public async Task Compare_Int_EmptyDestinationList()
        {
            List<int> source = new List<int> { 1 }
                , destination = new List<int>();

            var comparisonResult = await KeyComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            comparisonResult.KeysInSourceOnly.ShouldBeEquivalentToIgnoringOrder(new List<int> { 1 });
            comparisonResult.KeysInDestinationOnly.ShouldBeEmpty();
            comparisonResult.Matches.ShouldBeEmpty();
        }

        [Fact]
        public async Task Compare_Int_NullableSourceList()
        {
            List<int> source = null
                , destination = new List<int>();

            var comparisonResult = await KeyComparerAgent<int>.Create()
                    .SetSourceProvider(source)
                    .SetDestinationProvider(destination)
                    .CompareAsync(CancellationToken.None);

            comparisonResult.KeysInSourceOnly.ShouldBeEmpty();
            comparisonResult.KeysInDestinationOnly.ShouldBeEmpty();
            comparisonResult.Matches.ShouldBeEmpty();
        }

        [Fact]
        public async Task Compare_Int_NullableDestinationList()
        {
            List<int> source = new List<int>()
                , destination = null;

            var comparisonResult = await KeyComparerAgent<int>.Create()
                    .SetSourceProvider(source)
                    .SetDestinationProvider(destination)
                    .CompareAsync(CancellationToken.None);

            comparisonResult.KeysInSourceOnly.ShouldBeEmpty();
            comparisonResult.KeysInDestinationOnly.ShouldBeEmpty();
            comparisonResult.Matches.ShouldBeEmpty();
        }

        #endregion

        #region No duplicates or null-able keys

        [Fact]
        public async Task Compare_Int_NullableKeyInSource()
        {
            List<int?> source = new List<int?> { 1, null }
                , destination = new List<int?>();

            Func<Task> act = async () => await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NullReferenceException>(act)).Message.ShouldBe("Null-able keys found in the source list.");
        }

        [Fact]
        public async Task Compare_Int_NullableKeyInDestination()
        {
            List<int?> source = new List<int?>()
                , destination = new List<int?> { null, 1 };

            Func<Task> act = async () => await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<NullReferenceException>(act)).Message.ShouldBe("Null-able keys found in the destination list.");
        }

        [Fact]
        public async Task Compare_Int_DuplicatedKeyInSource()
        {
            List<int?> source = new List<int?> { 1, 1 }
                , destination = new List<int?> { 2, 3 };

            Func<Task> act = async () => await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<ArgumentException>(act)).Message.ShouldBe("Key '1' already exists in the source list.");
        }

        [Fact]
        public async Task Compare_Int_DuplicatedKeyInDestination()
        {
            List<int?> source = new List<int?> { 1 }
                , destination = new List<int?> { 3, 2, 2 };

            Func<Task> act = async () => await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            (await Should.ThrowAsync<ArgumentException>(act)).Message.ShouldBe("Key '2' already exists in the destination list.");
        }

        #endregion
    }
}
