/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public readonly partial struct Uuid7 {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsDefaultString(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsDefaultString(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 36) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

        (destination[0], destination[1]) = ToTwoHexBytes(bytes[0]);
        (destination[2], destination[3]) = ToTwoHexBytes(bytes[1]);
        (destination[4], destination[5]) = ToTwoHexBytes(bytes[2]);
        (destination[6], destination[7]) = ToTwoHexBytes(bytes[3]);
        destination[8] = '-';
        (destination[9], destination[10]) = ToTwoHexBytes(bytes[4]);
        (destination[11], destination[12]) = ToTwoHexBytes(bytes[5]);
        destination[13] = '-';
        (destination[14], destination[15]) = ToTwoHexBytes(bytes[6]);
        (destination[16], destination[17]) = ToTwoHexBytes(bytes[7]);
        destination[18] = '-';
        (destination[19], destination[20]) = ToTwoHexBytes(bytes[8]);
        (destination[21], destination[22]) = ToTwoHexBytes(bytes[9]);
        destination[23] = '-';
        (destination[24], destination[25]) = ToTwoHexBytes(bytes[10]);
        (destination[26], destination[27]) = ToTwoHexBytes(bytes[11]);
        (destination[28], destination[29]) = ToTwoHexBytes(bytes[12]);
        (destination[30], destination[31]) = ToTwoHexBytes(bytes[13]);
        (destination[32], destination[33]) = ToTwoHexBytes(bytes[14]);
        (destination[34], destination[35]) = ToTwoHexBytes(bytes[15]);

        charsWritten = 36;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsNoHypensString(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsNoHypensString(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 32) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

        (destination[0], destination[1]) = ToTwoHexBytes(bytes[0]);
        (destination[2], destination[3]) = ToTwoHexBytes(bytes[1]);
        (destination[4], destination[5]) = ToTwoHexBytes(bytes[2]);
        (destination[6], destination[7]) = ToTwoHexBytes(bytes[3]);
        (destination[8], destination[9]) = ToTwoHexBytes(bytes[4]);
        (destination[10], destination[11]) = ToTwoHexBytes(bytes[5]);
        (destination[12], destination[13]) = ToTwoHexBytes(bytes[6]);
        (destination[14], destination[15]) = ToTwoHexBytes(bytes[7]);
        (destination[16], destination[17]) = ToTwoHexBytes(bytes[8]);
        (destination[18], destination[19]) = ToTwoHexBytes(bytes[9]);
        (destination[20], destination[21]) = ToTwoHexBytes(bytes[10]);
        (destination[22], destination[23]) = ToTwoHexBytes(bytes[11]);
        (destination[24], destination[25]) = ToTwoHexBytes(bytes[12]);
        (destination[26], destination[27]) = ToTwoHexBytes(bytes[13]);
        (destination[28], destination[29]) = ToTwoHexBytes(bytes[14]);
        (destination[30], destination[31]) = ToTwoHexBytes(bytes[15]);

        charsWritten = 32;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsBracesString(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsBracesString(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 38) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

        destination[0] = '{';
        (destination[1], destination[2]) = ToTwoHexBytes(bytes[0]);
        (destination[3], destination[4]) = ToTwoHexBytes(bytes[1]);
        (destination[5], destination[6]) = ToTwoHexBytes(bytes[2]);
        (destination[7], destination[8]) = ToTwoHexBytes(bytes[3]);
        destination[9] = '-';
        (destination[10], destination[11]) = ToTwoHexBytes(bytes[4]);
        (destination[12], destination[13]) = ToTwoHexBytes(bytes[5]);
        destination[14] = '-';
        (destination[15], destination[16]) = ToTwoHexBytes(bytes[6]);
        (destination[17], destination[18]) = ToTwoHexBytes(bytes[7]);
        destination[19] = '-';
        (destination[20], destination[21]) = ToTwoHexBytes(bytes[8]);
        (destination[22], destination[23]) = ToTwoHexBytes(bytes[9]);
        destination[24] = '-';
        (destination[25], destination[26]) = ToTwoHexBytes(bytes[10]);
        (destination[27], destination[28]) = ToTwoHexBytes(bytes[11]);
        (destination[29], destination[30]) = ToTwoHexBytes(bytes[12]);
        (destination[31], destination[32]) = ToTwoHexBytes(bytes[13]);
        (destination[33], destination[34]) = ToTwoHexBytes(bytes[14]);
        (destination[35], destination[36]) = ToTwoHexBytes(bytes[15]);
        destination[37] = '}';

        charsWritten = 38;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsParenthesesString(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsParenthesesString(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 38) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

        destination[0] = '(';
        (destination[1], destination[2]) = ToTwoHexBytes(bytes[0]);
        (destination[3], destination[4]) = ToTwoHexBytes(bytes[1]);
        (destination[5], destination[6]) = ToTwoHexBytes(bytes[2]);
        (destination[7], destination[8]) = ToTwoHexBytes(bytes[3]);
        destination[9] = '-';
        (destination[10], destination[11]) = ToTwoHexBytes(bytes[4]);
        (destination[12], destination[13]) = ToTwoHexBytes(bytes[5]);
        destination[14] = '-';
        (destination[15], destination[16]) = ToTwoHexBytes(bytes[6]);
        (destination[17], destination[18]) = ToTwoHexBytes(bytes[7]);
        destination[19] = '-';
        (destination[20], destination[21]) = ToTwoHexBytes(bytes[8]);
        (destination[22], destination[23]) = ToTwoHexBytes(bytes[9]);
        destination[24] = '-';
        (destination[25], destination[26]) = ToTwoHexBytes(bytes[10]);
        (destination[27], destination[28]) = ToTwoHexBytes(bytes[11]);
        (destination[29], destination[30]) = ToTwoHexBytes(bytes[12]);
        (destination[31], destination[32]) = ToTwoHexBytes(bytes[13]);
        (destination[33], destination[34]) = ToTwoHexBytes(bytes[14]);
        (destination[35], destination[36]) = ToTwoHexBytes(bytes[15]);
        destination[37] = ')';

        charsWritten = 38;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsHexadecimalString(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsHexadecimalString(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 68) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

        (destination[0], destination[1], destination[2]) = ('{', '0', 'x');
        (destination[3], destination[4]) = ToTwoHexBytes(bytes[0]);
        (destination[5], destination[6]) = ToTwoHexBytes(bytes[1]);
        (destination[7], destination[8]) = ToTwoHexBytes(bytes[2]);
        (destination[9], destination[10]) = ToTwoHexBytes(bytes[3]);
        (destination[11], destination[12], destination[13]) = (',', '0', 'x');
        (destination[14], destination[15]) = ToTwoHexBytes(bytes[4]);
        (destination[16], destination[17]) = ToTwoHexBytes(bytes[5]);
        (destination[18], destination[19], destination[20]) = (',', '0', 'x');
        (destination[21], destination[22]) = ToTwoHexBytes(bytes[6]);
        (destination[23], destination[24]) = ToTwoHexBytes(bytes[7]);
        (destination[25], destination[26], destination[27], destination[28]) = (',', '{', '0', 'x');
        (destination[29], destination[30]) = ToTwoHexBytes(bytes[8]);
        (destination[31], destination[32], destination[33]) = (',', '0', 'x');
        (destination[34], destination[35]) = ToTwoHexBytes(bytes[9]);
        (destination[36], destination[37], destination[38]) = (',', '0', 'x');
        (destination[39], destination[40]) = ToTwoHexBytes(bytes[10]);
        (destination[41], destination[42], destination[43]) = (',', '0', 'x');
        (destination[44], destination[45]) = ToTwoHexBytes(bytes[11]);
        (destination[46], destination[47], destination[48]) = (',', '0', 'x');
        (destination[49], destination[50]) = ToTwoHexBytes(bytes[12]);
        (destination[51], destination[52], destination[53]) = (',', '0', 'x');
        (destination[54], destination[55]) = ToTwoHexBytes(bytes[13]);
        (destination[56], destination[57], destination[58]) = (',', '0', 'x');
        (destination[59], destination[60]) = ToTwoHexBytes(bytes[14]);
        (destination[61], destination[62], destination[63]) = (',', '0', 'x');
        (destination[64], destination[65]) = ToTwoHexBytes(bytes[15]);
        (destination[66], destination[67]) = ('}', '}');

        charsWritten = 68;
        return true;
    }
    private static readonly char[] Base16Alphabet = [
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
    ];
    private static readonly Lazy<Dictionary<char, byte>> Base16AlphabetDict = new(() => {
        var dict = new Dictionary<char, byte>(Base16Alphabet.Length);
        for (var i = 0; i < Base16Alphabet.Length; i++) {
            var ch = Base16Alphabet[i];
            dict.Add(ch, (byte)i);
            if (char.IsLetter(ch)) { dict.Add(char.ToUpperInvariant(ch), (byte)i); }  // case-insensitive
        }
        return dict;
    });

#if NET6_0_OR_GREATER
    private static bool TryParseAsString(ReadOnlySpan<char> source, out Uuid7 result) {
#else
    private static bool TryParseAsString(char[] source, out Uuid7 result) {
#endif
        var alphabetDict = Base16AlphabetDict.Value;
        var count = 0;
        var buffer = new byte[16];
        foreach (var ch in source) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                if (count >= 32) { result = Uuid7.Empty; return false; }
                var i = count / 2;
                buffer[i] = (byte)(buffer[i] << 4 | offset);
                count++;
            }
        }
        if (count != 32) { result = Uuid7.Empty; return false; }

        result = new Uuid7(buffer);
        return true;
    }

}
