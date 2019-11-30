using FluentAssertions;
using FluentSync.Comparers.Configurations;
using Xunit;

namespace FluentSync.Tests.Comparers.Configurations
{
    public class ComparerConfigurationsTests
    {
        [Fact]
        public void ComparerConfigurationsShouldHaveValidString()
        {
            var configurations = new ComparerConfigurations { AllowDuplicateKeys = RuleAllowanceType.Source, AllowNullableItems = RuleAllowanceType.Destination, AllowDuplicateItems = RuleAllowanceType.None };

            configurations.ToString().Should().Be($"{nameof(configurations.AllowDuplicateKeys)}: {configurations.AllowDuplicateKeys}, {nameof(configurations.AllowDuplicateItems)}: {configurations.AllowDuplicateItems}, {nameof(configurations.AllowNullableItems)}: {configurations.AllowNullableItems}");
        }
    }
}
