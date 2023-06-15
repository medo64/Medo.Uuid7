using System;
using System.Threading;
using Medo;

namespace Uuid7Benchmark;

internal static class App {

    public static void Main() {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        Thread.Sleep(1000);

        var uuids = new Uuid7[5];
        {
            Uuid7.Fill(uuids);

            foreach (var uuid in uuids) {
                Console.WriteLine($"UUID: {uuid}");
                Console.WriteLine($"ID25: {uuid.ToId25String()}");
                Console.WriteLine($"ID22: {uuid.ToId22String()}");
            }
        }

        Console.WriteLine();
        TestSingleThread.Run();

        Console.WriteLine();
        TestMultiThread.Run();

        Console.WriteLine();
        TestToString.Run();
    }
}
