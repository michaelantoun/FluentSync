using BenchmarkDotNet.Attributes;
using FluentSync.Benchmarks.Fixtures;
using FluentSync.Benchmarks.Models;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;

namespace FluentSync.Benchmarks.Benchmarks;

public class SyncAgentBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int N;

    private List<BenchPerson> _sourceTemplate;
    private List<BenchPerson> _destinationTemplate;

    private List<BenchPerson> _source;
    private List<BenchPerson> _destination;

    [GlobalSetup]
    public void GlobalSetup()
    {
        (_sourceTemplate, _destinationTemplate) = DataGenerator.CreateLists(N, seed: 42);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _source = DataGenerator.CloneList(_sourceTemplate);
        _destination = DataGenerator.CloneList(_destinationTemplate);
    }

    [Benchmark]
    public Task TwoWay() => Run(SyncModePreset.TwoWay);

    [Benchmark]
    public Task MirrorToDestination() => Run(SyncModePreset.MirrorToDestination);

    [Benchmark]
    public Task UpdateDestination() => Run(SyncModePreset.UpdateDestination);

    private Task Run(SyncModePreset preset)
    {
        var comparer = ComparerAgent<int, BenchPerson>.Create()
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
            });

        return SyncAgent<int, BenchPerson>.Create()
            .Configure(c => c.SyncMode.SyncModePreset = preset)
            .SetComparerAgent(comparer)
            .SetSourceProvider(new ListSyncProvider<BenchPerson> { Items = _source })
            .SetDestinationProvider(new ListSyncProvider<BenchPerson> { Items = _destination })
            .SyncAsync(CancellationToken.None);
    }
}
