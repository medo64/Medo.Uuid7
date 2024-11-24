namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to a string in the store.
/// </summary>
public class Uuid7ToStringConverter : ValueConverter<Uuid7, string> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToStringConverter()
        : base(
            convertToProviderExpression: x => x.ToString(),
            convertFromProviderExpression: x => Uuid7.FromString(x)) {
    }
}
