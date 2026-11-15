/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

#if NET6_0_OR_GREATER

using System;
using System.Diagnostics.CodeAnalysis;

public readonly partial struct Uuid7 : ISpanFormattable {

#if NET7_0_OR_GREATER
    /// <summary>
    /// Tries to format the current instance into the provided character span.
    /// The following format specifiers are supported:
    /// - D: Default - 32 digits separated by hyphens, e.g. 00000000-0000-0000-0000-000000000000
    /// - N: No hyphen - 32 digits, e.g. 00000000000000000000000000000000
    /// - B: Braces - 	32 digits separated by hyphens, enclosed in braces, e.g. {00000000-0000-0000-0000-000000000000}
    /// - P: Parentheses - 32 digits separated by hyphens, enclosed in parentheses, e.g. (00000000-0000-0000-0000-000000000000)
    /// - X: Hexadecimal - Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces, e.g. {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} - for the love of all that is holy do not use this format
    /// - C: Crockford - 26 characters from a lexicographically sortable, case-insensitive 32 character alphabet, e.g. 000G40SM6NV7FE5SZBXZSZFYZW.
    /// - c: Crockford - 26 characters from a lower-case lexicographically sortable, case-insensitive 32 character alphabet, e.g. 000g40r40m30e209185gr38e1w.
    /// - Č: Crockford - 26 characters from a lexicographically sortable, case-insensitive 32 character alphabet with checksum, e.g. 000G40R40M30E209185GR38E1X.
    /// - č: Crockford - 26 characters from a lower-case lexicographically sortable, case-insensitive 32 character alphabet with checksum, e.g. 000g40r40m30e209185gr38e1x.
    /// - 6: Id26 - 26 characters from a lexicographicallz sortable, case-insensitive 32 character alphabet, e.g. 000jnpfwbu1xvb3pg9cd522zv.
    /// - 5: Id25 - 25 characters from a case-insensitive 35 character alphabet, e.g. 000jnpfwbu1xvb3pg9cd522zv.
    /// - 2: Id22 - 22 characters from a 58 character alphabet, e.g. 112drYSDr45nCJ6chixdxJ
    /// </summary>
    /// <param name="destination">The span in which to write.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters written into the span.</param>
    /// <param name="format">A read-only span containing the character representing one of the following specifiers that indicates the exact format to use when interpreting input: "N", "D", "B", "P", or "X".</param>
    /// <param name="provider">Not used.</param>
    /// <exception cref="NotImplementedException"></exception>
    public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.GuidFormat)] ReadOnlySpan<char> format, IFormatProvider? provider) {  // formatProvider is ignored
        if (format.Length > 1) { charsWritten = 0; return false; }

        var formatChar = (format.Length == 1) ? format[0] : 'D';
        return formatChar switch {  // treat uppercase and lowercase the same (compatibility with Guid ToFormat)
            'D' or 'd' => TryWriteAsDefaultString(destination, Bytes, out charsWritten),
            'N' or 'n' => TryWriteAsNoHypensString(destination, Bytes, out charsWritten),
            'B' or 'b' => TryWriteAsBracesString(destination, Bytes, out charsWritten),
            'P' or 'p' => TryWriteAsParenthesesString(destination, Bytes, out charsWritten),
            'X' or 'x' => TryWriteAsHexadecimalString(destination, Bytes, out charsWritten),
            'C' => TryWriteAsCrockford(uppercase: true, includeChecksum: false, destination, Bytes, out charsWritten),
            'c' => TryWriteAsCrockford(uppercase: false, includeChecksum: false, destination, Bytes, out charsWritten),
            'Č' => TryWriteAsCrockford(uppercase: true, includeChecksum: true, destination, Bytes, out charsWritten),
            'č' => TryWriteAsCrockford(uppercase: false, includeChecksum: true, destination, Bytes, out charsWritten),
            '6' => TryWriteAsId26(destination, Bytes, out charsWritten),
            '5' => TryWriteAsId25(destination, Bytes, out charsWritten),
            '2' => TryWriteAsId22(destination, Bytes, out charsWritten),
            _ => throw new FormatException("Invalid UUID format."),
        };
    }

