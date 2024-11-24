namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to an ID22 string in the store.
/// </summary>
public class Uuid7ToId22Converter : ValueConverter<Uuid7, string> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToId22Converter()
        : base(
            convertToProviderExpression: x => x.ToId22String(),
            convertFromProviderExpression: x => Uuid7.FromId22String(x)) {
    }
}
