using System;
using Medo;

namespace Uuid7Generator;

internal static class App {

    public static void Main() {
        while (true) {
            var uuid = Uuid7.NewUuid7();
            Console.WriteLine($"UUID: {uuid}");
            Console.WriteLine($"ID25: {uuid.ToId25String()}");
            Console.WriteLine($"ID22: {uuid.ToId22String()}");
            Console.WriteLine();

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape) { break; }
        }
    }

}
