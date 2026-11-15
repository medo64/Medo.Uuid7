namespace Tests;
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

[TestClass]
public class Uuid7_Id22_Tests {

    private const bool IsModernDotNet =
#if NET6_0_OR_GREATER
        true;
#else
        false;
#endif


    [TestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "1111111111111111111111")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "1111111111111111111112")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "1111111111111111111113")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "111111111112WK48GNSUQf")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "112drYSDr45nCJ6chixdxJ")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "18AQGAut7N92awznwCnjuR")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "1FKoXLpmDjH4AtzasQaUoq")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "94PbGyNhxXNzM9jv37aXvF")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "YcVfxkQb6JRzqk5kF2tNLv")]
    public void Uuid7_FormatParse_Id22(byte[] bytes, string id22Text) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(id22Text, uuid.ToId22String());
        Assert.AreEqual(uuid, Uuid7.FromId22String(id22Text));
        Assert.IsTrue(Uuid7.TryParseId22(id22Text, out var _));
    }

    [TestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "112d/rYSD/r45/nCJ6/chix/dxJ")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "YcVfx kQb6JR zqk5k F2tNLv")]
    public void Uuid7_FormatParse_Id22Dirty(string uuidText, string id22Text) {
        var parsedUuid = Uuid7.FromString(uuidText);
        var id22Uuid = Uuid7.FromId22String(id22Text);
        Assert.AreEqual(parsedUuid, id22Uuid);
        Assert.AreNotEqual(Uuid7.Empty, parsedUuid);
        Assert.AreNotEqual(Uuid7.Empty, id22Uuid);
        Assert.IsTrue(Uuid7.TryParseId22(id22Text, out var _));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("YcVfx kQb6JR zqk5k F2tNL")]  // too short
    [DataRow("YcVfx kQb6JR zqk5k F2tNLvv")]  // too long
    public void Uuid7_FormatParse_Id22Errors(string id22Text) {
        if (id22Text == null && !IsModernDotNet) {
            // Only NetStandard 2.0 throws ArgumentNullException
            Assert.ThrowsExactly<ArgumentNullException>(() => {
                Uuid7.FromId22String(id22Text);
            });
        } else {
            Assert.ThrowsExactly<FormatException>(() => {
                Uuid7.FromId22String(id22Text);
            });
        }
        Assert.IsFalse(Uuid7.TryParseId22(id22Text, out var _));
    }

}
