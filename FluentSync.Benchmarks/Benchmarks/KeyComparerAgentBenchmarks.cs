using BenchmarkDotNet.Attributes;
using FluentSync.Benchmarks.Fixtures;
using FluentSync.Comparers;
using FluentSync.Comparers.Providers;

namespace FluentSync.Benchmarks.Benchmarks;

public class KeyComparerAgentBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int N;

    private List<int> _sourceKeys;
    private List<int> _destinationKeys;

    [GlobalSetup]
    public void Setup()
    {
        (_sourceKeys, _destinationKeys) = DataGenerator.CreateKeySets(N, seed: 42);
    }

    [Benchmark]
    public async Task<int> CompareAsync()
    {
        var agent = KeyComparerAgent<int>.Create();
        agent.SourceProvider = new ComparerProvider<int> { Items = _sourceKeys };
        agent.DestinationProvider = new ComparerProvider<int> { Items = _destinationKeys };

        var result = await agent.CompareAsync(CancellationToken.None);
        return result.Matches.Count + result.KeysInSourceOnly.Count + result.KeysInDestinationOnly.Count;
    }
}
