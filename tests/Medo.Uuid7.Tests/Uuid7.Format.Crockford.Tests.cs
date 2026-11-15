namespace Tests;
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

[TestClass]
public class Uuid7_Crockford_Tests {

    private const bool IsModernDotNet =
#if NET6_0_OR_GREATER
        true;
#else
        false;
#endif


    [TestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "00000000000000000000000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "00000000000000000000000004")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "00000000000000000000000008")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000002800000000000")]
    [DataRow(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, "000G40R40M30E209185GR38E1W")]
    [DataRow(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, "000g40r40m30e209185gr38e1w")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000G40SM6NV7FE5SZBXZSZFYZW")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000g40sm6nv7fe5szbxzszfyzw")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "04000000000000000000000000")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "08000000000000000000000000")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "85146H258S3MGJAA9D64TKJFA0")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "85146h258s3mgjaa9d64tkjfa0")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "ZZZZZZZZZZZZZZZZZZZZZZZZZW")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
    public void Uuid7_FormatParse_Crockford(byte[] bytes, string crockfordText) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(crockfordText, uuid.ToCrockfordString(), ignoreCase: true);
        Assert.AreEqual(uuid, Uuid7.FromCrockfordString(crockfordText));
        Assert.IsTrue(Uuid7.TryParseCrockford(crockfordText, out var _));
    }

    [TestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "00000000000000000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "00000000000000000000000007")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "00000000000000000000000009")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000002800000000002")]
    [DataRow(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, "000G40R40M30E209185GR38E1X")]
    [DataRow(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, "000g40r40m30e209185gr38e1x")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000G40SM6NV7FE5SZBXZSZFYZY")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000g40sm6nv7fe5szbxzszfyzy")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "04000000000000000000000003")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "08000000000000000000000001")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "85146H258S3MGJAA9D64TKJFA3")]
    [DataRow(new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 }, "85146h258s3mgjaa9d64tkjfa3")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "ZZZZZZZZZZZZZZZZZZZZZZZZZY")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "zzzzzzzzzzzzzzzzzzzzzzzzzy")]
    public void Uuid7_FormatParse_CrockfordWithChecksum(byte[] bytes, string crockfordText) {
        var uuid = new Uuid7(bytes);
        Assert.AreEqual(crockfordText, uuid.ToCrockfordString(uppercase: false, includeChecksum: true), ignoreCase: true);
        Assert.AreEqual(uuid, Uuid7.FromCrockfordString(crockfordText));
        Assert.IsTrue(Uuid7.TryParseCrockford(crockfordText, out var _));
    }

    [TestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "00/0g4/0sm/6nv/7fe/5sz/bxz/szf/yzy")]
    [DataRow("00010203-0405-0607-0809-0a0b0c0d0e0f", "Oo0G40R40M30E209185GR38E1X")]
    [DataRow("00010203-0405-0607-0809-0a0b0c0d0e0f", "000G40R40M30E209l85GR38EiX")]
    [DataRow("00010203-0405-0607-0809-0a0b0c0d0e0f", "000G40R40M30E209L85GR38EIX")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "ZZ zzz zzz zzz zzz zzz zzz zzz zzy")]
    public void Uuid7_FormatParse_CrockfordDirty(string uuidText, string crockfordText) {
        var plainUuid = Uuid7.Parse(uuidText);
        var crockfordUuid = Uuid7.FromCrockfordString(crockfordText);
        Assert.AreEqual(plainUuid, crockfordUuid);
        Assert.AreNotEqual(Uuid7.Empty, plainUuid);
        Assert.AreNotEqual(Uuid7.Empty, crockfordUuid);
        Assert.IsTrue(Uuid7.TryParseCrockford(crockfordText, out var _));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("ZZZZZ ZZZZZ ZZZZZ ZZZZZ ZZZ ZZZ")]
    public void Uuid7_FormatParse_CrockfordChecksumError(string text) {
        Assert.ThrowsExactly<FormatException>(() => {
            Uuid7.FromCrockfordString(text);
        });
        Assert.IsFalse(Uuid7.TryParseCrockford(text, out var _));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("X")]
    [DataRow("000G4 0SM6N V7FE5 SZBXZ SZF YZ")]  // too short
    [DataRow("000G4 0SM6N V7FE5 SZBXZ SZF YZY0")]  // too long
    public void Uuid7_FormatParse_CrockfordErrors(string text) {
        if (text == null && !IsModernDotNet) {
            // Only NetStandard 2.0 throws ArgumentNullException
            Assert.ThrowsExactly<ArgumentNullException>(() => {
                Uuid7.FromCrockfordString(text);
            });
        } else {
            Assert.ThrowsExactly<FormatException>(() => {
                Uuid7.FromCrockfordString(text);
            });
        }
        Assert.IsFalse(Uuid7.TryParseCrockford(text, out var _));
    }

}
