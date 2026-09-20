using BenchmarkDotNet.Running;
using FluentSync.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, BenchmarkConfig.Create(args));

internal partial class Program { }
