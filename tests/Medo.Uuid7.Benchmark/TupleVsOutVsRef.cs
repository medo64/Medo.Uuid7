namespace Uuid7Benchmark;
using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class TupleVsOutVsRef {

    private static char[] Chars = new char[16];

    [Benchmark(Baseline = true)]
    public void FillViaTuple() {
        (Chars[0], Chars[1]) = ToTwoHexBytesViaTuple(42);
        (Chars[2], Chars[3]) = ToTwoHexBytesViaTuple(42);
        (Chars[4], Chars[5]) = ToTwoHexBytesViaTuple(42);
        (Chars[6], Chars[7]) = ToTwoHexBytesViaTuple(42);
        (Chars[8], Chars[9]) = ToTwoHexBytesViaTuple(42);
        (Chars[10], Chars[11]) = ToTwoHexBytesViaTuple(42);
        (Chars[12], Chars[13]) = ToTwoHexBytesViaTuple(42);
        (Chars[14], Chars[15]) = ToTwoHexBytesViaTuple(42);
    }

    [Benchmark()]
    public void FillViaOut() {
        ToTwoHexBytesViaOut(42, out Chars[0], out Chars[1]);
        ToTwoHexBytesViaOut(42, out Chars[2], out Chars[3]);
        ToTwoHexBytesViaOut(42, out Chars[4], out Chars[5]);
        ToTwoHexBytesViaOut(42, out Chars[6], out Chars[7]);
        ToTwoHexBytesViaOut(42, out Chars[8], out Chars[9]);
        ToTwoHexBytesViaOut(42, out Chars[10], out Chars[11]);
        ToTwoHexBytesViaOut(42, out Chars[12], out Chars[13]);
        ToTwoHexBytesViaOut(42, out Chars[14], out Chars[15]);
    }

    [Benchmark()]
    public void FillViaRef() {
        ToTwoHexBytesViaRef(42, ref Chars[0], ref Chars[1]);
        ToTwoHexBytesViaRef(42, ref Chars[2], ref Chars[3]);
        ToTwoHexBytesViaRef(42, ref Chars[4], ref Chars[5]);
        ToTwoHexBytesViaRef(42, ref Chars[6], ref Chars[7]);
        ToTwoHexBytesViaRef(42, ref Chars[8], ref Chars[9]);
        ToTwoHexBytesViaRef(42, ref Chars[10], ref Chars[11]);
        ToTwoHexBytesViaRef(42, ref Chars[12], ref Chars[13]);
        ToTwoHexBytesViaRef(42, ref Chars[14], ref Chars[15]);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (char, char) ToTwoHexBytesViaTuple(byte b) {
        return ((char)(b >> 4), (char)(b & 0x0F));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ToTwoHexBytesViaOut(byte b, out char o1, out char o2) {
        o1 = (char)(b >> 4);
        o2 = (char)(b & 0x0F);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ToTwoHexBytesViaRef(byte b, ref char o1, ref char o2) {
        o1 = (char)(b >> 4);
        o2 = (char)(b & 0x0F);
    }

}
