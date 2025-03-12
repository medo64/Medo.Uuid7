namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class ParseAll {

    private static Uuid7 Template =  Uuid7.NewUuid7();

    private string GuidDefault =  Template.ToGuid().ToString();
    private string Uuid7Default =  Template.ToString();
    private string Uuid7Id26 =  Template.ToId26String();
    private string Uuid7Id25 =  Template.ToId25String();
    private string Uuid7Id22 =  Template.ToId22String();


    [Benchmark(Baseline = true)]
    public Guid Guid_ParseDefault() => Guid.Parse(GuidDefault);

    [Benchmark]
    public Uuid7 Uuid7_ParseDefault() => Uuid7.Parse(Uuid7Default);

    [Benchmark]
    public Uuid7 Uuid7_ParseId26() => Uuid7.FromId26String(Uuid7Id26);

    [Benchmark]
    public Uuid7 Uuid7_ParseId25() => Uuid7.FromId25String(Uuid7Id25);

    [Benchmark]
    public Uuid7 Uuid7_ParseId22() => Uuid7.FromId22String(Uuid7Id22);

}
