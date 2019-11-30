using FluentAssertions;
using FluentSync.Comparers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.KeyComparerAgent
{
    public partial class KeyComparerAgentTests
    {
        [Fact]
        public async Task Compare_Guid_NonEmptyLists()
        {
            var commonGuid = Guid.NewGuid();
            List<Guid> source = new List<Guid> { commonGuid }
                , destination = new List<Guid> { commonGuid };

            for (int i = 0; i < 3; i++)
            {
                source.Add(Guid.NewGuid());
                destination.Add(Guid.NewGuid());
            }

            var keysComparisonResult = await KeyComparerAgent<Guid>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            keysComparisonResult.KeysInSourceOnly.Count.Should().Be(3);
            keysComparisonResult.KeysInDestinationOnly.Count.Should().Be(3);
            keysComparisonResult.KeysInSourceOnly.Should().NotBeEquivalentTo(keysComparisonResult.KeysInDestinationOnly);
            keysComparisonResult.Matches.Should().BeEquivalentTo(new List<Guid> { commonGuid });
        }

        [Fact]
        public async Task Compare_Guid_EquivalentLists()
        {
            List<Guid> source = new List<Guid>()
                , destination = new List<Guid>();

            for (int i = 0; i < 3; i++)
            {
                var guid = Guid.NewGuid();
                source.Add(guid);
                destination.Add(guid);
            }

            var keysComparisonResult = await KeyComparerAgent<Guid>.Create()
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            keysComparisonResult.KeysInSourceOnly.Count.Should().Be(0);
            keysComparisonResult.KeysInDestinationOnly.Count.Should().Be(0);
            keysComparisonResult.Matches.Count.Should().Be(3);
        }
    }
}
