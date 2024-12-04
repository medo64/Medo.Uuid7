namespace Medo;
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to a Guid in the store.
/// </summary>
public class Uuid7ToGuidConverter : ValueConverter<Uuid7, Guid> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToGuidConverter()
        : base(
            convertToProviderExpression: x => x.ToGuid(),
            convertFromProviderExpression: x => new Uuid7(x)) {
    }
}
