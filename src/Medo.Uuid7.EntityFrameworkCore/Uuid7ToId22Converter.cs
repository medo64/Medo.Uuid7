using Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Tests;

public class Uuid7ToId22Converter  : ValueConverter<Uuid7, string>
{
    public Uuid7ToId22Converter()
        : base(
            convertToProviderExpression: x => x.ToId22String(),
            convertFromProviderExpression: x => Uuid7.FromId22String(x))
    {
    }
}
