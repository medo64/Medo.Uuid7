namespace Uuid7Benchmark;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class CompareArrays
{
    private byte[][] _bytes = Array.Empty<byte[]>();

    [Params(100, 1000, 10000)]
    public int Iterations { get; set; }


    [Benchmark(Baseline = true)]
    public void Original()
    {
        for (var i = 0; i < Iterations; i++)
        {
            CompareArrays_Orig(_bytes[i], _bytes[i]);
            CompareArrays_Orig(_bytes[i], _bytes[^(i + 1)]);
        }
    }

    [Benchmark]
    public void Switch()
    {
        for (var i = 0; i < Iterations; i++)
        {
            CompareArrays_Switch(_bytes[i], _bytes[i]);
            CompareArrays_Switch(_bytes[i], _bytes[^(i + 1)]);
        }
    }

    [Benchmark]
    public void SwitchSpan()
    {
        for (var i = 0; i < Iterations; i++)
        {
            CompareArrays_SwitchSpan(_bytes[i], _bytes[i]);
            CompareArrays_SwitchSpan(_bytes[i], _bytes[^(i + 1)]);
        }
    }

    [Benchmark]
    public void SpanSequence()
    {
        for (var i = 0; i < Iterations; i++)
        {
            CompareArrays_SpanSequence(_bytes[i], _bytes[i]);
            CompareArrays_SpanSequence(_bytes[i], _bytes[^(i + 1)]);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        _bytes = new byte[Iterations][];
        for (var i = 0; i < Iterations; i++)
        {
            _bytes[i] = Medo.Uuid7.NewUuid7().ToByteArray();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int CompareArrays_Orig(byte[] buffer1, byte[] buffer2)
    {
        if ((buffer1 != null) && (buffer2 != null) && (buffer1.Length == 16) && (buffer2.Length == 16))
        {  // protecting against EF or similar API that uses reflection (https://github.com/medo64/Medo.Uuid7/issues/1)
            var comparer = Comparer<byte>.Default;
            for (int i = 0; i < buffer1.Length; i++)
            {
                if (comparer.Compare(buffer1[i], buffer2[i]) < 0) { return -1; }
                if (comparer.Compare(buffer1[i], buffer2[i]) > 0) { return +1; }
            }
        }
        else if ((buffer1 == null) || (buffer1.Length != 16))
        {
            return -1;
        }
        else if ((buffer2 == null) || (buffer2.Length != 16))
        {
            return +1;
        }

        return 0;  // object are equal
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int CompareArrays_Switch(byte[] buffer1, byte[] buffer2)
    {
        if ((buffer1 != null) && (buffer2 != null) && (buffer1.Length == 16) && (buffer2.Length == 16))
        {  // protecting against EF or similar API that uses reflection (https://github.com/medo64/Medo.Uuid7/issues/1)
            var comparer = Comparer<byte>.Default;
            for (int i = 0; i < buffer1.Length; i++)
            {
                switch (comparer.Compare(buffer1[i], buffer2[i]))
                {
                    case < 0: return -1;
                    case > 0: return +1;
                }
            }
        }
        else if ((buffer1 == null) || (buffer1.Length != 16))
        {
            return -1;
        }
        else if ((buffer2 == null) || (buffer2.Length != 16))
        {
            return +1;
        }

        return 0;  // object are equal
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int CompareArrays_SwitchSpan(ReadOnlySpan<byte> buffer1, ReadOnlySpan<byte> buffer2)
    {
        if ((buffer1.Length == 16) && (buffer2.Length == 16))
        {  // protecting against EF or similar API that uses reflection (https://github.com/medo64/Medo.Uuid7/issues/1)
            var comparer = Comparer<byte>.Default;
            for (int i = 0; i < buffer1.Length; i++)
            {
                switch (comparer.Compare(buffer1[i], buffer2[i]))
                {
                    case < 0: return -1;
                    case > 0: return +1;
                }
            }
        }
        else if (buffer1.Length != 16)
        {
            return -1;
        }
        else if (buffer2.Length != 16)
        {
            return +1;
        }

        return 0;  // object are equal
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int CompareArrays_SpanSequence(ReadOnlySpan<byte> buffer1, ReadOnlySpan<byte> buffer2)
    {
        if (buffer1.Length == 16 && buffer2.Length == 16)
        {
            return buffer1.SequenceCompareTo(buffer2);
        }
        else if (buffer1.Length != 16)
        {
            return -1;
        }
        else if (buffer2.Length != 16)
        {
            return +1;
        }

        return 0;  // object are equal
    }

}
