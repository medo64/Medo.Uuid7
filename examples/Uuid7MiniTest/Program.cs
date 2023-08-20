using Medo;
using System.Diagnostics;

{
    var stopwatch = Stopwatch.StartNew();
    const int guidCount = 100_000_000;
    for (var i = 0; i < guidCount; i++)
        Uuid7.NewUuid4();
    stopwatch.Stop();
    Console.WriteLine($"{guidCount:n0} v4 UUIDs in {stopwatch.ElapsedMilliseconds:n0}ms; {((double)guidCount / stopwatch.ElapsedMilliseconds):f1}/ms");
}

{
    var stopwatch = Stopwatch.StartNew();
    const int guidCount = 100_000_000;
    for (var i = 0; i < guidCount; i++)
        Uuid7.NewUuid7();
    stopwatch.Stop();
    Console.WriteLine($"{guidCount:n0} v7 UUIDs in {stopwatch.ElapsedMilliseconds:n0}ms; {((double)guidCount / stopwatch.ElapsedMilliseconds):f1}/ms");
}
