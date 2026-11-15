namespace Tests;
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

[TestClass]
public class Uuid7_Id26_Tests {

    private const bool IsModernDotNet =
#if NET6_0_OR_GREATER
        true;
#else
        false;
#endif


    [TestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "00000000000000000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "00000000000000000000000005")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "0000000000000000000000000b")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000002800000000000")]
    [DataRow(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, "000h40s40n30f209185hs38f1y")]
    [DataRow(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, "000H40S40N30F209185HS38F1Y")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000h40tn6pv7gf5tzcxztzgyzy")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000H40TN6PV7GF5TZCXZTZGYZY")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "04000000000000000000000000")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "08000000000000000000000002")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "85146j258t3nhkbb9e64umkgb2")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "85146J258T3NHKBB9E64UMKGB2")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "zzzzzzzzzzzzzzzzzzzzzzzzzy")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "ZZZZZZZZZZZZZZZZZZZZZZZZZY")]
    public void Uuid7_FormatParse_Id26(byte[] bytes, string id26Text) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(id26Text, uuid.ToId26String(), ignoreCase: true);
        Assert.AreEqual(uuid, Uuid7.FromId26String(id26Text));
        Assert.IsTrue(Uuid7.TryParseId26(id26Text, out var _));
    }

    [TestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "00/0h4/0tn/6pv/7gf/5tz/cxz/tzg/yzy")]
    [DataRow("00010203-0405-0607-0809-0a0b0c0d0e0f", "Oo0h40s40n30f209185hs38f1y")]
    [DataRow("00010203-0405-0607-0809-0a0b0c0d0e0f", "000h40s40n30f209L85hs38fIy")]
    [DataRow("00010203-0405-0607-0809-0a0b0c0d0e0f", "000h40s40n30f209l85hs38fiy")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "zz zzz zzz zzz zzz zzz zzz zzz zzy")]
    public void Uuid7_FormatParse_Id26Dirty(string uuidText, string id26Text) {
        var plainUuid = Uuid7.Parse(uuidText);
        var id26Uuid = Uuid7.FromId26String(id26Text);
        Assert.AreEqual(plainUuid, id26Uuid);
        Assert.AreNotEqual(Uuid7.Empty, plainUuid);
        Assert.AreNotEqual(Uuid7.Empty, id26Uuid);
        Assert.IsTrue(Uuid7.TryParseId26(id26Text, out var _));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("000h4 0tn6p v7gf5 tzcxz tzg yz")]  // too short
    [DataRow("000h4 0tn6p v7gf5 tzcxz tzg yzyz")]  // too long
    public void Uuid7_FormatParse_Id26Errors(string id26Text) {
        if (id26Text == null && !IsModernDotNet) {
            // Only NetStandard 2.0 throws ArgumentNullException
            Assert.ThrowsExactly<ArgumentNullException>(() => {
                Uuid7.FromId26String(id26Text);
            });
        } else {
            Assert.ThrowsExactly<FormatException>(() => {
                Uuid7.FromId26String(id26Text);
            });
        }
        Assert.IsFalse(Uuid7.TryParseId26(id26Text, out var _));
    }


    [TestMethod]
    public void Uuid7_FormatParse_26Format_1() {
        var bytes = new byte[] {
            0b_0000_1000, 0b_0100_0010, 0b_0001_0000, 0b_1000_0100,
            0b_0010_0001, 0b_0000_1000, 0b_0100_0010, 0b_0001_0000,
            0b_1000_0100, 0b_0010_0001, 0b_0000_1000, 0b_0100_0010,
            0b_0001_0000, 0b_1000_0100, 0b_0010_0001, 0b_0000_1000,
        };
        var uuid = new Uuid7(bytes);
        Assert.AreEqual("11111111111111111111111111", uuid.ToId26String());
        Assert.AreEqual(BitConverter.ToString(bytes),
                        BitConverter.ToString(Uuid7.FromId26String(uuid.ToId26String()).ToByteArray()));
    }

    [TestMethod]
    public void Uuid7_FormatParse_26Format_2() {
        var bytes = new byte[] {
            0b_0010_0001, 0b_0000_1000, 0b_0100_0010, 0b_0001_0000,
            0b_1000_0100, 0b_0010_0001, 0b_0000_1000, 0b_0100_0010,
            0b_0001_0000, 0b_1000_0100, 0b_0010_0001, 0b_0000_1000,
            0b_0100_0010, 0b_0001_0000, 0b_1000_0100, 0b_0010_0001,
        };
        var uuid = new Uuid7(bytes);
        Assert.AreEqual("44444444444444444444444446", uuid.ToId26String());
        Assert.AreEqual(BitConverter.ToString(bytes),
                        BitConverter.ToString(Uuid7.FromId26String(uuid.ToId26String()).ToByteArray()));
    }

}
