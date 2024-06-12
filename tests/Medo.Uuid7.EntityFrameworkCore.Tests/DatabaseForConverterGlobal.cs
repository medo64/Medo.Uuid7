using System;
using Microsoft.EntityFrameworkCore;
using Medo;

namespace Tests;

public class DatabaseForConverterGlobal : DbContext {
    private readonly Type ConverterType;

    public DatabaseForConverterGlobal(Type converterType) {
        ConverterType = converterType;
    }

    public DbSet<User> UuidSevens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite($"DataSource={Guid.NewGuid()}.db");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
        configurationBuilder.Properties<Uuid7>().HaveConversion(ConverterType);
    }
}
