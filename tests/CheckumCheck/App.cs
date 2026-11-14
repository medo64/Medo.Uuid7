using System;
using System.Runtime.CompilerServices;
using Medo;

int index = 0;

var id = Uuid7.NewUuid7().ToCrockfordString(uppercase: false, includeChecksum: true);
Console.WriteLine(id);

Console.WriteLine();

for (var i = 2; i <= id.Length; i++) {
    var left = id[0..(i - 2)];
    var swapped = id[(i - 1)..i] + id[(i - 2)..(i - 1)];
    var right = id[i..^0];
    var id2 = left + swapped + right;
    try {
        Uuid7.FromCrockfordString(id2);
        Print(left, swapped, right);  // print if checksum passes after swap
    } catch (FormatException) { }
}

Console.WriteLine();

for (var i = 0; i < id.Length; i++) {
    var left = id[0..i];
    var lower = ((char)(id[i] - 1)).ToString();
    var right = id[(i + 1)..^0];
    var id2 = left + lower + right;
    try {
        Uuid7.FromCrockfordString(id2);
        Print(left, lower, right);  // print if checksum passes after swap
    } catch (FormatException) { }
}

Console.WriteLine();

for (var i = 0; i < id.Length; i++) {
    var left = id[0..i];
    var upper = ((char)(id[i] + 1)).ToString();
    var right = id[(i + 1)..^0];
    var id2 = left + upper + right;
    try {
        Uuid7.FromCrockfordString(id2);
        Print(left, upper, right);  // print if checksum passes after swap
    } catch (FormatException) { }
}


void Print(string left, string mid, string right) {
    index += 1;
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(index.ToString().PadLeft(2) + ". ");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(left);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write(mid);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(right);

    Console.ResetColor();
    Console.WriteLine();
}
