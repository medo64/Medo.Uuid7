namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToStringX {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuidX() => ExampleGuid.ToString("X");

    [Benchmark]
    public string ToStringUuid7X() => ExampleUuid7.ToString("X");

    [Benchmark]
    public string ToStringUuid4X() => ExampleUuid4.ToString("X");

}
