using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

namespace Tests;

[TestClass]
public class Uuid7_EfCoreConverterTests {

    [TestMethod]
    [DataRow(typeof(Uuid7ToBytesConverter))]
    [DataRow(typeof(Uuid7ToGuidConverter))]
    [DataRow(typeof(Uuid7ToId22Converter))]
    [DataRow(typeof(Uuid7ToId25Converter))]
    [DataRow(typeof(Uuid7ToStringConverter))]
    public void Uuid7_AllConverters_Global(Type converterType) {
        using var db = CreateDatabaseForConverterGlobal(converterType);
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.Id, user.Id);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToGuidConverter() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.Id, user.Id);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToBytesConverter() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.AsBytes, user.AsBytes);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToStringConverter() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.AsString, user.AsString);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToId25Converter() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.AsIdTwentyFive, user.AsIdTwentyFive);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToId22Converter() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.AsIdTwentyTwo, user.AsIdTwentyTwo);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }


    private static readonly byte[] DummyUuidBytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };

    [TestMethod]
    public void Uuid7_Uuid7ToGuidConverter_Fixed() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser(DummyUuidBytes);
        db.UuidSevens.Add(user);
        db.SaveChanges();

        var entry = db.Database.SqlQueryRaw<string>("SELECT [Id] FROM UuidSevens").ToList()[0];
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual("01020304-0506-0708-090A-0B0C0D0E0F10", entry);
        } else {
            Assert.AreEqual("04030201-0605-0807-090A-0B0C0D0E0F10", entry);
        }

        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToBytesConverter_Fixed() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser(DummyUuidBytes);
        db.UuidSevens.Add(user);
        db.SaveChanges();

        var entry = db.Database.SqlQueryRaw<byte[]>("SELECT [AsBytes] FROM UuidSevens").ToList()[0];
        Assert.AreEqual(Convert.ToBase64String(DummyUuidBytes), Convert.ToBase64String(entry));

        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToStringConverter_Fixed() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser(DummyUuidBytes);
        db.UuidSevens.Add(user);
        db.SaveChanges();

        var entry = db.Database.SqlQueryRaw<string>("SELECT [AsString] FROM UuidSevens").ToList()[0];
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", entry);

        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToId25Converter_Fixed() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser(DummyUuidBytes);
        db.UuidSevens.Add(user);
        db.SaveChanges();

        var entry = db.Database.SqlQueryRaw<string>("SELECT [AsIdTwentyFive] FROM UuidSevens").ToList();
        Assert.AreEqual("043q0v97ii6dr81xzei508wyw", entry[0]);

        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Uuid7_Uuid7ToId22Converter_Fixed() {
        using var db = CreateDatabaseForConverterByEntity();
        User user = CreateUser(DummyUuidBytes);
        db.UuidSevens.Add(user);
        db.SaveChanges();

        var entry = db.Database.SqlQueryRaw<string>("SELECT [AsIdTwentyTwo] FROM UuidSevens").ToList();
        Assert.AreEqual("18DfbjXLth7APvt3qQPgtf", entry[0]);

        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }


    #region Helpers

    private static DatabaseForConverterByEntity CreateDatabaseForConverterByEntity() {
        var database = new DatabaseForConverterByEntity();
        database.Database.OpenConnection();
        database.Database.EnsureCreated();
        return database;
    }

    private static DatabaseForConverterGlobal CreateDatabaseForConverterGlobal(Type converterType) {
        var database = new DatabaseForConverterGlobal(converterType);
        database.Database.OpenConnection();
        database.Database.EnsureCreated();
        return database;
    }

    private static User CreateUser() {
        return new() {
            Id = Uuid7.NewUuid7(),
            AsBytes = Uuid7.NewUuid7(),
            AsIdTwentyFive = Uuid7.NewUuid7(),
            AsIdTwentyTwo = Uuid7.NewUuid7(),
            AsString = Uuid7.NewUuid7()
        };
    }

    private static User CreateUser(byte[] uuidBytes) {
        return new() {
            Id = new Uuid7(uuidBytes),
            AsBytes = new Uuid7(uuidBytes),
            AsIdTwentyFive = new Uuid7(uuidBytes),
            AsIdTwentyTwo = new Uuid7(uuidBytes),
            AsString = new Uuid7(uuidBytes)
        };
    }

    #endregion
}
