/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

public readonly partial struct Uuid7 {

    #region Endianess

    private static readonly bool IsBigEndian = !BitConverter.IsLittleEndian;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseGuidEndianess(ref byte[] bytes) {
        (bytes[0], bytes[1], bytes[2], bytes[3]) = (bytes[3], bytes[2], bytes[1], bytes[0]);
        (bytes[4], bytes[5]) = (bytes[5], bytes[4]);
        (bytes[6], bytes[7]) = (bytes[7], bytes[6]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte[] ReverseGuidEndianess(byte[] bytes) {
        var guidBytes = new byte[16];
        guidBytes[0] = bytes[3]; guidBytes[1] = bytes[2]; guidBytes[2] = bytes[1]; guidBytes[3] = bytes[0];
        guidBytes[4] = bytes[5]; guidBytes[5] = bytes[4];
        guidBytes[6] = bytes[7]; guidBytes[7] = bytes[6];
        guidBytes[8] = bytes[8]; guidBytes[9] = bytes[9]; guidBytes[10] = bytes[10]; guidBytes[11] = bytes[11];
        guidBytes[12] = bytes[12]; guidBytes[13] = bytes[13]; guidBytes[14] = bytes[14]; guidBytes[15] = bytes[15];
        return guidBytes;
    }

    #endregion Endianess


    #region Helpers

    private const long UnixEpochMilliseconds = 62_135_596_800_000;
    private const long TicksPerMillisecond = 10_000;

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int CompareArrays(ReadOnlySpan<byte> buffer1, ReadOnlySpan<byte> buffer2) {
        if ((buffer1 != null) && (buffer2 != null)) {  // protecting against EF or similar API that uses reflection (https://github.com/medo64/Medo.Uuid7/issues/1)
            return buffer1.SequenceCompareTo(buffer2);
#else
    private static int CompareArrays(byte[] buffer1, byte[] buffer2) {
        if ((buffer1 != null) && (buffer2 != null)) {
            var comparer = Comparer<byte>.Default;
            for (int i = 0; i < buffer1.Length; i++) {
                if (comparer.Compare(buffer1[i], buffer2[i]) < 0) { return -1; }
                if (comparer.Compare(buffer1[i], buffer2[i]) > 0) { return +1; }
            }
#endif
#if NET8_0_OR_GREATER
        } else if (buffer1 != null) {
            if (buffer1.IndexOfAnyExcept((byte)0) >= 0) { return +1; }
        } else if (buffer2 != null) {
            if (buffer2.IndexOfAnyExcept((byte)0) >= 0) { return -1; }
        }
#else
        } else if (buffer1 != null) {
            for (int i = 0; i < buffer1.Length; i++) {
                if (buffer1[i] != 0) { return +1; }
            }
        } else if (buffer2 != null) {
            for (int i = 0; i < buffer2.Length; i++) {
                if (buffer2[i] != 0) { return -1; }
            }
        }
#endif
        return 0;  // object are equal
    }

    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();  // needed due to .NET Standard 2.0
#if !UUID7_NO_RANDOM_BUFFER
    private const int RandomBufferSize = 2048;
    private static readonly ThreadLocal<byte[]> RandomBuffer = new(() => {
#if !NETSTANDARD
        return GC.AllocateArray<byte>(RandomBufferSize, pinned: true);
#else  // no pinning in case of .NET Standard (legacy support)
        return new byte[RandomBufferSize];
#endif
    });
    private static readonly ThreadLocal<int> RandomBufferIndex = new(() => RandomBufferSize);  // first call needs to fill buffer no matter what
#endif

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static void GetRandomBytes(ref byte[] bytes, int offset, int count) {
#if !UUID7_NO_RANDOM_BUFFER
        var buffer = RandomBuffer.Value!;
        var bufferIndex = RandomBufferIndex.Value;

        if (unchecked(bufferIndex + count) > RandomBufferSize) {
            var leftover = unchecked(RandomBufferSize - bufferIndex);
            Buffer.BlockCopy(buffer, bufferIndex, bytes, offset, leftover);  // make sure to use all bytes
            offset = unchecked(offset + leftover);
            count = unchecked(count - leftover);

            Random.GetBytes(buffer);
            bufferIndex = 0;
        }

        Buffer.BlockCopy(buffer, bufferIndex, bytes, offset, count);
        RandomBufferIndex.Value = unchecked(bufferIndex + count);
#else
        Random.GetBytes(bytes, offset, count);
#endif
    }


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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsId25(Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsId25(char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 25) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }

#if NET6_0_OR_GREATER
        var number = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
#else
        var bytesEx = new byte[17];
        Buffer.BlockCopy(bytes, 0, bytesEx, 1, 16);
        if (BitConverter.IsLittleEndian) { Array.Reverse(bytesEx); }
        var number = new BigInteger(bytesEx);
#endif
        for (var i = 24; i >= 0; i--) {
            number = BigInteger.DivRem(number, Base35Modulo, out var remainder);
            destination[i] = Base35Alphabet[(int)remainder];
        }

        charsWritten = 25;
        return true;
    }

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

    private static readonly BigInteger Base16Modulo = 16;
    private static readonly char[] Base16Alphabet = [
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
    ];
    private static readonly Lazy<Dictionary<char, BigInteger>> Base16AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>(Base16Alphabet.Length);
        for (var i = 0; i < Base16Alphabet.Length; i++) {
            var ch = Base16Alphabet[i];
            dict.Add(ch, i);
            if (char.IsLetter(ch)) { dict.Add(char.ToUpperInvariant(ch), i); }  // case-insensitive
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
        var number = new BigInteger();
        foreach (var ch in source) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Base16Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 32) { result = Uuid7.Empty; return false; }

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
        return dict;
    });

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

    private static readonly BigInteger Base35Modulo = 35;
    private static readonly char[] Base35Alphabet = [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
        'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
        'v', 'w', 'x', 'y', 'z'
    ];
    private static readonly Lazy<Dictionary<char, BigInteger>> Base35AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>(Base35Alphabet.Length);
        for (var i = 0; i < Base35Alphabet.Length; i++) {
            var ch = Base35Alphabet[i];
            dict.Add(ch, i);
            if (char.IsLetter(ch)) {  // case-insensitive
                dict.Add(char.ToUpperInvariant(ch), i);
            }
        }
        return dict;
    });

#if NET6_0_OR_GREATER
    private static bool TryParseAsId25(ReadOnlySpan<char> source, out Uuid7 result) {
#else
    private static bool TryParseAsId25(char[] source, out Uuid7 result) {
#endif
        var alphabetDict = Base35AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in source) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Base35Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 25) { result = Empty; return false; }

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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (char, char) ToTwoHexBytes(byte b) {
        return (Base16Alphabet[b >> 4], Base16Alphabet[b & 0x0F]);
    }

    #endregion Helpers

}
