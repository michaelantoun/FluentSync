using FluentAssertions;
using FluentSync.Sync.Providers;
using System.Collections.Generic;
using Xunit;

namespace FluentSync.Tests.Sync.Providers
{
    public class SortedSetSyncProviderTests
    {
        [Fact]
        public void SortedSetSyncProviderShouldHaveAValidStringForNullableItems()
        {
            var provider = new SortedSetSyncProvider<int>();

            provider.ToString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void SortedSetSyncProviderShouldHaveAValidStringForNonNullableItems()
        {
            var provider = new SortedSetSyncProvider<int>();

            provider.Items = new SortedSet<int>();
            provider.Items.Add(1);

            provider.ToString().Should().Be(provider.Items.ToString());
        }
    }
}
