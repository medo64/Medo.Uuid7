using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Medo;

namespace Uuid7Generator;

internal static class App {

    public static void Main() {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        var uuids = new Uuid7[5];
        Uuid7.Fill(uuids);

        foreach (var uuid in uuids) {
            Console.WriteLine($"UUID: {uuid}");
            Console.WriteLine($"ID25: {uuid.ToId25String()}");
            Console.WriteLine($"ID22: {uuid.ToId22String()}");
            Console.WriteLine();
        }

        Thread.Sleep(1000);

        {
            var uuidCount = 0;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) {
                _ = Uuid7.NewUuid7();
                uuidCount++;
            }
            sw.Stop();
            Console.WriteLine($"Generated {uuidCount:#,##0} UUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({uuidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
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
            Console.WriteLine($"Generated {guidCount:#,##0} GUIDs in {sw.ElapsedMilliseconds:#,##0} millisecond ({guidCount / sw.ElapsedMilliseconds * 1000:#,##0} per second)");
        }

    }
}
