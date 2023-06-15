/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2023-06-14: Added buffer for random bytes
//2023-06-07: Minor optimizations
//            Added NewUuid4() method
//2023-05-17: Support for .NET Standard 2.0
//            ToString() performance improvements
//2023-05-16: Performance improvements
//2023-04-12: Timestamps are monotonically increasing even if time goes backward
//2023-01-14: Using random monotonic counter increment
//2023-01-12: Expanded monotonic counter from 18 to 26 bits
//            Added ToId22String and FromId22String methods
//            Moved to semi-random increment within the same millisecond
//2023-01-11: Expanded monotonic counter from 12 to 18 bits
//            Added ToId25String and FromId25String methods
//            Added FromString method
//2022-12-31: Initial version

namespace Medo;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

#if NET6_0_OR_GREATER
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
#endif

/// <summary>
/// Implements UUID version 7 as defined in RFC draft at
/// https://www.ietf.org/archive/id/draft-ietf-uuidrev-rfc4122bis-03.html.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Uuid7 : IComparable<Guid>, IComparable<Uuid7>, IEquatable<Uuid7>, IEquatable<Guid>, IFormattable {

    /// <summary>
    /// Creates a new instance filled with version 7 UUID.
    /// </summary>
    public Uuid7() {
        Bytes = new byte[16];
        FillBytes7(ref Bytes);
    }

    /// <summary>
    /// Creates a new instance from given byte array.
    /// No check if array is version 7 UUID is made.
    /// </summary>
    /// <exception cref="ArgumentNullException">Buffer cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Buffer must be exactly 16 bytes in length.</exception>
    public Uuid7(byte[] buffer) {
        if (buffer == null) { throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null."); }
        if (buffer.Length != 16) { throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer must be exactly 16 bytes in length."); }
        Bytes = new byte[16];
        Buffer.BlockCopy(buffer, 0, Bytes, 0, 16);
    }

    /// <summary>
    /// Creates a new instance from given GUID bytes.
    /// No check if GUID is version 7 UUID is made.
    /// </summary>
    public Uuid7(Guid guid) {
        Bytes = guid.ToByteArray();
    }

    /// <summary>
    /// Creates a new instance with a given byte array.
    /// No check if array is version 7 UUID is made.
    /// No check for array length is made.
    /// </summary>
    private Uuid7(ref byte[] buffer) {
        Bytes = buffer;
    }


    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    private readonly byte[] Bytes;


    #region Implemenation (v7)

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static void FillBytes7(ref byte[] bytes) {
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var ticks = DateTime.UtcNow.Ticks;  // DateTime is a smidgen faster than DateTimeOffset
        var millisecond = unchecked(ticks / TicksPerMillisecond);
        var msCounter = MillisecondCounter;

        var newStep = (millisecond != LastMillisecond);
        if (newStep) {  // we need to switch millisecond (i.e. counter)
            LastMillisecond = millisecond;
            long ms;
            ms = unchecked(millisecond - UnixEpochMilliseconds);
            if (msCounter < ms) {  // normal time progression
                msCounter = ms;
            } else {  // time went backward, just increase counter
                unchecked { msCounter++; }
            }
            MillisecondCounter = msCounter;
        }

        // Timestamp
        bytes[0] = (byte)(msCounter >> 40);
        bytes[1] = (byte)(msCounter >> 32);
        bytes[2] = (byte)(msCounter >> 24);
        bytes[3] = (byte)(msCounter >> 16);
        bytes[4] = (byte)(msCounter >> 8);
        bytes[5] = (byte)msCounter;

        // Randomness
        uint monoCounter;
        if (newStep) {
            GetRandomBytes(ref bytes, 6, 10);
            monoCounter = (uint)(((bytes[6] & 0x07) << 22) | (bytes[7] << 14) | ((bytes[8] & 0x3F) << 8) | bytes[9]);  // to use as monotonic random for future calls; total of 26 bits but only 25 are used initially with upper 1 bit reserved for rollover guard
        } else {
            GetRandomBytes(ref bytes, 9, 7);
            monoCounter = unchecked(MonotonicCounter + ((uint)bytes[9] >> 4) + 1);  // 4 bit random increment will reduce overall counter space by 3 bits on average (to 2^22 combinations)
            bytes[7] = (byte)(monoCounter >> 14);    // bits 14:21 of monotonics counter
            bytes[9] = (byte)(monoCounter);          // bits 0:7 of monotonics counter
        }
        MonotonicCounter = monoCounter;

        //Fixup
        bytes[6] = (byte)(0x70 | ((monoCounter >> 22) & 0x0F));  // set 4-bit version + bits 22:25 of monotonics counter
        bytes[8] = (byte)(0x80 | ((monoCounter >> 8) & 0x3F));   // set 2-bit variant + bits 8:13 of monotonics counter
    }


    [ThreadStatic]
    private static long LastMillisecond;  // real time in milliseconds since 0001-01-01

    [ThreadStatic]
    private static long MillisecondCounter;  // usually real time but doesn't go backward

    [ThreadStatic]
    private static uint MonotonicCounter;  // counter that gets embedded into UUID

    #endregion Implemenation (v7)

    #region Implemenation (v4)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FillBytes4(ref byte[] bytes) {
        GetRandomBytes(ref bytes, 0, 16);

        //Fixup
        bytes[6] = (byte)(0x40 | (bytes[6] & 0x0F));  // set 4-bit version
        bytes[8] = (byte)(0x20 | (bytes[8] & 0x3F));  // set 2-bit variant
    }

    #endregion Implemenation


    /// <summary>
    /// Returns current UUID version 7 as binary equivalent System.Guid.
    /// </summary>
    public Guid ToGuid() {
        return new Guid(Bytes);
    }

    /// <summary>
    /// Returns an array that contains UUID bytes.
    /// </summary>
    public byte[] ToByteArray() {
        var copy = new byte[16];
        Buffer.BlockCopy(Bytes, 0, copy, 0, 16);
        return copy;
    }


    #region Id22

    private static readonly BigInteger Base58Modulo = 58;
    private static readonly char[] Base58Alphabet = new char[] {
        '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
        'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
        'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
        'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
        'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };
    private static readonly Lazy<Dictionary<char, BigInteger>> Base58AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>();
        for (var i = 0; i < Base58Alphabet.Length; i++) {
            dict.Add(Base58Alphabet[i], i);
        }
        return dict;
    });

    /// <summary>
    /// Returns UUID representation in Id22 format. This is base58 encoder
    /// using the same alphabet as bitcoin does.
    /// </summary>
    public string ToId22String() {
#if NET6_0_OR_GREATER
        var number = new BigInteger(Bytes, isUnsigned: true, isBigEndian: true);
#else
        var bytes = new byte[17];
        Buffer.BlockCopy(Bytes, 0, bytes, 1, 16);
        if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }
        var number = new BigInteger(bytes);
