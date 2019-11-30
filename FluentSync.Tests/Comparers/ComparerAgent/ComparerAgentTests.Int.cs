using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Comparers.Configurations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.ComparerAgent
{
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_Int_NonEmptyLists()
        {
            List<int> source = new List<int> { 10, 30, 20, 5 }
                , destination = new List<int> { 15, 5, 30 };

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<int> { 10, 20 });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<int> { 15 });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<int>>
            {
                new MatchComparisonResult<int>{Source = 30, Destination = 30, ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<int>{Source = 5, Destination = 5, ComparisonResult = MatchComparisonResultType.Same}
            });
        }

        [Fact]
        public async Task Compare_Int_EquivalentLists()
        {
            List<int> source = new List<int> { 10, 30, 20 }
                , destination = new List<int> { 20, 10, 30 };

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<int>>
            {
                new MatchComparisonResult<int>{Source = 10, Destination = 10, ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<int>{Source = 20, Destination = 20, ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<int>{Source = 30, Destination = 30, ComparisonResult = MatchComparisonResultType.Same}
            });
        }

        [Fact]
        public async Task Compare_Int_EquivalentListsWithOneItem()
        {
            List<int> source = new List<int> { 10 }
                , destination = new List<int> { 10 };

            var comparisonResult = await ComparerAgent<int>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEmpty();
            comparisonResult.ItemsInDestinationOnly.Should().BeEmpty();

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<int>>
            {
                new MatchComparisonResult<int>{Source = 10, Destination = 10, ComparisonResult = MatchComparisonResultType.Same}
            });
        }

        [Fact]
        public void Compare_Int_PreventDuplicatesInSource()
        {
            List<int> source = new List<int> { 10, 10 }
                , destination = new List<int> { 20, 20 };

            Func<Task> act = async () => await ComparerAgent<int>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Destination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Duplicated items are not allowed in the source list, 1 item was found.");
        }

        [Fact]
        public void Compare_Int_PreventDuplicatesInDestination()
        {
            List<int> source = new List<int> { 10, 10 }
                , destination = new List<int> { 20, 20 };

            Func<Task> act = async () => await ComparerAgent<int>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Source)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Duplicated items are not allowed in the destination list, 1 item was found.");
        }

        [Fact]
        public void Compare_Int_PreventNullableItemsInSource()
        {
            List<int?> source = new List<int?> { 10, null }
                , destination = new List<int?> { 20, null };

            Func<Task> act = async () => await ComparerAgent<int?>.Create()
                .Configure((c) => c.AllowNullableItems = RuleAllowanceType.Destination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Null-able items are not allowed in the source list, 1 item was found.");
        }

        [Fact]
        public void Compare_Int_PreventNullableItemsInDestination()
        {
            List<int?> source = new List<int?> { 10, null }
                , destination = new List<int?> { 20, null };

            Func<Task> act = async () => await ComparerAgent<int?>.Create()
                .Configure((c) => c.AllowNullableItems = RuleAllowanceType.Source)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Null-able items are not allowed in the destination list, 1 item was found.");
        }
    }
}
