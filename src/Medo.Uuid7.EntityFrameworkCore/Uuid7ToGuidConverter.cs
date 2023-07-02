using System;
using Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Tests;

public class Uuid7ToGuidConverter  : ValueConverter<Uuid7, Guid>
{
    public Uuid7ToGuidConverter()
        : base(
            convertToProviderExpression: x => x.ToGuid(),
            convertFromProviderExpression: x => new Uuid7(x))
    {
    }
}
