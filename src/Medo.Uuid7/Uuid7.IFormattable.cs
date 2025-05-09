/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

#if NET7_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif


public readonly partial struct Uuid7 : IFormattable {

    #region IFormattable

    /// <summary>
    /// Formats the value of the current instance using the specified format.
    /// The following format specifiers are supported:
    /// - D: Default - 32 digits separated by hyphens, e.g. 00000000-0000-0000-0000-000000000000
    /// - N: No hyphen - 32 digits, e.g. 00000000000000000000000000000000
    /// - B: Braces - 	32 digits separated by hyphens, enclosed in braces, e.g. {00000000-0000-0000-0000-000000000000}
    /// - P: Parentheses - 32 digits separated by hyphens, enclosed in parentheses, e.g. (00000000-0000-0000-0000-000000000000)
    /// - X: Hexadecimal - Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces, e.g. {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} - for the love of all that is holy do not use this format
    /// - 5: Id25 - 25 characters from a case-insensitive 35 character alphabet, e.g. 0000000000000000000000000
    /// - 2: Id22 - 22 characters from a 58 character alphabet, e.g. 0000000000000000000000
    /// </summary>
    /// <param name="format">The format to use.</param>
#if NET7_0_OR_GREATER
    public string ToString([StringSyntax(StringSyntaxAttribute.GuidFormat)] string? format) {
#else
    public string ToString(string? format) {
#endif
        return ToString(format, formatProvider: null);
    }

    /// <summary>
    /// Formats the value of the current instance using the specified format.
    /// The following format specifiers are supported:
    /// - D: Default - 32 digits separated by hyphens, e.g. 00000000-0000-0000-0000-000000000000
    /// - N: No hyphen - 32 digits, e.g. 00000000000000000000000000000000
    /// - B: Braces - 	32 digits separated by hyphens, enclosed in braces, e.g. {00000000-0000-0000-0000-000000000000}
    /// - P: Parentheses - 32 digits separated by hyphens, enclosed in parentheses, e.g. (00000000-0000-0000-0000-000000000000)
    /// - X: Hexadecimal - Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces, e.g. {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} - for the love of all that is holy do not use this format
    /// - 5: Id25 - 25 characters from a case-insensitive 35 character alphabet, e.g. 0000000000000000000000000
    /// - 2: Id22 - 22 characters from a 58 character alphabet, e.g. 0000000000000000000000
    /// </summary>
    /// <param name="format">The format to use.</param>
    /// <param name="formatProvider">Not used.</param>
#if NET7_0_OR_GREATER
    public string ToString([StringSyntax(StringSyntaxAttribute.GuidFormat)] string? format, IFormatProvider? formatProvider) {
#else
    public string ToString(string? format, IFormatProvider? formatProvider) {  // formatProvider is ignored
#endif
        switch (format) {  // treat uppercase and lowercase the same (compatibility with Guid ToFormat)
            case null:
            case "":
            case "D":
            case "d": {
#if NET6_0_OR_GREATER
                    return string.Create(36, Bytes, static (destination, bytes)
                        => TryWriteAsDefaultString(destination, bytes, out _));
#else
                    var destination = new char[36];
                    TryWriteAsDefaultString(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "N":
            case "n": {
#if NET6_0_OR_GREATER
                    return string.Create(32, Bytes, static (destination, bytes)
                        => TryWriteAsNoHypensString(destination, bytes, out _));
#else
                    var destination = new char[32];
                    TryWriteAsNoHypensString(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "B":
            case "b": {
#if NET6_0_OR_GREATER
                    return string.Create(38, Bytes, static (destination, bytes)
                        => TryWriteAsBracesString(destination, bytes, out _));
#else
                    var destination = new char[38];
                    TryWriteAsBracesString(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "P":
            case "p": {
#if NET6_0_OR_GREATER
                    return string.Create(38, Bytes, static (destination, bytes)
                        => TryWriteAsParenthesesString(destination, bytes, out _));
#else
                    var destination = new char[38];
                    TryWriteAsParenthesesString(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "X":
            case "x": {
#if NET6_0_OR_GREATER
                    return string.Create(68, Bytes, static (destination, bytes)
                        => TryWriteAsHexadecimalString(destination, bytes, out _));
#else
                    var destination = new char[68];
                    TryWriteAsHexadecimalString(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "6": {  // non-standard (Id26C)
#if NET6_0_OR_GREATER
                    return string.Create(26, Bytes, static (destination, bytes)
                        => TryWriteAsId26(destination, bytes, out _));
#else
                    var destination = new char[26];
                    TryWriteAsId26(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "5": {  // non-standard (Id25)
#if NET6_0_OR_GREATER
                    return string.Create(25, Bytes, static (destination, bytes)
                        => TryWriteAsId25(destination, bytes, out _));
#else
                    var destination = new char[25];
                    TryWriteAsId25(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            case "2": {  // non-standard (Id22)
#if NET6_0_OR_GREATER
                    return string.Create(22, Bytes, static (destination, bytes)
                        => TryWriteAsId22(destination, bytes, out _));
#else
                    var destination = new char[22];
                    TryWriteAsId22(destination, Bytes, out _);
                    return new string(destination);
#endif
                }

            default: throw new FormatException("Invalid UUID format.");
        }
    }

    #endregion IFormattable

}
