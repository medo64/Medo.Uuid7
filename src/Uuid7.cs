/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/// <summary>
/// Implements UUID version 7 as defined in RFC draft at
/// https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-format-04.html.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Uuid7 : IComparable<Guid>, IComparable<Uuid7>, IEquatable<Uuid7>, IEquatable<Guid> {

    /// <summary>
    /// Creates a new instance filled with version 7 UUID.
    /// </summary>
    public Uuid7() {
        Bytes = new byte[16];

        var ticks = DateTime.UtcNow.Ticks;  // DateTime is a smidgen faster than DateTimeOffset
        var ms = (ticks / TicksPerMillisecond) - UnixEpochMilliseconds;

        // Timestamp
        Span<byte> msBytes = stackalloc byte[8];  // stackalloc into span doesn't require unsafe context
        BinaryPrimitives.WriteInt64BigEndian(msBytes, ms);
        msBytes[2..].CopyTo(Bytes);  // just lower 48 bits are used

        // Randomness
        if (LastMillisecond != ms) {
            LastMillisecond = ms;
            RandomNumberGenerator.Fill(Bytes.AsSpan(6));  // 12-bit rand_a + all of rand_b (extra bits will be overwritten later)
            MonotonicCounter = (uint)(((Bytes[6] & 0x07) << 22) | (Bytes[7] << 14) | ((Bytes[8] & 0x3F) << 8) | Bytes[9]);  // to use as monotonic random for future calls; total of 26 bits but only 25 are used initially with upper 1 bit reserved for rollover guard
        } else {
            RandomNumberGenerator.Fill(Bytes.AsSpan(9));  // rest of rand_b (14 bits "stolen" for monotonic counter) + 1 extra bytes at start (for random increment)
            MonotonicCounter += ((uint)Bytes[9] >> 4) + 1;  // 4 bit random increment will reduce overall counter space by 3 bits on average (to 2^22 combinations)
            Bytes[7] = (byte)(MonotonicCounter >> 14);    // bits 14:21 of monotonics counter
            Bytes[9] = (byte)(MonotonicCounter);          // bits 0:7 of monotonics counter
        }

        //Fixup
        Bytes[6] = (byte)(0x70 | ((MonotonicCounter >> 22) & 0x0F));  // set 4-bit version + bits 22:25 of monotonics counter
        Bytes[8] = (byte)(0x80 | ((MonotonicCounter >> 8) & 0x3F));   // set 2-bit variant + bits 8:13 of monotonics counter
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
        Buffer.BlockCopy(buffer, 0, Bytes, 0, Bytes.Length);
    }

    /// <summary>
    /// Creates a new instance from given GUID bytes.
    /// No check if GUID is version 7 UUID is made.
    /// </summary>
    public Uuid7(Guid guid) {
        Bytes = guid.ToByteArray();
    }


    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    private readonly byte[] Bytes;


    [ThreadStatic]
    private static long LastMillisecond;

    [ThreadStatic]
    private static uint MonotonicCounter;


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
        Buffer.BlockCopy(Bytes, 0, copy, 0, copy.Length);
        return copy;
    }


    #region Id22

    private static readonly BigInteger Id22Modulo = 58;
    private static readonly char[] Id22Alphabet = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
                                                               'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
                                                               'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
                                                               'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                                                               'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
                                                               's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    private static readonly Lazy<Dictionary<char, BigInteger>> Id22AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>();
        for (var i = 0; i < Id22Alphabet.Length; i++) {
            dict.Add(Id22Alphabet[i], i);
        }
        return dict;
    });

    /// <summary>
    /// Returns UUID representation in Id22 format. This is base58 encoder
    /// using the same alphabet as bitcoin does.
    /// </summary>
    public string ToId22String() {
        var number = new BigInteger(Bytes, isUnsigned: true, isBigEndian: true);
        var result = new char[22];  // always the same length
        for (var i = 21; i >= 0; i--) {
            number = BigInteger.DivRem(number, Id22Modulo, out var remainder);
            result[i] = Id22Alphabet[(int)remainder];
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

        var alphabetDict = Id22AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in id22Text) {
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Id22Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 22) { throw new FormatException("Input must be 22 characters."); }

        var buffer = number.ToByteArray(isUnsigned: true, isBigEndian: true);
        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, newBuffer.Length - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
        return new Uuid7(buffer);
    }

    #endregion Id22

    #region Id25

    private static readonly BigInteger Id25Modulo = 35;
    private static readonly char[] Id25Alphabet = new char[] { '0', '1', '2', '3', '4', '5', '6',
                                                               '7', '8', '9', 'a', 'b', 'c', 'd',
                                                               'e', 'f', 'g', 'h', 'i', 'j', 'k',
                                                               'm', 'n', 'o', 'p', 'q', 'r', 's',
                                                               't', 'u', 'v', 'w', 'x', 'y', 'z' };
    private static readonly Lazy<Dictionary<char, BigInteger>> Id25AlphabetDict = new(() => {
        var dict = new Dictionary<char, BigInteger>();
        for (var i = 0; i < Id25Alphabet.Length; i++) {
            dict.Add(Id25Alphabet[i], i);
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
        var number = new BigInteger(Bytes, isUnsigned: true, isBigEndian: true);
        var result = new char[25];  // always the same length
        for (var i = 24; i >= 0; i--) {
            number = BigInteger.DivRem(number, Id25Modulo, out var remainder);
            result[i] = Id25Alphabet[(int)remainder];
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

        var alphabetDict = Id25AlphabetDict.Value;
        var count = 0;
        var number = new BigInteger();
        foreach (var ch in id25Text.ToLowerInvariant()) {  // convert to lowercase first
            if (alphabetDict.TryGetValue(ch, out var offset)) {
                number = BigInteger.Multiply(number, Id25Modulo);
                number = BigInteger.Add(number, offset);
                count++;
            }
        }
        if (count != 25) { throw new FormatException("Input must be 25 characters."); }

        var buffer = number.ToByteArray(isUnsigned: true, isBigEndian: true);
        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, newBuffer.Length - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
        return new Uuid7(buffer);
    }

    #endregion Id25

    #region FromString

    private static readonly BigInteger Base16Modulo = 16;
    private static readonly char[] Base16Alphabet = new char[] { '0', '1', '2', '3', '4', '5', '6', '7',
                                                                 '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
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

        var buffer = number.ToByteArray(isUnsigned: true, isBigEndian: true);
        if (buffer.Length < 16) {
            var newBuffer = new byte[16];
            Buffer.BlockCopy(buffer, 0, newBuffer, newBuffer.Length - buffer.Length, buffer.Length);
            buffer = newBuffer;
        }
        return new Uuid7(buffer);
    }

    #endregion FromString

    #region Overrides

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) {
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
        return $"{Bytes[0]:x2}{Bytes[1]:x2}{Bytes[2]:x2}{Bytes[3]:x2}-{Bytes[4]:x2}{Bytes[5]:x2}-{Bytes[6]:x2}{Bytes[7]:x2}-{Bytes[8]:x2}{Bytes[9]:x2}-{Bytes[10]:x2}{Bytes[11]:x2}{Bytes[12]:x2}{Bytes[13]:x2}{Bytes[14]:x2}{Bytes[15]:x2}";
    }

    #endregion Overrides

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
    /// Please note this is not a valid UUID7 as it lacks version bits.
    /// </summary>
    public static readonly Uuid7 Empty = new(new byte[16]);

    /// <summary>
    /// A read-only instance of the Guid structure whose value is all 1's.
    /// Please note this is not a valid UUID7 as it lacks version bits.
    /// </summary>
    public static readonly Uuid7 Max = new(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 });


    /// <summary>
    /// Returns new UUID version 7.
    /// </summary>
    public static Uuid7 NewUuid7() {
        return new Uuid7();
    }


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

    #endregion Static


    #region Helpers

    private const long UnixEpochMilliseconds = 62_135_596_800_000;
    private const long TicksPerMillisecond = 10_000;

    private static int CompareArrays(byte[] buffer1, byte[] buffer2) {
        Debug.Assert(buffer1.Length == 16);
        Debug.Assert(buffer2.Length == 16);
        var comparer = Comparer<byte>.Default;
        for (int i = 0; i < buffer1.Length; i++) {
            if (comparer.Compare(buffer1[i], buffer2[i]) < 0) { return -1; }
            if (comparer.Compare(buffer1[i], buffer2[i]) > 0) { return +1; }
        }
        return 0;  // they're equal
    }

    #endregion Helpers

}
