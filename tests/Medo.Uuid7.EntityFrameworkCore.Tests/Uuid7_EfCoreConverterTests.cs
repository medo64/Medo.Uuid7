using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[TestClass]
public class Uuid7_EfCoreConverterTests {
    [TestMethod]
    public void Uuid7_Uuid7ToGuidConverter() {
        using var db = CreateDatabase();
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
        using var db = CreateDatabase();
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
        using var db = CreateDatabase();
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
        using var db = CreateDatabase();
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
        using var db = CreateDatabase();
        User user = CreateUser();
        db.UuidSevens.Add(user);
        db.SaveChanges();
        User dbUser = db.UuidSevens.First();
        Assert.AreEqual(dbUser.AsIdTwentyTwo, user.AsIdTwentyTwo);
        db.Database.CloseConnection();
        db.Database.EnsureDeleted();
    }

    #region Helpers

    private static Database CreateDatabase() {
        var database = new Database();
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

    #endregion
}
