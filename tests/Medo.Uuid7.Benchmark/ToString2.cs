namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToString2 {

    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();
    private Uuid7 ExampleUuid4 =  Uuid7.NewUuid4();


    [Benchmark]
    public string ToStringUuid72() => ExampleUuid7.ToString("2");

    [Benchmark]
    public string ToStringUuid42() => ExampleUuid4.ToString("2");

}
