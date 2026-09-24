namespace FluentSync.Tests
{
    public class MatchComparisonResultTests
    {
        [Fact]
        public void MatchComparisonResultShouldHaveAValidString()
        {
            var matchComparisonResult = new MatchComparisonResult<int> { ComparisonResult = MatchComparisonResultType.Same };

            matchComparisonResult.ToString().ShouldBe($"{nameof(matchComparisonResult.ComparisonResult)}: {matchComparisonResult.ComparisonResult}");
        }
    }
}
