/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

public readonly partial struct Uuid7 {

    private static readonly BigInteger Base58Modulo = 58;
    private static readonly char[] Base58Alphabet = [
        '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
        'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
        'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
        'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
        'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    ];
    private static readonly Lazy<Dictionary<char, BigInteger>> Base58AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>(Base58Alphabet.Length);
        for (var i = 0; i < Base58Alphabet.Length; i++) {
            dict.Add(Base58Alphabet[i], i);
        }
        return dict;
    });


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsId22(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsId22(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 22) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

#if NET6_0_OR_GREATER
        var number = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
#else
        var bytesEx = new byte[17];
        Buffer.BlockCopy(bytes, 0, bytesEx, 1, 16);
        if (BitConverter.IsLittleEndian) { Array.Reverse(bytesEx); }
        var number = new BigInteger(bytesEx);
#endif
        for (var i = 21; i >= 0; i--) {
            number = BigInteger.DivRem(number, Base58Modulo, out var remainder);
            destination[i] = Base58Alphabet[(int)remainder];
        }

        charsWritten = 22;
        return true;
    }


#if NET6_0_OR_GREATER
    private static bool TryParseAsId22(ReadOnlySpan<char> source, out Uuid7 result) {
#else
    private static bool TryParseAsId22(char[] source, out Uuid7 result) {
#endif
        var alphabetDict = Base58AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in source) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Base58Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 22) { result = Empty; return false; }

#if NET6_0_OR_GREATER
        var byteCount = number.GetByteCount(isUnsigned: true);
        Span<byte> buffer = stackalloc byte[byteCount];
        number.TryWriteBytes(buffer, out _, isUnsigned: true, isBigEndian: true);

        if (buffer.Length < 16) {
            Span<byte> newBuffer = stackalloc byte[16];
            buffer.CopyTo(newBuffer[(16 - buffer.Length)..]);
            buffer = newBuffer;
        }
#else
        byte[] numberBytes = number.ToByteArray();
        if (BitConverter.IsLittleEndian) { Array.Reverse(numberBytes); }
        var buffer = new byte[16];
        if (numberBytes.Length > 16) {
            Buffer.BlockCopy(numberBytes, numberBytes.Length - 16, buffer, 0, 16);
        } else {
            Buffer.BlockCopy(numberBytes, 0, buffer, 16 - numberBytes.Length, numberBytes.Length);
        }

        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, 16 - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
#endif

        result = new Uuid7(buffer);
        return true;
    }

}
