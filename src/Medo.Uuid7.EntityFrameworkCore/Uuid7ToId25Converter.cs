namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to a ID25 string in the store.
/// </summary>
public class Uuid7ToId25Converter : ValueConverter<Uuid7, string> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToId25Converter()
        : base(
            convertToProviderExpression: x => x.ToId25String(),
            convertFromProviderExpression: x => Uuid7.FromId25String(x)) {
    }
}
