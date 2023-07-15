namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToString {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string ToStringGuid() => ExampleGuid.ToString();

    [Benchmark]
    public string ToStringUuid7() => ExampleUuid7.ToString();

    [Benchmark]
    public string ToStringUuid4() => ExampleUuid4.ToString();

}