#endif
        var result = new char[22];  // always the same length
        for (var i = 21; i >= 0; i--) {
            number = BigInteger.DivRem(number, Base58Modulo, out var remainder);
            result[i] = Base58Alphabet[(int)remainder];
        }
        return new string(result);
    }

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to Id22 alphabet are ignored.
    /// Input must contain exactly 22 characters.
    /// </summary>
    /// <param name="id22Text">Id22 text.</param>
    /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
    /// <exception cref="FormatException">Input must be 22 characters.</exception>
    public static Uuid7 FromId22String(string id22Text) {
        if (id22Text == null) { throw new ArgumentNullException(nameof(id22Text), "Text cannot be null."); }

        var alphabetDict = Base58AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in id22Text) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Base58Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 22) { throw new FormatException("Input must be 22 characters."); }

#if NET6_0_OR_GREATER
        var buffer = number.ToByteArray(isUnsigned: true, isBigEndian: true);
#else
        byte[] numberBytes = number.ToByteArray();
        if (BitConverter.IsLittleEndian) { Array.Reverse(numberBytes); }
        var buffer = new byte[16];
        if (numberBytes.Length > 16) {
            Buffer.BlockCopy(numberBytes, numberBytes.Length - 16, buffer, 0, 16);
        } else {
            Buffer.BlockCopy(numberBytes, 0, buffer, 16 - numberBytes.Length, numberBytes.Length);
        }
