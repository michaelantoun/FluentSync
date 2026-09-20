using FluentSync.Sync.Providers;

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
        public async Task DictionaryWithNullableItemsShouldThrowExpression()
        {
            var provider = new DictionaryBatchSyncProvider<int, int>();
            Func<Task> act = async () => await provider.AddAsync(new List<int> { }, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(provider.Items)} cannot be null.");
        }

        [Fact]
        public async Task DictionaryWithNullableKeySelectorShouldThrowExpression()
        {
            var provider = new DictionaryBatchSyncProvider<int, int> { Items = new Dictionary<int, int>() };
            Func<Task> act = async () => await provider.AddAsync(new List<int> { }, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(provider.KeySelector)} cannot be null.");
        }
    }
}
