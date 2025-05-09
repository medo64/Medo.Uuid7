/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

#if NET6_0_OR_GREATER

using System;
using System.Diagnostics.CodeAnalysis;

public readonly partial struct Uuid7 : ISpanFormattable {

#if NET7_0_OR_GREATER
    /// <summary>
    /// Tries to format the current instance into the provided character span.
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
            '6' => TryWriteAsId26(destination, Bytes, out charsWritten),
            '5' => TryWriteAsId25(destination, Bytes, out charsWritten),
            '2' => TryWriteAsId22(destination, Bytes, out charsWritten),
            _ => throw new FormatException("Invalid UUID format."),
        };
    }

#elif NET6_0_OR_GREATER

    /// <summary>
    /// Tries to format the current instance into the provided character span.
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
            '6' => TryWriteAsId26(destination, Bytes, out charsWritten),
            '5' => TryWriteAsId25(destination, Bytes, out charsWritten),
            '2' => TryWriteAsId22(destination, Bytes, out charsWritten),
            _ => throw new FormatException("Invalid UUID format."),
        };
    }

#endif

}

#endif
