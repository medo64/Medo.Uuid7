using System;
using Microsoft.EntityFrameworkCore;
using Medo;

namespace Tests;

public class Database : DbContext {
    public DbSet<User> UuidSevens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite($"DataSource={Guid.NewGuid()}.db");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>().Property(x => x.Id).HasConversion<Uuid7ToGuidConverter>();
        modelBuilder.Entity<User>().Property(x => x.AsBytes).HasConversion<Uuid7ToBytesConverter>();
        modelBuilder.Entity<User>().Property(x => x.AsIdTwentyFive).HasConversion<Uuid7ToId25Converter>();
        modelBuilder.Entity<User>().Property(x => x.AsIdTwentyTwo).HasConversion<Uuid7ToId22Converter>();
        modelBuilder.Entity<User>().Property(x => x.AsString).HasConversion<Uuid7ToStringConverter>();
        base.OnModelCreating(modelBuilder);
    }
}
