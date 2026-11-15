/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

public readonly partial struct Uuid7 {

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
    private static (char, char) ToTwoHexBytes(byte b) {
        return (Base16Alphabet[b >> 4], Base16Alphabet[b & 0x0F]);
    }

}
