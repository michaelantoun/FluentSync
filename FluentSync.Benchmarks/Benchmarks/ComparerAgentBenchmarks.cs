using BenchmarkDotNet.Attributes;
using FluentSync.Benchmarks.Fixtures;
using FluentSync.Benchmarks.Models;
using FluentSync.Comparers;

namespace FluentSync.Benchmarks.Benchmarks;

public class ComparerAgentBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int N;

    private List<BenchPerson> _source;
    private List<BenchPerson> _destination;

    [GlobalSetup]
    public void Setup()
    {
        (_source, _destination) = DataGenerator.CreateLists(N, seed: 42);
    }

    [Benchmark]
    public async Task<int> CompareAsync()
    {
        var result = await ComparerAgent<int, BenchPerson>.Create()
            .SetKeySelector(p => p.Id)
            .SetCompareItemFunc((s, d) =>
            {
                if (s.FirstName == d.FirstName
                    && s.LastName == d.LastName
                    && s.ModifiedDate == d.ModifiedDate)
                    return MatchComparisonResultType.Same;
                if (s.ModifiedDate < d.ModifiedDate)
                    return MatchComparisonResultType.NewerDestination;
                if (s.ModifiedDate > d.ModifiedDate)
                    return MatchComparisonResultType.NewerSource;
                return MatchComparisonResultType.Conflict;
            })
            .SetSourceProvider(_source)
            .SetDestinationProvider(_destination)
            .CompareAsync(CancellationToken.None);

        return result.Matches.Count + result.ItemsInSourceOnly.Count + result.ItemsInDestinationOnly.Count;
    }
}
