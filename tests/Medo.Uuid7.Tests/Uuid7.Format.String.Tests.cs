namespace Tests;
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

[TestClass]
public class Uuid7_String_Tests {

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
        Assert.AreEqual("000h40tn6pv7gf5tzcxztzgyzy", uuid.ToString("6"));
        Assert.ThrowsExactly<FormatException>(() => {
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
        Assert.AreEqual("000h40tn6pv7gf5tzcxztzgyzy", $"{uuid:6}");
        Assert.ThrowsExactly<FormatException>(() => {
            _ = $"{uuid:y}";
        });
    }


    [TestMethod]
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

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("00010203-3435-7677-b8b9-fafbfcfdfef")]  // too short
    [DataRow("00010203-3435-7677-b8b9-fafbfcfdfefff")]  // too long
    public void Uuid7_FormatParse_StringErrors(string text) {
        Assert.IsFalse(Uuid7.TryParse(text, out _));
        Assert.IsFalse(Uuid7.TryParse(text, CultureInfo.InvariantCulture, out _));
        if (text == null) {
            Assert.ThrowsExactly<ArgumentNullException>(() => {
                Uuid7.Parse(text);
                Uuid7.Parse(text, CultureInfo.InvariantCulture);
            });
        } else {
            Assert.ThrowsExactly<FormatException>(() => {
                Uuid7.Parse(text);
                Uuid7.Parse(text, CultureInfo.InvariantCulture);
            });
        }
    }

}
