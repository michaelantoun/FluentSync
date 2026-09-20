using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace FluentSync.Benchmarks;

internal static class BenchmarkConfig
{
    public static IConfig Create(string[] args)
    {
        var config = DefaultConfig.Instance.AddDiagnoser(MemoryDiagnoser.Default);

        // A job given on the command line (e.g. --job Dry) replaces this one instead of running alongside it.
        if (!args.Any(IsJobArgument))
            config = config.AddJob(Job.Default
                .WithWarmupCount(3)
                .WithIterationCount(5));

        return config;
    }

    private static bool IsJobArgument(string arg) =>
        arg is "-j" or "--job" || arg.StartsWith("--job=", StringComparison.Ordinal);
}
