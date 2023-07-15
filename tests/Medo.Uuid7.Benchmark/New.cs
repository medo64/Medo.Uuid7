namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class New {

    [Benchmark(Baseline = true)]
    public Guid NewGuid() => Guid.NewGuid();

    [Benchmark]
    public void NewUuid7() => Uuid7.NewUuid7();

    [Benchmark]
    public void NewUuid4() => Uuid7.NewUuid4();

}
