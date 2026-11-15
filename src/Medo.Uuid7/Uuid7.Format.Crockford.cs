/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public readonly partial struct Uuid7 {

    /// <summary>
    /// Returns UUID parsed from a given input.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 ParseCrockford(string s) {
        return ParseCrockford(s, provider: null);
    }

    /// <summary>
    /// Returns UUID parsed from a given input.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <exception cref="ArgumentNullException">Input cannot be null.</exception>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 ParseCrockford(string s, IFormatProvider? provider) {
        if (s is null) { throw new ArgumentNullException(nameof(s), "Input cannot be null."); }
        if (TryParseCrockford(s, provider, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }

#if NET7_0_OR_GREATER

    /// <summary>
    /// Returns UUID parsed from a given input.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 ParseCrockford(ReadOnlySpan<char> s, IFormatProvider? provider) {
        if (TryParseCrockford(s, provider, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }

#endif


    /// <summary>
    /// Returns true if UUID was successfully parsed.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing or an undefined value on failure.</param>
#if NET6_0_OR_GREATER
    public static bool TryParseCrockford([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Uuid7 result) {
#else
    public static bool TryParseCrockford(string? s, out Uuid7 result) {
#endif
        return TryParseCrockford(s, provider: null, out result);
    }

    /// <summary>
    /// Returns true if UUID was successfully parsed.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing or an undefined value on failure.</param>
#pragma warning disable IDE0060  // Remove unused parameter
#if NET6_0_OR_GREATER
    public static bool TryParseCrockford([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Uuid7 result) {
        if (s == null) { result = Empty; return false; }
        return TryParseAsCrockford(s.AsSpan(), out result);
    }
#else
    public static bool TryParseCrockford(string? s, IFormatProvider? provider, out Uuid7 result) {
        if (s == null) { result = Empty; return false; }
        return TryParseAsCrockford(s.ToCharArray(), out result);
    }
#endif
#pragma warning restore IDE0060  // Remove unused parameter

#if NET7_0_OR_GREATER

    /// <summary>
    /// Returns true if UUID was successfully parsed.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing or an undefined value on failure.</param>
    public static bool TryParseCrockford(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Uuid7 result) {
        return TryParseAsCrockford(s, out result);
    }

#endif


    private static readonly char[] CrockfordLowercaseAlphabet = [
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'a', 'b', 'c', 'd', 'e', 'f',
        'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q',
        'r', 's', 't', 'v', 'w', 'x', 'y', 'z',
    ];
    private static readonly char[] CrockfordUppercaseAlphabet = [
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
        'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q',
        'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z',
    ];
    private static readonly Lazy<Dictionary<char, int>> CrockfordAlphabetDict = new(() => {
        var dict = new Dictionary<char, int>(CrockfordLowercaseAlphabet.Length);
        for (var i = 0; i < CrockfordLowercaseAlphabet.Length; i++) {
            var ch = CrockfordLowercaseAlphabet[i];
            dict.Add(ch, i);
            if (char.IsLetter(ch)) {  // case-insensitive
                dict.Add(char.ToUpperInvariant(ch), i);
            }
        }
        dict.Add('o', 0); dict.Add('O', 0);
        dict.Add('i', 1); dict.Add('I', 1); dict.Add('l', 1); dict.Add('L', 1);
        return dict;
    });


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static bool TryWriteAsCrockford(bool uppercase, bool includeChecksum, Span<char> destination, ReadOnlySpan<byte> bytes, out int charsWritten) {
#else
    private static bool TryWriteAsCrockford(bool uppercase, bool includeChecksum, char[] destination, byte[] bytes, out int charsWritten) {
#endif
        if (destination.Length < 26) { charsWritten = 0; return false; }

        if (bytes == null) { bytes = MinValue.Bytes; }
        if (includeChecksum) {
            TryWriteAsCrockfordWithChecksum(uppercase ? CrockfordUppercaseAlphabet : CrockfordLowercaseAlphabet, destination, bytes);
        } else {
            TryWriteAsCrockfordWithoutChecksum(uppercase ? CrockfordUppercaseAlphabet : CrockfordLowercaseAlphabet, destination, bytes);
        }

        charsWritten = 26;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static void TryWriteAsCrockfordWithoutChecksum(char[] alphabet, Span<char> destination, ReadOnlySpan<byte> bytes) {
#else
    private static void TryWriteAsCrockfordWithoutChecksum(char[] alphabet, char[] destination, byte[] bytes) {
#endif
        var reservoir = 0;
        var level = 0;
        var sourceIndex = 0;
        for (var i = 0; i < 26; i++) {
            if (level < 5) {  // load new byte
                byte data;
                if (sourceIndex < 16) {
                    data = bytes[sourceIndex++];
                } else {
                    data = 0;
                }
                reservoir = (reservoir << 8) | data;
                level += 8;
            }
            level -= 5;
            var index = (reservoir >> level) & 0b11111;

            destination[i] = alphabet[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET6_0_OR_GREATER
    private static void TryWriteAsCrockfordWithChecksum(char[] alphabet, Span<char> destination, ReadOnlySpan<byte> bytes) {
#else
    private static void TryWriteAsCrockfordWithChecksum(char[] alphabet, char[] destination, byte[] bytes) {
#endif
        var sumA = 1;
        var sumB = 0;
        var reservoir = 0;
        var level = 0;
        var sourceIndex = 0;
        for (var i = 0; i < 26; i++) {
            if (level < 5) {  // load new byte
                byte data;
                if (sourceIndex < 16) {
                    data = bytes[sourceIndex++];
                    sumA += data;
                    sumB += sumA;
                } else {
                    data = (byte)((1 + sumB % 3) << 6);
                }
                reservoir = (reservoir << 8) | data;
                level += 8;
            }
            level -= 5;
            var index = (reservoir >> level) & 0b11111;

            destination[i] = alphabet[index];
        }
    }


#if NET6_0_OR_GREATER
    private static bool TryParseAsCrockford(ReadOnlySpan<char> source, out Uuid7 result) {
#else
    private static bool TryParseAsCrockford(char[] source, out Uuid7 result) {
#endif

        var buffer = new byte[16];
        var outCount = 0;
        var alphabetDict = CrockfordAlphabetDict.Value;
        var inCount = 0;
        var sumA = 1;
        var sumB = 0;
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
                    sumA += data;
                    sumB += sumA;
                    if (outCount == 16) {
                        var expected = (byte)(1 + sumB % 3);
                        var checksum = (byte)(reservoir & 0b11);
                        if ((checksum != 0) && (checksum != expected)) {  // invalid checksum; fill result anyhow
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
