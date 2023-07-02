using Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Tests;

public class Uuid7ToId25Converter  : ValueConverter<Uuid7, string>
{
    public Uuid7ToId25Converter()
        : base(
            convertToProviderExpression: x => x.ToId25String(),
            convertFromProviderExpression: x => Uuid7.FromId25String(x))
    {
    }
}
