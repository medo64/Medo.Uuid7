namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToStringB {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuidB() => ExampleGuid.ToString("B");

    [Benchmark]
    public string ToStringUuid7B() => ExampleUuid7.ToString("B");

    [Benchmark]
    public string ToStringUuid4B() => ExampleUuid4.ToString("B");

}
