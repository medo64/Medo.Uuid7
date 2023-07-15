namespace Uuid7Benchmark;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ToString_Id25 {

    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();


    [Benchmark(Baseline = true)]
    public string Uuid7_ToString() => ExampleUuid7.ToString();

    [Benchmark]
    public string Uuid7_ToId25String() => ExampleUuid7.ToId25String();

}
