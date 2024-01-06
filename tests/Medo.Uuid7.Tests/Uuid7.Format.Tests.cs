using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

namespace Tests;

public partial class Uuid7_Tests {

    private const bool IsModernDotNet =
#if NET6_0_OR_GREATER
        true;
#else
        false;
#endif


    [TestMethod]
    public void Uuid7_FormatParse_StringFormat() {
        var bytes = new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 };
        var uuid = new Uuid7(bytes);
        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", uuid.ToString(null));
        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", uuid.ToString(""));
        Assert.AreEqual("0001020334357677b8b9fafbfcfdfeff", uuid.ToString("N"));
        Assert.AreEqual("0001020334357677b8b9fafbfcfdfeff", uuid.ToString("n"));
        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", uuid.ToString("D"));
        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", uuid.ToString("d"));
        Assert.AreEqual("{00010203-3435-7677-b8b9-fafbfcfdfeff}", uuid.ToString("B"));
        Assert.AreEqual("{00010203-3435-7677-b8b9-fafbfcfdfeff}", uuid.ToString("b"));
        Assert.AreEqual("(00010203-3435-7677-b8b9-fafbfcfdfeff)", uuid.ToString("P"));
        Assert.AreEqual("(00010203-3435-7677-b8b9-fafbfcfdfeff)", uuid.ToString("p"));
        Assert.AreEqual("{0x00010203,0x3435,0x7677,{0xb8,0xb9,0xfa,0xfb,0xfc,0xfd,0xfe,0xff}}", uuid.ToString("X"));
        Assert.AreEqual("{0x00010203,0x3435,0x7677,{0xb8,0xb9,0xfa,0xfb,0xfc,0xfd,0xfe,0xff}}", uuid.ToString("x"));
        Assert.AreEqual("112drYSDr45nCJ6chixdxJ", uuid.ToString("2"));
        Assert.AreEqual("000jnpiacvek52kvka6to5ogn", uuid.ToString("5"));
        Assert.ThrowsException<FormatException>(() => {
            uuid.ToString("y");
        });
    }

    [TestMethod]
    public void Uuid7_FormatParse_ISpanFormattable() {
        var bytes = new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 };
        var uuid = new Uuid7(bytes);

        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", $"{uuid}");
        Assert.AreEqual("0001020334357677b8b9fafbfcfdfeff", $"{uuid:N}");
        Assert.AreEqual("0001020334357677b8b9fafbfcfdfeff", $"{uuid:n}");
        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", $"{uuid:D}");
        Assert.AreEqual("00010203-3435-7677-b8b9-fafbfcfdfeff", $"{uuid:d}");
        Assert.AreEqual("{00010203-3435-7677-b8b9-fafbfcfdfeff}", $"{uuid:B}");
        Assert.AreEqual("{00010203-3435-7677-b8b9-fafbfcfdfeff}", $"{uuid:b}");
        Assert.AreEqual("(00010203-3435-7677-b8b9-fafbfcfdfeff)", $"{uuid:P}");
        Assert.AreEqual("(00010203-3435-7677-b8b9-fafbfcfdfeff)", $"{uuid:p}");
        Assert.AreEqual("{0x00010203,0x3435,0x7677,{0xb8,0xb9,0xfa,0xfb,0xfc,0xfd,0xfe,0xff}}", $"{uuid:X}");
        Assert.AreEqual("{0x00010203,0x3435,0x7677,{0xb8,0xb9,0xfa,0xfb,0xfc,0xfd,0xfe,0xff}}", $"{uuid:x}");
        Assert.AreEqual("112drYSDr45nCJ6chixdxJ", $"{uuid:2}");
        Assert.AreEqual("000jnpiacvek52kvka6to5ogn", $"{uuid:5}");
        Assert.ThrowsException<FormatException>(() => {
            _ = $"{uuid:y}";
        });
    }


    [DataTestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "00000000-0000-0000-0000-000000000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "00000000-0000-0000-0000-000000000001")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "00000000-0000-0000-0000-000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000-0000-0000-0900-000000000000")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "00010203-3435-7677-b8b9-fafbfcfdfeff")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "01000000-0000-0000-0000-000000000000")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "02000000-0000-0000-0000-000000000000")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "ffffffff-ffff-ffff-ffff-ffffffffffff")]
    public void Uuid7_FormatParse_String(byte[] bytes, string text) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(text, uuid.ToString());
        Assert.AreEqual(uuid, Uuid7.Parse(text));
        Assert.IsTrue(Uuid7.TryParse(text, out var parsed));
        Assert.AreEqual(uuid, parsed);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("00010203-3435-7677-b8b9-fafbfcfdfef")]  // too short
    [DataRow("00010203-3435-7677-b8b9-fafbfcfdfefff")]  // too long
    public void Uuid7_FormatParse_StringErrors(string text) {
        Assert.IsFalse(Uuid7.TryParse(text, out _));
        Assert.IsFalse(Uuid7.TryParse(text, CultureInfo.InvariantCulture, out _));
        if (text == null) {
            Assert.ThrowsException<ArgumentNullException>(() => {
                Uuid7.Parse(text);
                Uuid7.Parse(text, CultureInfo.InvariantCulture);
            });
        } else {
            Assert.ThrowsException<FormatException>(() => {
                Uuid7.Parse(text);
                Uuid7.Parse(text, CultureInfo.InvariantCulture);
            });
        }
    }


    [DataTestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0000000000000000000000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "0000000000000000000000001")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "0000000000000000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000006q3abpe675nu")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000jnpiacvek52kvka6to5ogn")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "042kt5d5ybb4r50zg5p72g3f1")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0856marbwnn9ha1yxbde4x6v2")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "usz5xbbiqsfq7s727n0pzr2xa")]
    public void Uuid7_FormatParse_Id25(byte[] bytes, string id25Text) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(id25Text, uuid.ToId25String());
        Assert.AreEqual(uuid, Uuid7.FromId25String(id25Text));
    }

    [DataTestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "00/0jn/pi/acv/ek/52k/vk/a6t/o5/ogn")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "usz5x bbiqs fq7s7 27n0p zr2xa")]
    public void Uuid7_FormatParse_Id25Dirty(string uuidText, string id25Text) {
        var expectedUuid = Uuid7.Parse(uuidText);
        var id25Uuid = Uuid7.FromId25String(id25Text);
        Assert.AreEqual(expectedUuid, id25Uuid);
        Assert.AreNotEqual(Uuid7.Empty, expectedUuid);
        Assert.AreNotEqual(Uuid7.Empty, id25Uuid);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("usz5x bbiqs fq7s7 27n0p zr2x")]  // too short
    [DataRow("usz5x bbiqs fq7s7 27n0p zr2xaa")]  // too long
    public void Uuid7_FormatParse_Id25Errors(string text) {
        if (text == null && !IsModernDotNet) {
            // Only NetStandard 2.0 throws ArgumentNullException
            Assert.ThrowsException<ArgumentNullException>(() => {
                Uuid7.FromId25String(text);
            });
        } else {
            Assert.ThrowsException<FormatException>(() => {
                Uuid7.FromId25String(text);
            });
        }
    }


    [DataTestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "1111111111111111111111")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "1111111111111111111112")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "1111111111111111111113")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "111111111112WK48GNSUQf")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "112drYSDr45nCJ6chixdxJ")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "18AQGAut7N92awznwCnjuR")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "1FKoXLpmDjH4AtzasQaUoq")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "YcVfxkQb6JRzqk5kF2tNLv")]
    public void Uuid7_FormatParse_Id22(byte[] bytes, string id22Text) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(id22Text, uuid.ToId22String());
        Assert.AreEqual(uuid, Uuid7.FromId22String(id22Text));
    }

    [DataTestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "112d/rYSD/r45/nCJ6/chix/dxJ")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "YcVfx kQb6JR zqk5k F2tNLv")]
    public void Uuid7_FormatParse_Id22Dirty(string uuidText, string id22Text) {
        var expectedUuid = Uuid7.FromString(uuidText);
        var id22Uuid = Uuid7.FromId22String(id22Text);
        Assert.AreEqual(expectedUuid, id22Uuid);
        Assert.AreNotEqual(Uuid7.Empty, expectedUuid);
        Assert.AreNotEqual(Uuid7.Empty, id22Uuid);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("YcVfx kQb6JR zqk5k F2tNL")]  // too short
    [DataRow("YcVfx kQb6JR zqk5k F2tNLvv")]  // too long
    public void Uuid7_FormatParse_Id22Errors(string text) {
        if (text == null && !IsModernDotNet) {
            // Only NetStandard 2.0 throws ArgumentNullException
            Assert.ThrowsException<ArgumentNullException>(() => {
                Uuid7.FromId22String(text);
            });
        } else {
            Assert.ThrowsException<FormatException>(() => {
                Uuid7.FromId22String(text);
            });
        }
    }

}
