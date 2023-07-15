namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToStringP {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuidP() => ExampleGuid.ToString("P");

    [Benchmark]
    public string ToStringUuid7P() => ExampleUuid7.ToString("P");

    [Benchmark]
    public string ToStringUuid4P() => ExampleUuid4.ToString("P");

}