#endif
        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, 16 - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
        return new Uuid7(buffer);
    }

    #endregion Id22

    #region Id25

    private static readonly BigInteger Base35Modulo = 35;
    private static readonly char[] Base35Alphabet = new char[] {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
        'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
        'v', 'w', 'x', 'y', 'z'
    };
    private static readonly Lazy<Dictionary<char, BigInteger>> Base35AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>();
        for (var i = 0; i < Base35Alphabet.Length; i++) {
            var ch = Base35Alphabet[i];
            dict.Add(ch, i);
            if (char.IsLetter(ch)) {  // case-insensitive
                dict.Add(char.ToUpperInvariant(ch), i);
            }
        }
        return dict;
    });

    /// <summary>
    /// Returns UUID representation in Id25 format.
    /// Please note that while conversion is the same as one in
    /// https://github.com/stevesimmons/uuid7-csharp/, UUIDs are not fully
    /// compatible and thus not necessarily interchangeable.
    /// </summary>
    public string ToId25String() {
#if NET6_0_OR_GREATER
        var number = new BigInteger(Bytes, isUnsigned: true, isBigEndian: true);
#else
        var bytes = new byte[17];
        Buffer.BlockCopy(Bytes, 0, bytes, 1, 16);
        if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }
        var number = new BigInteger(bytes);
#endif
        var result = new char[25];  // always the same length
        for (var i = 24; i >= 0; i--) {
            number = BigInteger.DivRem(number, Base35Modulo, out var remainder);
            result[i] = Base35Alphabet[(int)remainder];
        }
        return new string(result);
    }

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to Id25 alphabet are ignored.
    /// Input must contain exactly 25 characters.
    /// </summary>
    /// <param name="id25Text">Id25 text.</param>
    /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
    /// <exception cref="FormatException">Input must be 25 characters.</exception>
    public static Uuid7 FromId25String(string id25Text) {
        if (id25Text == null) { throw new ArgumentNullException(nameof(id25Text), "Text cannot be null."); }

        var alphabetDict = Base35AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in id25Text) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Base35Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 25) { throw new FormatException("Input must be 25 characters."); }

#if NET6_0_OR_GREATER
        var buffer = number.ToByteArray(isUnsigned: true, isBigEndian: true);
#else
        byte[] numberBytes = number.ToByteArray();
        if (BitConverter.IsLittleEndian) { Array.Reverse(numberBytes); }
        var buffer = new byte[16];
        if (numberBytes.Length > 16) {
            Buffer.BlockCopy(numberBytes, numberBytes.Length - 16, buffer, 0, 16);
        } else {
            Buffer.BlockCopy(numberBytes, 0, buffer, 16 - numberBytes.Length, numberBytes.Length);
        }
#endif
        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, 16 - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
        return new Uuid7(buffer);
    }

    #endregion Id25

    #region FromString

    private static readonly BigInteger Base16Modulo = 16;
    private static readonly char[] Base16Alphabet = new char[] {
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
    };
    private static readonly Lazy<Dictionary<char, BigInteger>> Base16AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>();
        for (var i = 0; i < Base16Alphabet.Length; i++) {
            var ch = Base16Alphabet[i];
            dict.Add(ch, i);
            if (char.IsLetter(ch)) {  // case-insensitive
                dict.Add(char.ToUpperInvariant(ch), i);
            }
        }
        return dict;
    });

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to hexadecimal alphabet are ignored.
    /// Input must contain exactly 32 characters.
    /// </summary>
    /// <param name="text">UUID text.</param>
    /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
    /// <exception cref="FormatException">Input must be 32 characters.</exception>
    public static Uuid7 FromString(string text) {
        if (text == null) { throw new ArgumentNullException(nameof(text), "Text cannot be null."); }

        var alphabetDict = Base16AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in text) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Base16Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 32) { throw new FormatException("Input must be 32 characters."); }

