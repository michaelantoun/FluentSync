namespace FluentSync.Tests
{
    public class MatchComparisonResultTests
    {
        [Fact]
        public void MatchComparisonResultShouldHaveAValidString()
        {
            var matchComparisonResult = new MatchComparisonResult<int> { ComparisonResult = MatchComparisonResultType.Same };

            matchComparisonResult.ToString().Should().Be($"{nameof(matchComparisonResult.ComparisonResult)}: {matchComparisonResult.ComparisonResult}");
        }
    }
}
