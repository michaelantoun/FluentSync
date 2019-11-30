using FluentAssertions;
using FluentSync.Sync.Providers;
using System.Collections.Generic;
using Xunit;

namespace FluentSync.Tests.Sync.Providers
{
    public class ListSyncProviderTests
    {
        [Fact]
        public void ListSyncProviderShouldHaveAValidStringForNullableItems()
        {
            var provider = new ListSyncProvider<int>();

            provider.ToString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ListSyncProviderShouldHaveAValidStringForNonNullableItems()
        {
            var provider = new ListSyncProvider<int>();

            provider.Items = new List<int>();
            provider.Items.Add(1);

            provider.ToString().Should().Be(provider.Items.ToString());
        }
    }
}
