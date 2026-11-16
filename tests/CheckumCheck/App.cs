using System;
using System.Runtime.CompilerServices;
using Medo;

int failed = 0;

var id = Uuid7.NewUuid7().ToCrockfordString(includeChecksum: true);

Console.WriteLine();

Console.WriteLine(id);

Console.WriteLine();

for (var i = 2; i <= id.Length; i++) {
    var left = id[0..(i - 2)];
    var swapped = id[(i - 1)..i] + id[(i - 2)..(i - 1)];
    var right = id[i..^0];
    if (!Uuid7.TryParseCrockford(left + swapped + right, out var _)) {
        Print(left, swapped, right);
    }
}

Console.WriteLine();

for (var i = 0; i < id.Length; i++) {
    var left = id[0..i];
    var lower = ((char)(id[i] - 1)).ToString();
    var right = id[(i + 1)..^0];
    if (!Uuid7.TryParseCrockford(left + lower + right, out var _)) {
        Print(left, lower, right);
    }
}

Console.WriteLine();

for (var i = 0; i < id.Length; i++) {
    var left = id[0..i];
    var upper = ((char)(id[i] + 1)).ToString();
    var right = id[(i + 1)..^0];
    if (!Uuid7.TryParseCrockford(left + upper + right, out var _)) {
        Print(left, upper, right);
    }
}


void Print(string left, string mid, string right) {
    failed += 1;

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(failed.ToString().PadLeft(2) + ". ");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(left);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write(mid);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(right);

    Console.ResetColor();
    Console.WriteLine();
}
