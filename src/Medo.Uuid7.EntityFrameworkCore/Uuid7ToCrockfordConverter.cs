namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to a Crockford base 32 string in the store.
/// </summary>
public class Uuid7ToCrockfordConverter : ValueConverter<Uuid7, string> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToCrockfordConverter()
        : base(
            convertToProviderExpression: x => x.ToCrockfordString(),
            convertFromProviderExpression: x => Uuid7.FromCrockfordString(x)) {
    }
}
