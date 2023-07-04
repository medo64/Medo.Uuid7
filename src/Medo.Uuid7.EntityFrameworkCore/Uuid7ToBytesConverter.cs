namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class Uuid7ToBytesConverter  : ValueConverter<Uuid7, byte[]>
{
    private static readonly ConverterMappingHints defaultHints = new ConverterMappingHints(size: 16);

    public Uuid7ToBytesConverter()
        : base(
            convertToProviderExpression: x => x.ToByteArray(),
            convertFromProviderExpression: x => new Uuid7(x),
            mappingHints: defaultHints)
    {
    }
}
