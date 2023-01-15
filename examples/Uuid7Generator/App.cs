using System;
using Medo;

namespace Uuid7Generator;

internal static class App {

    public static void Main() {
        for (var i = 0; i < 5; i++) {
            var uuid = Uuid7.NewUuid7();
            Console.WriteLine($"UUID: {uuid}");
            Console.WriteLine($"ID25: {uuid.ToId25String()}");
            Console.WriteLine($"ID22: {uuid.ToId22String()}");
            Console.WriteLine();
        }
    }

}
