/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Runtime.CompilerServices;

#if NET7_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

public readonly partial struct Uuid7 : IEquatable<Guid>, IEquatable<Uuid7> {

    #region IEquatable<Guid>

    /// <summary>
    /// Returns a value that indicates whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="other">An object to compare to this instance.</param>
    public bool Equals(Guid other)
    {
#if NET7_0_OR_GREATER
        if (Vector128.IsHardwareAccelerated) {
            var vector1 = (Bytes != null)
                ? Unsafe.ReadUnaligned<Vector128<byte>>(ref Bytes[0])
                : Vector128<byte>.Zero;
            var vector2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref other.ToByteArray()[0]);
            return vector1 == vector2;
        }
#endif
        return CompareArrays(Bytes, other.ToByteArray()) == 0;
    }

    #endregion IEquatable<Guid>


    #region IEquatable<Uuid>

    /// <summary>
    /// Returns a value that indicates whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="other">An object to compare to this instance.</param>
    public bool Equals(Uuid7 other)
    {
#if NET7_0_OR_GREATER
        if (Vector128.IsHardwareAccelerated) {
            var vector1 = (Bytes != null)
                ? Unsafe.ReadUnaligned<Vector128<byte>>(ref Bytes[0])
                : Vector128<byte>.Zero;
            if (other.Bytes == null) {
                return vector1 == Vector128<byte>.Zero;
            } else {
                var vector2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref other.Bytes[0]);
                return vector1 == vector2;
            }
        }
#endif
        return CompareArrays(Bytes, other.Bytes) == 0;
    }

    #endregion IEquatable<Uuid>

}
