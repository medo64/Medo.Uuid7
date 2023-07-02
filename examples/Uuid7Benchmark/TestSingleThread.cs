using System;
using System.Diagnostics;
using System.Threading;
using Medo;

namespace Uuid7Benchmark;

public static class TestSingleThread {

    public static void Run() {
        Thread.Sleep(1000);
        {
            var uuidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) {
                _ = Uuid7.NewUuid7();
                uuidCount++;
            }
            sw.Stop();
            Console.WriteLine($"Generated {uuidCount:#,##0} v7 UUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({uuidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }

        Thread.Sleep(1000);
        {
            var uuidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) {
                _ = Uuid7.NewUuid4();
                uuidCount++;
            }
            sw.Stop();
            Console.WriteLine($"Generated {uuidCount:#,##0} v4 UUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({uuidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }

        Thread.Sleep(1000);
        {
            var guidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) {
                _ = Guid.NewGuid();
                guidCount++;
            }
            sw.Stop();
            Console.WriteLine($"Generated {guidCount:#,##0} reference GUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({guidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }
    }

}
