namespace Uuid7Benchmark;
using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class App {
    public static void Main(string[] args) {
        var summary = BenchmarkRunner.Run(typeof(App).Assembly);
    }
}
