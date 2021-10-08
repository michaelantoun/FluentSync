using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Comparers.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.ComparerAgent
{
    public partial class ComparerAgentTests
    {
        #region Missing configurations

        [Fact]
        public void Compare_Int_NoSourceProvider()
        {
            List<int> destination = new List<int>();

            Func<Task> act = async () => await ComparerAgent<int>.Create()
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(ComparerAgent<int>.SourceProvider)} cannot be null.");
        }

        [Fact]
        public void Compare_Int_NoDestinationProvider()
        {
            List<int> source = new List<int>();

            Func<Task> act = async () => await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(ComparerAgent<int>.DestinationProvider)} cannot be null.");
        }

        [Fact]
        public void Compare_Int_NullableKeySelector()
        {
            List<int> source = new List<int>()
                , destination = new List<int>();

            var comparerAgent = ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination);
            comparerAgent.KeySelector = null;

            Func<Task> act = async () => await comparerAgent
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(ComparerAgent<int>.KeySelector)} cannot be null.");
        }

        [Fact]
        public void Compare_Int_NullableCompareItemFunc()
        {
            List<int> source = new List<int>()
                , destination = new List<int>();

            var comparerAgent = ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination);
            comparerAgent.CompareItemFunc = null;

            Func<Task> act = async () => await comparerAgent
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().ThrowAsync<NullReferenceException>().WithMessage($"The {nameof(ComparerAgent<int>.CompareItemFunc)} cannot be null.");
        }

        #endregion

        #region Compare null-able/empty lists

        [Fact]
        public async Task Compare_Int_EmptyLists()
        {
            List<int> source = new List<int>()
                , destination = new List<int>();

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();
            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_Int_EmptySourceList()
        {
            List<int> source = new List<int>()
                , destination = new List<int> { 1 };

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<int> { 1 });
            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_Int_EmptyDestinationList()
        {
            List<int> source = new List<int> { 1 }
                , destination = new List<int>();

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<int> { 1 });
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();
            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_Int_NullableSourceList()
        {
            List<int> source = null
                , destination = new List<int>();

            var comparisonResult = await ComparerAgent<int>.Create()
                    .SetSourceProvider(source)
                    .SetDestinationProvider(destination)
                    .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();
            comparisonResult.Matches.Should().BeEmpty();
        }

        [Fact]
        public async Task Compare_Int_NullableDestinationList()
        {
            List<int> source = new List<int>()
                , destination = null;

            var comparisonResult = await ComparerAgent<int>.Create()
                    .SetSourceProvider(source)
                    .SetDestinationProvider(destination)
                    .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();
            comparisonResult.Matches.Should().BeEmpty();
        }

        #endregion

        [Fact]
        public void ComparerAgentShouldHaveValidString()
        {
            List<int> source = new List<int>()
                , destination = new List<int>();

            var comparerAgent = ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination);

            comparerAgent.ToString().Should().Be($"{nameof(comparerAgent.Configurations)}: {{{comparerAgent.Configurations}}}");
        }

        [Fact]
        public async Task ComparerAgentShouldHaveComparerProviders()
        {
            ComparerProvider<int> source = new ComparerProvider<int> { Items = new List<int>() }
                , destination = new ComparerProvider<int> { Items = new List<int>() };

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();
            comparisonResult.Matches.Should().BeEmpty();
        }
    }
}
