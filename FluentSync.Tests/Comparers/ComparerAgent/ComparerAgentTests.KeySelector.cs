using FluentAssertions;
using FluentSync.Comparers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.ComparerAgent
{
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_String_WithCaseInsensitiveKeySelector_AndNullableItems()
        {
            List<string> source = new List<string> { null, "Tim", "bob", null }
                , destination = new List<string> { "Bob", null, "Tim" };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetKeySelector(x => x?.ToLower())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { null });
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
            {
                new MatchComparisonResult<string>{Source = null, Destination = null, ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<string>{Source = "Tim", Destination = "Tim", ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<string>{Source = "bob", Destination = "Bob", ComparisonResult = MatchComparisonResultType.Conflict}
            });
        }

        [Fact]
        public async Task Compare_String_WithCaseInsensitiveKeySelector_AndOneNullableItemOnly()
        {
            List<string> source = new List<string> { null }
                , destination = new List<string> { null };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetKeySelector(x => x?.ToLower())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
            {
                new MatchComparisonResult<string>{Source = null, Destination = null, ComparisonResult = MatchComparisonResultType.Same}
            });
        }

        [Fact]
        public async Task Compare_String_WithCaseInsensitiveKeySelector_AndTwoNullableItemsOnly()
        {
            List<string> source = new List<string> { null, null }
                , destination = new List<string> { null, null };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetKeySelector(x => x?.ToLower())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
            {
                new MatchComparisonResult<string>{Source = null, Destination = null, ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<string>{Source = null, Destination = null, ComparisonResult = MatchComparisonResultType.Same}
            });
        }

        [Fact]
        public async Task Compare_String_WithCaseInsensitiveKeySelector_AndNullableItemsInSource()
        {
            List<string> source = new List<string> { null }
                , destination = new List<string> { "Tom" };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetKeySelector(x => x?.ToLower())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { null });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { "Tom" });

            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_String_WithInsensitiveKeySelector_AndNullableItemsInDestination()
        {
            List<string> source = new List<string> { "Tom" }
                , destination = new List<string> { null };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetKeySelector(x => x?.ToLower())
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { "Tom" });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { null });

            comparisonResult.Matches.Should().BeEmpty();
        }
    }
}
