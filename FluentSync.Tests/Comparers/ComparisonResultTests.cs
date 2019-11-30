using FluentAssertions;
using FluentSync.Comparers;
using Xunit;

namespace FluentSync.Tests.Comparers
{
    public class ComparisonResultTests
    {
        [Fact]
        public void ComparisonResultShouldHaveValidString()
        {
            var comparisonResult = new ComparisonResult<int>();

            comparisonResult.ItemsInSourceOnly.Add(1);

            comparisonResult.ItemsInDestinationOnly.AddRange(new int[] { 2, 3 });

            comparisonResult.ToString().Should().Be($"{nameof(comparisonResult.ItemsInSourceOnly)}: {comparisonResult.ItemsInSourceOnly.Count}, {nameof(comparisonResult.ItemsInDestinationOnly)}: {comparisonResult.ItemsInDestinationOnly.Count}, {nameof(comparisonResult.Matches)}: {comparisonResult.Matches.Count}");
        }
    }
}
