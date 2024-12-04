namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

[MemoryDiagnoser]
public class Fill {

    [Benchmark(Baseline = true)]
    public void Loop() {
        var elements = new Guid[1000];
        for(var i = 0; i < elements.Length; i++) {  // no Fill method
            elements[i] = Guid.NewGuid();
        }
    }

    [Benchmark]
    public void FillUuid4() {
        var elements = new Uuid7[1000];
        Uuid7.FillUuid4(elements);
    }

    [Benchmark]
    public void FillUuid7() {
        var elements = new Uuid7[1000];
        Uuid7.Fill(elements);
    }

    [Benchmark]
    public void FillGuid() {
        var elements = new Guid[1000];
        Uuid7.FillGuid(elements);
    }

    [Benchmark]
    public void FillGuidLE() {
        var elements = new Guid[1000];
        Uuid7.FillGuid(elements, bigEndian: false);
    }

    [Benchmark]
    public void FillGuidBE() {
        var elements = new Guid[1000];
        Uuid7.FillGuid(elements, bigEndian: true);
    }

    [Benchmark]
    public void FillMsSqlUniqueIdentifier() {
        var elements = new Guid[1000];
        Uuid7.FillMsSqlUniqueIdentifier(elements);
    }

}
