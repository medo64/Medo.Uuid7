namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToStringN {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuidN() => ExampleGuid.ToString("N");

    [Benchmark]
    public string ToStringUuid7N() => ExampleUuid7.ToString("N");

    [Benchmark]
    public string ToStringUuid4N() => ExampleUuid4.ToString("N");

}
