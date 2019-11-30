using FluentAssertions;
using FluentSync.Comparers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.ComparerAgent
{
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_String_WithCaseInsensitiveEqualityComparer()
        {
            List<string> source = new List<string> { "Tom", "Tim", "bob", "Zoo" }
                , destination = new List<string> { "Bob", "Sam", "Tim" };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetKeySelector(x => x?.ToLower())
                .SetCompareItemFunc((s, d) => string.Equals(s, d, StringComparison.OrdinalIgnoreCase) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { "Tom", "Zoo" });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { "Sam" });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
            {
                new MatchComparisonResult<string>{Source = "Tim", Destination = "Tim", ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<string>{Source = "bob", Destination = "Bob", ComparisonResult = MatchComparisonResultType.Same}
            });
        }

        [Fact]
        public async Task Compare_String_WithCaseInsensitiveCompareItemFunc_AndOneNullableItemOnly()
        {
            List<string> source = new List<string> { null }
                , destination = new List<string> { null };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetCompareItemFunc((s, d) => string.Equals(s, d, StringComparison.OrdinalIgnoreCase) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
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
        public async Task Compare_String_WithCaseInsensitiveCompareItemFunc_AndTwoNullableItemsOnly()
        {
            List<string> source = new List<string> { null, null }
                , destination = new List<string> { null, null };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetCompareItemFunc((s, d) => string.Equals(s, d, StringComparison.OrdinalIgnoreCase) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
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
        public async Task Compare_String_WithCaseInsensitiveCompareItemFunc_AndNullableItemsInSource()
        {
            List<string> source = new List<string> { null }
                , destination = new List<string> { "Tom" };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetCompareItemFunc((s, d) => string.Equals(s, d, StringComparison.OrdinalIgnoreCase) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { null });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { "Tom" });
            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_String_WithCaseInsensitiveCompareItemFunc_AndNullableItemsInDestination()
        {
            List<string> source = new List<string> { "Tom" }
                , destination = new List<string> { null };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetCompareItemFunc((s, d) => string.Equals(s, d, StringComparison.OrdinalIgnoreCase) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { "Tom" });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { null });
            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_String_WithCaseInsensitiveCompareItemFunc_AndNullableItems()
        {
            List<string> source = new List<string> { null, "Tim", "bob", null }
                , destination = new List<string> { "Bob", null, "Tim" };

            var comparisonResult = await ComparerAgent<string>.Create()
                .SetCompareItemFunc((s, d) => string.Equals(s, d, StringComparison.OrdinalIgnoreCase) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { null, "bob" });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { "Bob" });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
            {
                new MatchComparisonResult<string>{Source = "Tim", Destination = "Tim", ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<string>{Source = null, Destination = null, ComparisonResult = MatchComparisonResultType.Same}
            });
        }
    }
}
