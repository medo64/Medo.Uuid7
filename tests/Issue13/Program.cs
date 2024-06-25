using Medo;

{  // preserve raw order
    var uuid = Uuid7.NewUuid7();
    Console.WriteLine($"{uuid}");

    var guid = (Guid)uuid;
    Console.WriteLine($"{guid}");
}

Console.WriteLine();

{  // preserve logical order
    var uuid = Uuid7.NewUuid7();
    Console.WriteLine($"{uuid}");

    var guid = uuid.ToGuidMsSql();
    Console.WriteLine($"{guid}");
}