#if NET6_0_OR_GREATER
        var buffer = number.ToByteArray(isUnsigned: true, isBigEndian: true);
#else
        byte[] numberBytes = number.ToByteArray();
        if (BitConverter.IsLittleEndian) { Array.Reverse(numberBytes); }
        var buffer = new byte[16];
        if (numberBytes.Length > 16) {
            Buffer.BlockCopy(numberBytes, numberBytes.Length - 16, buffer, 0, 16);
        } else {
            Buffer.BlockCopy(numberBytes, 0, buffer, 16 - numberBytes.Length, numberBytes.Length);
        }
#endif
        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, 16 - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
        return new Uuid7(buffer);
    }

    #endregion FromString

    #region Overrides

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    public override bool Equals([NotNullWhen(true)] object? obj) {
#else
    public override bool Equals(object? obj) {
#endif
        if (obj is Uuid7 uuid) {
            return CompareArrays(Bytes, uuid.Bytes) == 0;
        } else if (obj is Guid guid) {
            return CompareArrays(Bytes, guid.ToByteArray()) == 0;
        }
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
        return Bytes.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString() {
        return ToDefaultString(Bytes);
    }

    #endregion Overrides

    #region IFormattable

    /// <summary>
    /// Formats the value of the current instance using the specified format.
    /// </summary>
    /// <param name="format">The format to use.</param>
#if NET7_0_OR_GREATER
    public string ToString([StringSyntax(StringSyntaxAttribute.GuidFormat)] string? format) {
#else
    public string ToString(string? format) {
#endif
        return ToString(format, formatProvider: null);
    }

    /// <inheritdoc/>
#if NET7_0_OR_GREATER
    public string ToString([StringSyntax(StringSyntaxAttribute.GuidFormat)] string? format, IFormatProvider? formatProvider) {
#else
    public string ToString(string? format, IFormatProvider? formatProvider) {  // formatProvider is ignored
#endif
        return format switch {  // treat uppercase and lowercase the same (compatibility with Guid ToFormat)
            "D" or "d" or "" or null => ToDefaultString(Bytes),
            "N" or "n" => ToNoHypensString(Bytes),
            "B" or "b" => ToBracesString(Bytes),
            "P" or "p" => ToParenthesesString(Bytes),
            "X" or "x" => ToHexadecimalString(Bytes),
            _ => throw new FormatException("Invalid UUID format."),
        };
    }

    private static string ToDefaultString(byte[] bytes) {
        var chars = new char[36];
        var j = 0;
        for (var i = 0; i < 16; i++) {
            chars[j + 0] = Base16Alphabet[bytes[i] >> 4];
            chars[j + 1] = Base16Alphabet[bytes[i] & 0x0F];
            if (i is 3 or 5 or 7 or 9) {
                chars[j + 2] = '-';
                j += 3;
            } else {
                j += 2;
            }
        }
        return new string(chars);
    }

    private static string ToNoHypensString(byte[] bytes) {
        var chars = new char[32];
        var j = 0;
        for (var i = 0; i < 16; i++) {
            chars[j + 0] = Base16Alphabet[bytes[i] >> 4];
            chars[j + 1] = Base16Alphabet[bytes[i] & 0x0F];
            j += 2;
        }
        return new string(chars);
    }

    private static string ToBracesString(byte[] bytes) {
        var chars = new char[38];
        chars[0] = '{';
        chars[37] = '}';
        var j = 1;
        for (var i = 0; i < 16; i++) {
            chars[j + 0] = Base16Alphabet[bytes[i] >> 4];
            chars[j + 1] = Base16Alphabet[bytes[i] & 0x0F];
            if (i is 3 or 5 or 7 or 9) {
                chars[j + 2] = '-';
                j += 3;
            } else {
                j += 2;
            }
        }
        return new string(chars);
    }

    private static string ToParenthesesString(byte[] bytes) {
        var chars = new char[38];
        chars[0] = '(';
        chars[37] = ')';
        var j = 1;
        for (var i = 0; i < 16; i++) {
            chars[j + 0] = Base16Alphabet[bytes[i] >> 4];
            chars[j + 1] = Base16Alphabet[bytes[i] & 0x0F];
            if (i is 3 or 5 or 7 or 9) {
                chars[j + 2] = '-';
                j += 3;
            } else {
                j += 2;
            }
        }
        return new string(chars);
    }

    private static string ToHexadecimalString(byte[] bytes) {
        var chars = new char[68];
        chars[0] = '{';
        chars[66] = '}';
        chars[67] = '}';
        var j = 1;
        for (var i = 0; i < 16; i++) {
            if (i is 0) {
                chars[j + 0] = '0';
                chars[j + 1] = 'x';
                j += 2;
            } else if (i is 4 or 6 or >= 9) {
                chars[j + 2] = ',';
                chars[j + 3] = '0';
                chars[j + 4] = 'x';
                j += 5;
            } else if (i is 8) {
                chars[j + 2] = ',';
                chars[j + 3] = '{';
                chars[j + 4] = '0';
                chars[j + 5] = 'x';
                j += 6;
            } else {
                j += 2;
            }
            chars[j + 0] = Base16Alphabet[bytes[i] >> 4];
            chars[j + 1] = Base16Alphabet[bytes[i] & 0x0F];
        }
        return new string(chars);
    }

    #endregion IFormattable

    #region Operators

    /// <inheritdoc/>
    public static bool operator ==(Uuid7 left, Uuid7 right) {
        return left.Equals(right);
    }

    /// <inheritdoc/>
    public static bool operator ==(Uuid7 left, Guid right) {
        return left.Equals(right);
    }

    /// <inheritdoc/>
    public static bool operator ==(Guid left, Uuid7 right) {
        return left.Equals(right);
    }

    /// <inheritdoc/>
    public static bool operator !=(Uuid7 left, Uuid7 right) {
        return !(left == right);
    }

    /// <inheritdoc/>
    public static bool operator !=(Uuid7 left, Guid right) {
        return !(left == right);
    }

    /// <inheritdoc/>
    public static bool operator !=(Guid left, Uuid7 right) {
        return !(left == right);
    }

    /// <inheritdoc/>
    public static bool operator <(Uuid7 left, Uuid7 right) {
        return left.CompareTo(right) < 0;
    }

    /// <inheritdoc/>
    public static bool operator <(Uuid7 left, Guid right) {
        return left.CompareTo(right) < 0;
    }

    /// <inheritdoc/>
    public static bool operator <(Guid left, Uuid7 right) {
        return left.CompareTo(right) < 0;
    }

    /// <inheritdoc/>
    public static bool operator >(Uuid7 left, Uuid7 right) {
        return left.CompareTo(right) > 0;
    }

    /// <inheritdoc/>
    public static bool operator >(Uuid7 left, Guid right) {
        return left.CompareTo(right) > 0;
    }

    /// <inheritdoc/>
    public static bool operator >(Guid left, Uuid7 right) {
        return left.CompareTo(right) > 0;
    }


    /// <inheritdoc/>
    public static bool operator <=(Uuid7 left, Uuid7 right) {
        return left.CompareTo(right) is < 0 or 0;
    }

    /// <inheritdoc/>
    public static bool operator <=(Uuid7 left, Guid right) {
        return left.CompareTo(right) is < 0 or 0;
    }

    /// <inheritdoc/>
    public static bool operator <=(Guid left, Uuid7 right) {
        return left.CompareTo(right) is < 0 or 0;
    }

    /// <inheritdoc/>
    public static bool operator >=(Uuid7 left, Uuid7 right) {
        return left.CompareTo(right) is > 0 or 0;
    }

    /// <inheritdoc/>
    public static bool operator >=(Uuid7 left, Guid right) {
        return left.CompareTo(right) is > 0 or 0;
    }

    /// <inheritdoc/>
    public static bool operator >=(Guid left, Uuid7 right) {
        return left.CompareTo(right) is > 0 or 0;
    }

    #endregion Operators

    #region IComparable<Guid>

    /// <inheritdoc/>
    public int CompareTo(Guid other) {
        return CompareArrays(Bytes, other.ToByteArray());
    }

    #endregion IComparable<Guid>

    #region IComparable<Uuid>

    /// <inheritdoc/>
    public int CompareTo(Uuid7 other) {
        return CompareArrays(Bytes, other.Bytes);
    }

    #endregion IComparable<Uuid>

    #region IEquatable<Uuid>

    /// <inheritdoc/>
    public bool Equals(Uuid7 other) {
        return CompareArrays(Bytes, other.Bytes) == 0;
    }

    #endregion IEquatable<Uuid>

    #region IEquatable<Guid>

    /// <inheritdoc/>
    public bool Equals(Guid other) {
        return CompareArrays(Bytes, other.ToByteArray()) == 0;
    }

    #endregion IEquatable<Guid>


    #region Static

    /// <summary>
    /// A read-only instance of the Guid structure whose value is all zeros.
    /// Please note this is not a valid UUID7 as it lacks its version bits.
    /// </summary>
    public static readonly Uuid7 Empty = new(new byte[16]);

    /// <summary>
    /// A read-only instance of the Guid structure whose value is all 1's.
    /// Please note this is not a valid UUID7 as it lacks its version bits.
    /// </summary>
    public static readonly Uuid7 Max = new(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 });


    /// <summary>
    /// Returns new UUID version 7.
    /// </summary>
    public static Uuid7 NewUuid7() {
        return new Uuid7();
    }

    /// <summary>
    /// Returns new UUID version 4.
    /// </summary>
    public static Uuid7 NewUuid4() {
        var bytes = new byte[16];
        FillBytes4(ref bytes);
        return new Uuid7(ref bytes);
    }


#if NET6_0_OR_GREATER
    /// <summary>
    /// Fills a span with UUIDs.
    /// </summary>
    /// <param name="data">The span to fill.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static void Fill(Span<Uuid7> data) {
        if (data == null) { throw new ArgumentNullException(nameof(data), "Data cannot be null."); }
        for (var i = 0; i < data.Length; i++) {
            data[i] = NewUuid7();
        }
    }
#endif

    #endregion Static


    #region Helpers

    private const long UnixEpochMilliseconds = 62_135_596_800_000;
    private const long TicksPerMillisecond = 10_000;

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static int CompareArrays(byte[] buffer1, byte[] buffer2) {
        if ((buffer1 != null) && (buffer2 != null) && (buffer1.Length == 16) && (buffer2.Length == 16)) {  // protecting against EF or similar API that uses reflection (https://github.com/medo64/Medo.Uuid7/issues/1)
            var comparer = Comparer<byte>.Default;
            for (int i = 0; i < buffer1.Length; i++) {
                if (comparer.Compare(buffer1[i], buffer2[i]) < 0) { return -1; }
                if (comparer.Compare(buffer1[i], buffer2[i]) > 0) { return +1; }
            }
        } else if ((buffer1 == null) || (buffer1.Length != 16)) {
            return -1;
        } else if ((buffer2 == null) || (buffer2.Length != 16)) {
            return +1;
        }

        return 0;  // object are equal
    }


    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();  // needed due to .NET Standard 2.0
#if !UUID7_NO_RANDOM_BUFFER
    private static readonly object RandomBufferLock = new();
    private static readonly byte[] RandomBuffer = new byte[1024];
    private static int RandomBufferIndex = RandomBuffer.Length;  // first call needs to fill buffer no matter what
#endif

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static void GetRandomBytes(ref byte[] bytes, int offset, int count) {
#if !UUID7_NO_RANDOM_BUFFER
        lock (RandomBufferLock) {
            if (RandomBufferIndex + count > RandomBuffer.Length) {  // ignore any leftover bytes
                Random.GetBytes(RandomBuffer);
                RandomBufferIndex = 0;
            }
            Buffer.BlockCopy(RandomBuffer, RandomBufferIndex, bytes, offset, count);
            RandomBufferIndex += count;
        }
#else
        Random.GetBytes(bytes, offset, count);
#endif
    }

    #endregion Helpers

}
