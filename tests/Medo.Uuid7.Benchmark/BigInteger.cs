namespace Uuid7Benchmark;
using System;
using System.Numerics;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Medo;

public class Integer {

    private Uuid7 ExampleUuid7 =  Uuid7.NewUuid7();

    [Benchmark(Baseline = true)]
    public BigInteger ToBigInteger() {
        return new BigInteger(ExampleUuid7.ToByteArray(), isUnsigned: true, isBigEndian: true);
    }

    [Benchmark()]
    public UInt128 ToUInt128() {
        Span<byte> bytes = ExampleUuid7.ToByteArray();
        var a = System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(bytes);
        var b = System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(bytes.Slice(8, 8));
        return new UInt128(a, b);
    }


    [Benchmark]
    public void BigIntegerToId22() {
        var destination = new char[22];
        BigIntegerTest.TryWriteAsId22(destination, ExampleUuid7.ToByteArray(), out _);
    }

    [Benchmark]
    public void UInt128ToId22() {
        var destination = new char[22];
        UInt128Test.TryWriteAsId22(destination, ExampleUuid7.ToByteArray(), out _);
    }


    private static class BigIntegerTest {

        internal static bool TryWriteAsId22(Span<char> destination, byte[] bytes, out int charsWritten) {
            if (destination.Length < 22) { charsWritten = 0; return false; }

            var number = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
            for (var i = 21; i >= 0; i--) {
                number = BigInteger.DivRem(number, Base58Modulo, out var remainder);
                destination[i] = Base58Alphabet[(int)remainder];
            }

            charsWritten = 22;
            return true;
        }

        private static readonly BigInteger Base58Modulo = 58;
        private static readonly char[] Base58Alphabet = new char[] {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
            'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
            'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };
    }

    private static class UInt128Test {

        internal static bool TryWriteAsId22(Span<char> destination, byte[] bytes, out int charsWritten) {
            if (destination.Length < 22) { charsWritten = 0; return false; }

            var a = System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(bytes);
            var b = System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(bytes[8..]);
            var number = new UInt128(a, b);
            for (var i = 21; i >= 0; i--) {
                (number, var remainder) = UInt128.DivRem(number, Base58Modulo);
                destination[i] = Base58Alphabet[(int)remainder];
            }

            charsWritten = 22;
            return true;
        }

        private static readonly UInt128 Base58Modulo = 58;
        private static readonly char[] Base58Alphabet = new char[] {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
            'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
            'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };
    }
}
