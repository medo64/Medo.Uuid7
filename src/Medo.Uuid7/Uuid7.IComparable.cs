/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;

public readonly partial struct Uuid7 : IComparable<Guid>, IComparable<Uuid7> {

    #region IComparable<Guid>

    /// <summary>
    /// Compares this instance to a specified Guid object and returns an indication of their relative values.
    /// A negative integer if this instance is less than value; a positive integer if this instance is greater than value; or zero if this instance is equal to value.
    /// </summary>
    /// <param name="other">An object to compare to this instance.</param>
    public int CompareTo(Guid other) {
        var guidBytes = other.ToByteArray();
        if (BitConverter.IsLittleEndian) { ReverseGuidEndianess(ref guidBytes); }
        return CompareArrays(Bytes, guidBytes);
    }

    #endregion IComparable<Guid>


    #region IComparable<Uuid>

    /// <summary>
    /// Compares this instance to a specified Guid object and returns an indication of their relative values.
    /// A negative integer if this instance is less than value; a positive integer if this instance is greater than value; or zero if this instance is equal to value.
    /// </summary>
    /// <param name="other">An object to compare to this instance.</param>
    public int CompareTo(Uuid7 other) {
        return CompareArrays(Bytes, other.Bytes);
    }

    #endregion IComparable<Uuid>

}
