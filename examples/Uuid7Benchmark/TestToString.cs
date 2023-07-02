using System;
using System.Diagnostics;
using System.Threading;
using Medo;

namespace Uuid7Benchmark;

public static class TestToString {

    public static void Run() {
        Thread.Sleep(1000);
        {
            var uuidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) {
                Uuid7.NewUuid7().ToString();
                uuidCount++;
            }
            sw.Stop();
            Console.WriteLine($"ToString() {uuidCount:#,##0} UUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({uuidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }

        Thread.Sleep(1000);
        {
            var uuidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) {
                Guid.NewGuid().ToString();
                uuidCount++;
            }
            sw.Stop();
            Console.WriteLine($"ToString() {uuidCount:#,##0} reference GUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({uuidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }
    }

}
