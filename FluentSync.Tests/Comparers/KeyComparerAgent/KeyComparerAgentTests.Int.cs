using FluentAssertions;
using FluentSync.Comparers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            keysComparisonResult.KeysInSourceOnly.Should().BeEquivalentTo(new List<int> { 20 });
            keysComparisonResult.KeysInDestinationOnly.Should().BeEquivalentTo(new List<int> { 25 });
            keysComparisonResult.Matches.Should().BeEquivalentTo(new List<int> { 5, 10 });
        }

        [Fact]
        public async Task Compare_NullableInt_NonEmptyLists()
        {
            List<int?> source = new List<int?> { 10, 5, 20 }
                , destination = new List<int?> { 10, 25, 5 };

            var keysComparisonResult = await KeyComparerAgent<int?>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            keysComparisonResult.KeysInSourceOnly.Should().BeEquivalentTo(new List<int?> { 20 });
            keysComparisonResult.KeysInDestinationOnly.Should().BeEquivalentTo(new List<int?> { 25 });
            keysComparisonResult.Matches.Should().BeEquivalentTo(new List<int?> { 5, 10 });
        }
    }
}
