namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToStringAll {

    private Guid ExampleGuid =  Guid.NewGuid();
    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();


    [Benchmark(Baseline = true)]
    public string Guid_ToString() => ExampleGuid.ToString();

    [Benchmark]
    public string Uuid7_ToString() => ExampleUuid7.ToString();

    [Benchmark]
    public string Uuid7_ToId26String() => ExampleUuid7.ToId26String();

    [Benchmark]
    public string Uuid7_ToId25String() => ExampleUuid7.ToId25String();

    [Benchmark]
    public string Uuid7_ToId22String() => ExampleUuid7.ToId22String();

}
