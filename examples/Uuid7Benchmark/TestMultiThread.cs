using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Medo;

namespace Uuid7Benchmark;

public static class TestMultiThread {

    public static void Run() {
        var threadCount = Environment.ProcessorCount / 2;
        if (threadCount < 1) { threadCount = 1; }

        Thread.Sleep(1000);
        {
            var totalCount = 0;
            var sw = Stopwatch.StartNew();

            Parallel.For(0, threadCount, (i) => {
                int count = 0;
                var threadSw = Stopwatch.StartNew();
                while (threadSw.ElapsedMilliseconds < 5000) {
                    _ = Uuid7.NewUuid7();
                    count++;
                }
                threadSw.Stop();
                Interlocked.Add(ref totalCount, count); // Sum the counts from all threads
            });

            sw.Stop();
            Console.WriteLine($"Generated {totalCount:#,##0} v7 UUIDs using {threadCount} threads in {sw.ElapsedMilliseconds:#,##0} milliseconds ({totalCount / threadCount / sw.ElapsedMilliseconds * 1000:#,##0} per second per thread)");
        }

        Thread.Sleep(1000);
        {
            var totalCount = 0;
            var sw = Stopwatch.StartNew();

            Parallel.For(0, threadCount, (i) => {
                int count = 0;
                var threadSw = Stopwatch.StartNew();
                while (threadSw.ElapsedMilliseconds < 5000) {
                    _ = Uuid7.NewUuid4();
                    count++;
                }
                threadSw.Stop();
                Interlocked.Add(ref totalCount, count); // Sum the counts from all threads
            });

            sw.Stop();
            Console.WriteLine($"Generated {totalCount:#,##0} v4 UUIDs using {threadCount} threads in {sw.ElapsedMilliseconds:#,##0} milliseconds ({totalCount / threadCount / sw.ElapsedMilliseconds * 1000:#,##0} per second per thread)");
        }

        Thread.Sleep(1000);
        {
            var totalCount = 0;
            var sw = Stopwatch.StartNew();

            Parallel.For(0, threadCount, (i) => {
                int count = 0;
                var threadSw = Stopwatch.StartNew();
                while (threadSw.ElapsedMilliseconds < 5000) {
                    _ = Guid.NewGuid();
                    count++;
                }
                threadSw.Stop();
                Interlocked.Add(ref totalCount, count); // Sum the counts from all threads
            });

            sw.Stop();
            Console.WriteLine($"Generated {totalCount:#,##0} GUIDs using {threadCount} threads in {sw.ElapsedMilliseconds:#,##0} milliseconds ({totalCount / threadCount / sw.ElapsedMilliseconds * 1000:#,##0} per second per thread)");
        }
    }

}
