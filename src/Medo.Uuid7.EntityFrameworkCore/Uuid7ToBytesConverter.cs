namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to a byte array in the store.
/// </summary>
public class Uuid7ToBytesConverter  : ValueConverter<Uuid7, byte[]> {
    private static readonly ConverterMappingHints defaultHints = new ConverterMappingHints(size: 16);

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToBytesConverter()
        : base(
            convertToProviderExpression: x => x.ToByteArray(),
            convertFromProviderExpression: x => new Uuid7(x),
            mappingHints: defaultHints) {
    }
}