#elif NET6_0_OR_GREATER

    /// <summary>
    /// Tries to format the current instance into the provided character span.
    /// The following format specifiers are supported:
    /// - D: Default - 32 digits separated by hyphens, e.g. 00000000-0000-0000-0000-000000000000
    /// - N: No hyphen - 32 digits, e.g. 00000000000000000000000000000000
    /// - B: Braces - 	32 digits separated by hyphens, enclosed in braces, e.g. {00000000-0000-0000-0000-000000000000}
    /// - P: Parentheses - 32 digits separated by hyphens, enclosed in parentheses, e.g. (00000000-0000-0000-0000-000000000000)
    /// - X: Hexadecimal - Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces, e.g. {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} - for the love of all that is holy do not use this format
    /// - C: Crockford - 26 characters from a lexicographically sortable, case-insensitive 32 character alphabet, e.g. 000G40SM6NV7FE5SZBXZSZFYZW.
    /// - c: Crockford - 26 characters from a lower-case lexicographically sortable, case-insensitive 32 character alphabet, e.g. 000g40r40m30e209185gr38e1w.
    /// - Č: Crockford - 26 characters from a lexicographically sortable, case-insensitive 32 character alphabet with checksum, e.g. 000G40R40M30E209185GR38E1X.
    /// - č: Crockford - 26 characters from a lower-case lexicographically sortable, case-insensitive 32 character alphabet with checksum, e.g. 000g40r40m30e209185gr38e1x.
    /// - 6: Id26 - 26 characters from a lexicographicallz sortable, case-insensitive 32 character alphabet, e.g. 000jnpfwbu1xvb3pg9cd522zv.
    /// - 5: Id25 - 25 characters from a case-insensitive 35 character alphabet, e.g. 000jnpfwbu1xvb3pg9cd522zv.
    /// - 2: Id22 - 22 characters from a 58 character alphabet, e.g. 112drYSDr45nCJ6chixdxJ
    /// </summary>
    /// <param name="destination">The span in which to write.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters written into the span.</param>
    /// <param name="format">A read-only span containing the character representing one of the following specifiers that indicates the exact format to use when interpreting input: "N", "D", "B", "P", or "X".</param>
    /// <param name="provider">Not used.</param>
    /// <exception cref="NotImplementedException"></exception>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {  // formatProvider is ignored
        if (format.Length > 1) { charsWritten = 0; return false; }

        var formatChar = (format.Length == 1) ? format[0] : 'D';
        return formatChar switch {  // treat uppercase and lowercase the same (compatibility with Guid ToFormat)
            'D' or 'd' => TryWriteAsDefaultString(destination, Bytes, out charsWritten),
            'N' or 'n' => TryWriteAsNoHypensString(destination, Bytes, out charsWritten),
            'B' or 'b' => TryWriteAsBracesString(destination, Bytes, out charsWritten),
            'P' or 'p' => TryWriteAsParenthesesString(destination, Bytes, out charsWritten),
            'X' or 'x' => TryWriteAsHexadecimalString(destination, Bytes, out charsWritten),
            'C' => TryWriteAsCrockford(uppercase: true, includeChecksum: false, destination, Bytes, out charsWritten),
            'c' => TryWriteAsCrockford(uppercase: false, includeChecksum: false, destination, Bytes, out charsWritten),
            'Č' => TryWriteAsCrockford(uppercase: true, includeChecksum: true, destination, Bytes, out charsWritten),
            'č' => TryWriteAsCrockford(uppercase: false, includeChecksum: true, destination, Bytes, out charsWritten),
            '6' => TryWriteAsId26(destination, Bytes, out charsWritten),
            '5' => TryWriteAsId25(destination, Bytes, out charsWritten),
            '2' => TryWriteAsId22(destination, Bytes, out charsWritten),
            _ => throw new FormatException("Invalid UUID format."),
        };
    }

#endif

}

#endif
