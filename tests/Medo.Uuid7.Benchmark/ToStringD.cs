namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToStringD {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuidD() => ExampleGuid.ToString("D");

    [Benchmark]
    public string ToStringUuid7D() => ExampleUuid7.ToString("D");

    [Benchmark]
    public string ToStringUuid4D() => ExampleUuid4.ToString("D");

}
