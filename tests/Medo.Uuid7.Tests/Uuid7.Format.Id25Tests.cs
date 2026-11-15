namespace Tests;
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

[TestClass]
public class Uuid7_Id25_Tests {

    private const bool IsModernDotNet =
#if NET6_0_OR_GREATER
        true;
#else
        false;
#endif


    [TestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0000000000000000000000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "0000000000000000000000001")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "0000000000000000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000006q3abpe675nu")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000006Q3ABPE675NU")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000jnpiacvek52kvka6to5ogn")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000JNPIACVEK52KVKA6TO5OGN")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "042kt5d5ybb4r50zg5p72g3f1")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "042KT5D5YBB4R50ZG5P72G3F1")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0856marbwnn9ha1yxbde4x6v2")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0856MARBWNN9HA1YXBDE4X6V2")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "7kvj3h1ornbf9nui5v297qw6q")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "7KVJ3H1ORNBF9NUI5V297QW6Q")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "usz5xbbiqsfq7s727n0pzr2xa")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "USZ5XBBIQSFQ7S727N0PZR2XA")]
    public void Uuid7_FormatParse_Id25(byte[] bytes, string id25Text) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(id25Text, uuid.ToId25String(), ignoreCase: true);
        Assert.AreEqual(uuid, Uuid7.FromId25String(id25Text));
        Assert.IsTrue(Uuid7.TryParseId25(id25Text, out var _));
    }

    [TestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "00/0jn/pi/acv/ek/52k/vk/a6t/o5/ogn")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "usz5x bbiqs fq7s7 27n0p zr2xa")]
    public void Uuid7_FormatParse_Id25Dirty(string uuidText, string id25Text) {
        var parsedUuid = Uuid7.Parse(uuidText);
        var id25Uuid = Uuid7.FromId25String(id25Text);
        Assert.AreEqual(parsedUuid, id25Uuid);
        Assert.AreNotEqual(Uuid7.Empty, parsedUuid);
        Assert.AreNotEqual(Uuid7.Empty, id25Uuid);
        Assert.IsTrue(Uuid7.TryParseId25(id25Text, out var _));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("000jnpiacvek52kvka6to5og")]  // too short
    [DataRow("000jnpiacvek52kvka6to5ogna")]  // too long
    public void Uuid7_FormatParse_Id25Errors(string id25Text) {
        if (id25Text == null && !IsModernDotNet) {
            // Only NetStandard 2.0 throws ArgumentNullException
            Assert.ThrowsExactly<ArgumentNullException>(() => {
                Uuid7.FromId25String(id25Text);
            });
        } else {
            Assert.ThrowsExactly<FormatException>(() => {
                Uuid7.FromId25String(id25Text);
            });
        }
        Assert.IsFalse(Uuid7.TryParseId25(id25Text, out var _));
    }

}
