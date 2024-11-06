namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

[MemoryDiagnoser]
public class New {

    [Benchmark(Baseline = true)]
    public Guid NewSystemGuid() => Guid.NewGuid();

    [Benchmark]
    public void NewUuid7() => Uuid7.NewUuid7();

    [Benchmark]
    public void NewUuid4() => Uuid7.NewUuid4();

    [Benchmark]
    public void NewGuid() => Uuid7.NewGuid();

    [Benchmark]
    public void NewGuidNonMatched() => Uuid7.NewGuid(matchGuidEndianness: false);

    [Benchmark]
    public void NewGuidMatched() => Uuid7.NewGuid(matchGuidEndianness: true);

    [Benchmark]
    public void NewMsSqlUniqueIdentifier() => Uuid7.NewMsSqlUniqueIdentifier();

}
