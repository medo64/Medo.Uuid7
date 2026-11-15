/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public readonly partial struct Uuid7 {

    private static readonly char[] Id26Alphabet = [
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'b', 'c', 'd', 'e', 'f', 'g',
        'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    ];
    private static readonly Lazy<Dictionary<char, int>> Id26AlphabetDict = new(() => {
        var dict = new Dictionary<char, int>(Id26Alphabet.Length);
        for (var i = 0; i < Id26Alphabet.Length; i++) {
            var ch = Id26Alphabet[i];
            dict.Add(ch, i);
            if (char.IsLetter(ch)) {  // case-insensitive
                dict.Add(char.ToUpperInvariant(ch), i);
            }
        }
        dict.Add('o', 0);
        dict.Add('O', 0);
        dict.Add('l', 1);
        dict.Add('L', 1);
        dict.Add('i', 1);
        dict.Add('I', 1);
        return dict;
    });


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsId26(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsId26(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 26) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

        var sum1 = 1;
        var sum2 = 0;
        var reservoir = 0;
        var level = 0;
        var sourceIndex = 0;
        for (var i = 0; i < 26; i++) {
            if (level < 5) {  // load new byte
                byte data;
                if (sourceIndex < 16) {
                    data = bytes[sourceIndex++];
                    sum1 += data;
                    sum2 += sum1;
                } else {
                    data = (byte)(((sum1 % 2) << 7) | ((sum2 % 2) << 6));
                }
                reservoir = (reservoir << 8) | data;
                level += 8;
            }
            level -= 5;
            var index = (reservoir >> level) & 0b11111;

            destination[i] = Id26Alphabet[index];
        }

        charsWritten = 26;
        return true;
    }


#if NET6_0_OR_GREATER
    private static bool TryParseAsId26(ReadOnlySpan<char> source, out Uuid7 result) {
#else
    private static bool TryParseAsId26(char[] source, out Uuid7 result) {
#endif

        var buffer = new byte[16];
        var outCount = 0;
        var alphabetDict = Id26AlphabetDict.Value;
        var inCount = 0;
        var sum1 = 1;
        var sum2 = 0;
        var reservoir = 0;
        var level = 0;
        foreach (var ch in source) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                inCount++;
                reservoir = (reservoir << 5) | offset;
                level += 5;
                if (level >= 8) {  // enough for a byte
                    level -= 8;
                    var data = (byte)(reservoir >> level);
                    buffer[outCount++] = (byte)data;
                    sum1 += data;
                    sum2 += sum1;
                    if (outCount == 16) {
                        var expected = (byte)(((sum1 % 2) << 1) | ((sum2 % 2) << 0));
                        if ((reservoir & 0b11) != expected) {  // invalid checksum; fill result anyhow
                            result = new Uuid7(buffer);  // return parsed data anyhow
                            return false;
                        }
                    }
                }
            }
        }
        if (inCount != 26) { result = Empty; return false; }

        result = new Uuid7(buffer);
        return true;
    }

}
