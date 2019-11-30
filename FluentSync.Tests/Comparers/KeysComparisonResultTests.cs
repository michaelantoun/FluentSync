using FluentAssertions;
using FluentSync.Comparers;
using Xunit;

namespace FluentSync.Tests.Comparers
{
    public class KeysComparisonResultTests
    {
        [Fact]
        public void KeysComparisonResultShouldHaveValidString()
        {
            var keysComparisonResult = new KeysComparisonResult<int>();

            keysComparisonResult.KeysInSourceOnly.Add(1);

            keysComparisonResult.KeysInDestinationOnly.Add(2);
            keysComparisonResult.KeysInDestinationOnly.Add(3);

            keysComparisonResult.ToString().Should().Be($"{nameof(keysComparisonResult.KeysInSourceOnly)}: {keysComparisonResult.KeysInSourceOnly.Count}, {nameof(keysComparisonResult.KeysInDestinationOnly)}: {keysComparisonResult.KeysInDestinationOnly.Count}, {nameof(keysComparisonResult.Matches)}: {keysComparisonResult.Matches.Count}");
        }
    }
}
