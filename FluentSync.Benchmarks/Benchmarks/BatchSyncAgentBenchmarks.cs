using BenchmarkDotNet.Attributes;
using FluentSync.Benchmarks.Fixtures;
using FluentSync.Benchmarks.Models;
using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Sync.Configurations;
using FluentSync.Sync.Providers;

namespace FluentSync.Benchmarks.Benchmarks;

public class BatchSyncAgentBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int N;

    [Params(64, 1_000)]
    public int BatchSize;

    private Dictionary<int, BenchPerson> _sourceTemplate;
    private Dictionary<int, BenchPerson> _destinationTemplate;

    private Dictionary<int, BenchPerson> _source;
    private Dictionary<int, BenchPerson> _destination;

    [GlobalSetup]
    public void GlobalSetup()
    {
        (_sourceTemplate, _destinationTemplate) = DataGenerator.CreateDictionaries(N, seed: 42);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _source = DataGenerator.CloneDictionary(_sourceTemplate);
        _destination = DataGenerator.CloneDictionary(_destinationTemplate);
    }

    [Benchmark]
    public Task TwoWay() => Run(SyncModePreset.TwoWay);

    [Benchmark]
    public Task MirrorToDestination() => Run(SyncModePreset.MirrorToDestination);

    [Benchmark]
    public Task UpdateDestination() => Run(SyncModePreset.UpdateDestination);

    private Task Run(SyncModePreset preset)
    {
        return BatchSyncAgent<int, BenchPerson>.Create()
            .Configure(c =>
            {
                c.SyncMode.SyncModePreset = preset;
                c.BatchSize = BatchSize;
            })
            .SetComparerAgent(KeyComparerAgent<int>.Create())
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
            .SyncAsync(CancellationToken.None);
    }
}
