using FluentAssertions;
using Xunit;

namespace FluentSync.Tests
{
    public class ValueTypeExtensionsTests
    {
        [Theory]
        [InlineData(null, null, 0)]
        [InlineData(null, 1, -1)]
        [InlineData(1, null, 1)]
        [InlineData(2, 2, 0)]
        [InlineData(2, 3, -1)]
        [InlineData(3, 2, 1)]
        public void ValueTypeCompareMethodShouldReturnValidNumber(int? x, int? y, int expectedResult)
        {
            ValueTypeExtensions.CompareTo(x, y).Should().Be(expectedResult);
        }
    }
}
