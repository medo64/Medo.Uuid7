namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToString5 {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuid() => ExampleGuid.ToString("");

    [Benchmark]
    public string ToStringUuid75() => ExampleUuid7.ToString("5");

    [Benchmark]
    public string ToStringUuid45() => ExampleUuid4.ToString("5");

}
