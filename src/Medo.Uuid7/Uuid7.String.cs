/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;

public readonly partial struct Uuid7 {

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to hexadecimal alphabet are ignored.
    /// Input must contain exactly 32 hexadecimal characters.
    /// The following formats are supported: D, N, B, and P.
    /// </summary>
    /// <param name="text">UUID text.</param>
    /// <exception cref="ArgumentNullException">Text cannot be null.</exception>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 FromString(string text) {
        return Parse(text, provider: null);
    }


    /// <summary>
    /// Returns UUID representation in Crockford base 32 format.
    /// Crockford base 32 uses lexicographical sortable Base-32 dictionary.
    /// </summary>
    public string ToCrockfordString() {
 #if NET6_0_OR_GREATER
        return string.Create(26, Bytes, static (destination, bytes) => TryWriteAsCrockford(uppercase: true, includeChecksum: false, destination, bytes, out _));
#else
        var destination = new char[26];
        TryWriteAsCrockford(uppercase: true, includeChecksum: false, destination, Bytes, out _);
        return new string(destination);
#endif
    }

    /// <summary>
    /// Returns UUID representation in Crockford base 32 format.
    /// Crockford base 32 uses lexicographical sortable Base-32 dictionary with an optional checksum.
    /// </summary>
    /// <param name="uppercase"></param>
    /// <param name="includeChecksum">If true, the last 2 bit will contain modulo 3 checksum.</param>
    public string ToCrockfordString(bool uppercase, bool includeChecksum) {
 #if NET6_0_OR_GREATER
        return string.Create(26, Bytes, (destination, bytes) => TryWriteAsCrockford(uppercase, includeChecksum, destination, bytes, out _));
#else
        var destination = new char[26];
        TryWriteAsCrockford(uppercase, includeChecksum, destination, Bytes, out _);
        return new string(destination);
#endif
    }

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to Crockford alphabet are ignored.
    /// </summary>
    /// <param name="crockfordText">Crockford base 32 text.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
#if NET6_0_OR_GREATER
    public static Uuid7 FromCrockfordString(ReadOnlySpan<char> crockfordText) {
        if (TryParseAsCrockford(crockfordText, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#else
    public static Uuid7 FromCrockfordString(string crockfordText) {
        if (crockfordText == null) { throw new ArgumentNullException(nameof(crockfordText), "Text cannot be null."); }
        if (TryParseAsCrockford(crockfordText.ToCharArray(), out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#endif


    /// <summary>
    /// Returns UUID representation in Id26 format.
    /// Id26 uses lexicographical sortable Base-32 dictionary with 2-bit modulo 2 Fletcher checksum.
    /// </summary>
    public string ToId26String() {
        return ToString(format: "6", formatProvider: null);
    }

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to Id26 alphabet are ignored.
    /// </summary>
    /// <param name="id26Text">Id26 text.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
#if NET6_0_OR_GREATER
    public static Uuid7 FromId26String(ReadOnlySpan<char> id26Text) {
        if (TryParseAsId26(id26Text, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#else
    public static Uuid7 FromId26String(string id26Text) {
        if (id26Text == null) { throw new ArgumentNullException(nameof(id26Text), "Text cannot be null."); }
        if (TryParseAsId26(id26Text.ToCharArray(), out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#endif


    /// <summary>
    /// Returns UUID representation in Id25 format.
    /// Please note that while conversion is the same as one in
    /// https://github.com/stevesimmons/uuid7-csharp/, UUIDs are not fully
    /// compatible and thus not necessarily interchangeable.
    /// </summary>
    public string ToId25String() {
        return ToString(format: "5", formatProvider: null);
    }

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to Id25 alphabet are ignored.
    /// Input must contain exactly 25 characters.
    /// </summary>
    /// <param name="id25Text">Id25 text.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
#if NET6_0_OR_GREATER
    public static Uuid7 FromId25String(ReadOnlySpan<char> id25Text) {
        if (TryParseAsId25(id25Text, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#else
    public static Uuid7 FromId25String(string id25Text) {
        if (id25Text == null) { throw new ArgumentNullException(nameof(id25Text), "Text cannot be null."); }
        if (TryParseAsId25(id25Text.ToCharArray(), out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#endif


    /// <summary>
    /// Returns UUID representation in Id22 format. This is base58 encoder
    /// using the same alphabet as bitcoin does.
    /// </summary>
    public string ToId22String() {
        return ToString(format: "2", formatProvider: null);
    }

    /// <summary>
    /// Returns UUID from given text representation.
    /// All characters not belonging to Id22 alphabet are ignored.
    /// Input must contain exactly 22 characters.
    /// </summary>
    /// <param name="id22Text">Id22 text.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
#if NET6_0_OR_GREATER
    public static Uuid7 FromId22String(ReadOnlySpan<char> id22Text) {
        if (TryParseAsId22(id22Text, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#else
    public static Uuid7 FromId22String(string id22Text) {
        if (id22Text == null) { throw new ArgumentNullException(nameof(id22Text), "Text cannot be null."); }
        if (TryParseAsId22(id22Text.ToCharArray(), out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }
#endif

}
