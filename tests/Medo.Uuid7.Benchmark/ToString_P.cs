namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToString_P {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark(Baseline = true)]
    public string Guid_ToString() => ExampleGuid.ToString("P");

    [Benchmark]
    public string Uuid7_ToString() => ExampleUuid7.ToString("P");

    [Benchmark]
    public string Uuid4_ToString() => ExampleUuid4.ToString("P");

}
