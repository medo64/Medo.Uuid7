/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Diagnostics.CodeAnalysis;

#if NET7_0_OR_GREATER
public readonly partial struct Uuid7 : ISpanParsable<Uuid7> {
#else
public readonly partial struct Uuid7 {
#endif

    #region ISpanParsable<Uuid7>

    /// <summary>
    /// Returns UUID parsed from a given input.
    /// The following formats are supported: D, N, B, and P.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 Parse(string s) {
        return Parse(s, provider: null);
    }

    /// <summary>
    /// Returns true if UUID was successfully parsed.
    /// The following formats are supported: D, N, B, and P.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing or an undefined value on failure.</param>
#if NET6_0_OR_GREATER
    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Uuid7 result) {
#else
    public static bool TryParse(string? s, out Uuid7 result) {
#endif
        return TryParse(s, provider: null, out result);
    }

#if NET7_0_OR_GREATER

    /// <summary>
    /// Returns UUID parsed from a given input.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
        if (TryParse(s, provider, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }

    /// <summary>
    /// Returns true if UUID was successfully parsed.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing or an undefined value on failure.</param>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Uuid7 result) {
        return TryParseAsString(s, out result);
    }
#endif

    /// <summary>
    /// Returns UUID parsed from a given input.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <exception cref="ArgumentNullException">Input cannot be null.</exception>
    /// <exception cref="FormatException">Unrecognized UUID format.</exception>
    public static Uuid7 Parse(string s, IFormatProvider? provider) {
        if (s is null) { throw new ArgumentNullException(nameof(s), "Input cannot be null."); }
        if (TryParse(s, provider, out var result)) {
            return result;
        } else {
            throw new FormatException("Unrecognized UUID format.");
        }
    }

    /// <summary>
    /// Returns true if UUID was successfully parsed.
    /// </summary>
    /// <param name="s">Input.</param>
    /// <param name="provider">Not used.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing or an undefined value on failure.</param>
#pragma warning disable IDE0060  // Remove unused parameter
#if NET6_0_OR_GREATER
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Uuid7 result) {
        if (s == null) { result = Empty; return false; }
        return TryParseAsString(s.AsSpan(), out result);
    }
#else
    public static bool TryParse(string? s, IFormatProvider? provider, out Uuid7 result) {
        if (s == null) { result = Empty; return false; }
        return TryParseAsString(s.ToCharArray(), out result);
    }
#endif
#pragma warning restore IDE0060  // Remove unused parameter

    #endregion ISpanParsable<Uuid7>

}
