namespace Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// Defines conversions from an object of one Uuid7 in a model to a ID26 string in the store.
/// </summary>
public class Uuid7ToId26Converter : ValueConverter<Uuid7, string> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Uuid7ToId26Converter()
        : base(
            convertToProviderExpression: x => x.ToId26String(),
            convertFromProviderExpression: x => Uuid7.FromId26String(x)) {
    }
}
