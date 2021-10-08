using FluentAssertions;
using FluentSync.Sync.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Sync.Providers
{
    public class DictionaryBatchSyncProviderTests
    {
        [Fact]
        public void DictionaryBatchSyncProviderShouldHaveAValidStringForNullableItems()
        {
            var provider = new DictionaryBatchSyncProvider<int, int>();

            provider.ToString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DictionaryBatchSyncProviderShouldHaveAValidStringForNonNullableItems()
        {
            var provider = new DictionaryBatchSyncProvider<int, int>();

            provider.Items = new Dictionary<int, int>();
            provider.Items.Add(1, 1);

            provider.ToString().Should().Be(provider.Items.ToString());
        }

        [Fact]
        public void DictionaryWithNullableItemsShouldThrowExpression()
        {
            var provider = new DictionaryBatchSyncProvider<int, int>();
            Func<Task> act = async () => await provider.AddAsync(new List<int> { }, CancellationToken.None);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(provider.Items)} cannot be null.");
        }

        [Fact]
        public void DictionaryWithNullableKeySelectorShouldThrowExpression()
        {
            var provider = new DictionaryBatchSyncProvider<int, int> { Items = new Dictionary<int, int>() };
            Func<Task> act = async () => await provider.AddAsync(new List<int> { }, CancellationToken.None);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(provider.KeySelector)} cannot be null.");
        }
    }
}
