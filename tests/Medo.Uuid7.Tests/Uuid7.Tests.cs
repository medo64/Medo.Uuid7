using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;
using System.Runtime.Intrinsics;

namespace Tests;

[TestClass]
public partial class Uuid7_Tests {

    [TestMethod]
    public void Uuid7_Empty() {
        Assert.AreEqual(Guid.Empty, Uuid7.Empty.ToGuid());
        Assert.IsTrue(Uuid7.Empty.Equals(Guid.Empty));
        Assert.IsTrue(Uuid7.Empty.Equals(Guid.Empty));
        Assert.AreEqual("00000000-0000-0000-0000-000000000000", Uuid7.Empty.ToString());
        Assert.AreEqual("0000000000000000000000000", Uuid7.Empty.ToId25String());
    }

    [TestMethod]
    public void Uuid7_New() {
        var uuid1 = Uuid7.NewUuid7();
        var uuid2 = Uuid7.NewUuid7();
        Assert.AreNotEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid7_NewAtTimestamp() {
        var timestamp = new DateTimeOffset(1979, 07, 20, 20, 17, 00, TimeSpan.Zero);
        var uuid1 = Uuid7.NewUuid7(timestamp);
        var uuid2 = Uuid7.NewUuid7(timestamp);
        Assert.AreNotEqual(uuid1, uuid2);
        Assert.AreEqual(timestamp, uuid1.ToDateTimeOffset());
        Assert.AreEqual(timestamp, uuid2.ToDateTimeOffset());
    }

    [TestMethod]
    public void Uuid7_NewMsSqlUniqueIdentifier() {
        var guid1 = Uuid7.NewMsSqlUniqueIdentifier();
        var guid2 = Uuid7.NewMsSqlUniqueIdentifier();
        Assert.IsTrue(CompareMsSqlUniqueIdentifiers(guid1, guid2) < 0);
    }


    [TestMethod]
    public void Uuid7_NewGuid() {
        var guid1 = Uuid7.NewGuid();
        var guid2 = Uuid7.NewGuid();
        Assert.AreNotEqual(guid1, guid2);
        Assert.IsTrue(guid1 < guid2);
    }

    [TestMethod]
    public void Uuid7_NewGuidBE() {
        var guid1 = Uuid7.NewGuid();
        var guid2BE = Uuid7.NewGuid(bigEndian: true);
        var guid2 = new Guid(guid2BE.ToByteArray(), bigEndian: true);
        Assert.AreNotEqual(guid1, guid2);
        Assert.IsTrue(guid1 < guid2);
    }

    [TestMethod]
    public void Uuid7_NewGuidLE() {
        var guid1 = Uuid7.NewGuid();
        var guid2LE = Uuid7.NewGuid(bigEndian: false);
        var guid2 = new Guid(guid2LE.ToByteArray(), bigEndian: false);
        Assert.AreNotEqual(guid1, guid2);
        Assert.IsTrue(guid1 < guid2);
    }


    [TestMethod]
    public void Uuid7_NewFromBytes() {
        var uuid = new Uuid7(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
    }

    [TestMethod]
    public void Uuid7_NewFromGuid() {
        var guid = new Guid(new byte[] { 4, 3, 2, 1, 6, 5, 8, 7, 9, 10, 11, 12, 13, 14, 15, 16 });
        var uuid = new Uuid7(guid);
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", guid.ToString());
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
    }

    [TestMethod]
    public void Uuid7_NewFromGuidBE() {
        var guid = new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        var uuid = new Uuid7(guid, bigEndian: true);
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
        } else {
            Assert.AreEqual("04030201-0605-0807-090a-0b0c0d0e0f10", uuid.ToString());
        }
    }

    [TestMethod]
    public void Uuid7_NewFromGuidLE() {
        var guid = new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        var uuid = new Uuid7(guid, bigEndian: false);
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual("04030201-0605-0807-090a-0b0c0d0e0f10", uuid.ToString());
        } else {
            Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
        }
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Uuid7_NewFromSpan() {
        var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(bytes.AsSpan(1));
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
    }
#endif

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Uuid7_WriteToSpan() {
        var uuid = Uuid7.NewUuid7();

        var bytes = new byte[16];
        Assert.IsTrue(uuid.TryWriteBytes(bytes));

        Assert.IsTrue(CompareArrays(bytes, uuid.ToByteArray()) == 0);
    }

    [TestMethod]
    public void Uuid7_WriteToLargerSpan() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);

        var bytes = new byte[17];
        Assert.IsTrue(uuid.TryWriteBytes(bytes.AsSpan()[1..]));

        Assert.AreEqual("00-01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F-10", BitConverter.ToString(bytes));
    }

    [TestMethod]
    public void Uuid7_WriteToSmallSpan() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);

        var bytes = new byte[15];
        Assert.IsFalse(uuid.TryWriteBytes(bytes.AsSpan()));

        Assert.AreEqual("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00", BitConverter.ToString(bytes));
    }
#endif

    [TestMethod]
    public void Uuid7_HashCode() {
        var uuidBytes = new byte[] { 0xFA, 0xBB, 0x0D, 0xF2, 0xCB, 0xE8, 0xDD, 0x68, 0x5D, 0xC4, 0x84, 0xD8, 0xC6, 0x27, 0xEA, 0x4A };
        var uuid1 = new Uuid7(uuidBytes);
        var uuid2 = new Uuid7(uuidBytes);
        Assert.AreEqual(uuid1.GetHashCode(), uuid2.GetHashCode());
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Uuid7_HashCodeGuidCompatible() {
        var uuid = Uuid7.NewUuid7();
        var guid = uuid.ToGuid();
        Assert.AreEqual(uuid.GetHashCode(), guid.GetHashCode());
    }
#endif

    [TestMethod]
    public void Uuid7_GuidAndBack() {
        var uuid1 = Uuid7.NewUuid7();
        var guid = uuid1.ToGuid();
        var uuid2 = new Uuid7(guid);
        Assert.AreEqual(uuid1, uuid2);
        Assert.AreEqual(uuid1.ToString(), guid.ToString());
    }

    [TestMethod]
    public void Uuid7_GuidAndBackBE() {
        var uuid1 = Uuid7.NewUuid7();
        var guid = uuid1.ToGuid(bigEndian: true);
        var uuid2 = new Uuid7(guid, bigEndian: true);
        Assert.AreEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid7_GuidAndBackLE() {
        var uuid1 = Uuid7.NewUuid7();
        var guid = uuid1.ToGuid(bigEndian: false);
        var uuid2 = new Uuid7(guid, bigEndian: false);
        Assert.AreEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid7_GuidToString() {
        var uuid = new Uuid7(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        var guid = uuid.ToGuid();
        var guidLE = uuid.ToGuid(bigEndian: false);
        var guidBE = uuid.ToGuid(bigEndian: true);

        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", guid.ToString());
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", guidLE.ToString());
            Assert.AreEqual("04030201-0605-0807-090a-0b0c0d0e0f10", guidBE.ToString());
        } else {
            Assert.AreEqual("04030201-0605-0807-090a-0b0c0d0e0f10", guidLE.ToString());
            Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", guidBE.ToString());
        }
    }

    [TestMethod]
    public void Uuid7_TestAlwaysIncreasing() {
        var oldUuid = Uuid7.Empty;
        for (var i = 0; i < 1000000; i++) {  // assuming we're not generating more than 2^21 each millisecond, they will be monotonically increasing
            var newUuid = Uuid7.NewUuid7();
            Assert.IsTrue(UuidToNumber(newUuid) > UuidToNumber(oldUuid), $"Failed at iteration {i}\n{oldUuid}\n{newUuid}");  // using UuidToNumber intentionaly as to avoid trusting operator overloads
            oldUuid = newUuid;
        }
    }

    [TestMethod]
    public void Uuid7_TestUniqueness() {
        var set = new HashSet<Uuid7>();

        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 1000) {
            if (!set.Add(Uuid7.NewUuid7())) {
                throw new InvalidOperationException("Duplicate UUIDv7 generated.");
            }
        }
    }

    [TestMethod]
    public void Uuid7_TestThreadedUniqueness() {
        var set = new ConcurrentDictionary<Uuid7, nint>();

        Parallel.For(0, Math.Max(1, Environment.ProcessorCount / 2), (i) => {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 1000) {
                if (!set.TryAdd(Uuid7.NewUuid7(), 0)) {
                    throw new InvalidOperationException("Duplicate UUIDv4 generated.");
                }
            }
        });
    }

    [TestMethod]
    public void Uuid7_OperatorLessThan() {
        var uuid1 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid7(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        Assert.IsTrue(uuid1 < uuid2);
        Assert.IsTrue(uuid1 < uuid3);
        Assert.IsTrue(uuid1 < uuid4);
        Assert.IsTrue(uuid2 < uuid3);
        Assert.IsTrue(uuid3 < uuid4);
        Assert.IsTrue(uuid3 < uuid4);
        Assert.IsFalse(uuid1 > uuid2);
        Assert.IsFalse(uuid1 > uuid3);
        Assert.IsFalse(uuid1 > uuid4);
        Assert.IsFalse(uuid2 > uuid3);
        Assert.IsFalse(uuid3 > uuid4);
        Assert.IsFalse(uuid3 > uuid4);
        Assert.IsFalse(uuid1 == uuid2);
        Assert.IsFalse(uuid1 == uuid3);
        Assert.IsFalse(uuid1 == uuid4);
        Assert.IsFalse(uuid2 == uuid3);
        Assert.IsFalse(uuid3 == uuid4);
        Assert.IsFalse(uuid3 == uuid4);
    }

    [TestMethod]
    public void Uuid7_OperatorMoreThan() {
        var uuid1 = new Uuid7(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid2 = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid3 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid4 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        Assert.IsTrue(uuid1 > uuid2);
        Assert.IsTrue(uuid1 > uuid3);
        Assert.IsTrue(uuid1 > uuid4);
        Assert.IsTrue(uuid2 > uuid3);
        Assert.IsTrue(uuid3 > uuid4);
        Assert.IsTrue(uuid3 > uuid4);
        Assert.IsFalse(uuid1 < uuid2);
        Assert.IsFalse(uuid1 < uuid3);
        Assert.IsFalse(uuid1 < uuid4);
        Assert.IsFalse(uuid2 < uuid3);
        Assert.IsFalse(uuid3 < uuid4);
        Assert.IsFalse(uuid3 < uuid4);
        Assert.IsFalse(uuid1 == uuid2);
        Assert.IsFalse(uuid1 == uuid3);
        Assert.IsFalse(uuid1 == uuid4);
        Assert.IsFalse(uuid2 == uuid3);
        Assert.IsFalse(uuid3 == uuid4);
        Assert.IsFalse(uuid3 == uuid4);
    }

    [TestMethod]
    public void Uuid7_OperatorFromGuid() {
        Guid guid = Guid.NewGuid();
        Uuid7 uuid = guid;
        Assert.AreEqual(uuid.ToString(), guid.ToString());
    }

    [TestMethod]
    public void Uuid7_FromGuidLE() {
        Guid guid = Guid.NewGuid();
        Uuid7 uuid = Uuid7.FromGuid(guid, bigEndian: false);
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual(uuid.ToString(), guid.ToString());
        } else {
            Assert.AreEqual(BitConverter.ToString(guid.ToByteArray()), BitConverter.ToString(uuid.ToByteArray()));
        }
    }

    [TestMethod]
    public void Uuid7_FromGuidBE() {
        Guid guid = Guid.NewGuid();
        Uuid7 uuid = Uuid7.FromGuid(guid, bigEndian: true);
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual(BitConverter.ToString(guid.ToByteArray()), BitConverter.ToString(uuid.ToByteArray()));
        } else {
            Assert.AreEqual(uuid.ToString(), guid.ToString());
        }
    }

    [TestMethod]
    public void Uuid7_OperatorToGuid() {
        Uuid7 uuid = Uuid7.NewUuid7();
        Guid guid = uuid;
        Assert.AreEqual(uuid.ToString(), guid.ToString());
        if (!BitConverter.IsLittleEndian) {
            Assert.AreEqual(BitConverter.ToString(guid.ToByteArray()), BitConverter.ToString(uuid.ToByteArray()));
        }
    }


    [TestMethod]
    public void Uuid7_ToGuid() {
        Uuid7 uuid = Uuid7.NewUuid7();
        Guid guid = Uuid7.ToGuid(uuid, bigEndian: false);
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual(BitConverter.ToString(guid.ToByteArray()), BitConverter.ToString(uuid.ToByteArray()));
        } else {
            Assert.AreEqual(uuid.ToString(), guid.ToString());
        }
    }

    [TestMethod]
    public void Uuid7_ToGuidLE() {
        Uuid7 uuid = Uuid7.NewUuid7();
        Guid guid = Uuid7.ToGuid(uuid, bigEndian: false);
        if (BitConverter.IsLittleEndian) {
            Assert.AreEqual(BitConverter.ToString(guid.ToByteArray()), BitConverter.ToString(uuid.ToByteArray()));
        } else {
            Assert.AreEqual(uuid.ToString(), guid.ToString());
        }
    }

    [TestMethod]
    public void Uuid7_ToGuidBE() {
        Uuid7 uuid = Uuid7.NewUuid7();
        Guid guid = Uuid7.ToGuid(uuid, bigEndian: true);
        Assert.AreEqual(uuid.ToString(), guid.ToString());
        if (!BitConverter.IsLittleEndian) {
            Assert.AreEqual(guid, uuid);  // same only on BE platforms
        }
    }

    [TestMethod]
    public void Guid_NoConstructor() {
        var list = new Guid[1];

        Assert.AreEqual(Guid.Empty, list[0]);
    }

    [TestMethod]
    public void Uuid7_NoConstructor_Equals() {
        var list = new Uuid7[1];

        Assert.IsTrue(list[0].Equals(Uuid7.Empty));
        Assert.IsTrue(Uuid7.Empty.Equals(list[0]));
        Assert.AreEqual(list[0], Uuid7.Empty);
        Assert.AreEqual(Uuid7.Empty, list[0]);
    }

    [TestMethod]
    public void Uuid7_NoConstructor_EqualsGuid() {
        var list = new Uuid7[1];

        Assert.IsTrue(list[0].Equals(Guid.Empty));
        Assert.AreEqual((Guid)list[0], Guid.Empty);
        if (!BitConverter.IsLittleEndian) {
            Assert.AreEqual(Guid.Empty, list[0]);
            Assert.AreEqual(Guid.Empty, Uuid7.ToGuid(list[0], true));
            Assert.AreEqual(Guid.Empty, list[0].ToGuid(true));
        } else {
            Assert.AreEqual(Guid.Empty, Uuid7.ToGuid(list[0], false));
            Assert.AreEqual(Guid.Empty, list[0].ToGuid(false));
        }
        Assert.AreEqual(Guid.Empty.ToString(), list[0].ToString());
    }

    [TestMethod]
    public void Uuid7_NoConstructor_HashCode() {
        var listU = new Uuid7[1];
        var listG = new Guid[1];
        Assert.AreEqual(listU[0].GetHashCode(), listG[0].GetHashCode());
    }

    [TestMethod]
    public void Uuid7_NoConstructor_ToGuid() {
        var list = new Uuid7[1];
        Assert.AreEqual(Guid.Empty, list[0].ToGuid());
        Assert.AreEqual(Guid.Empty, list[0].ToGuid(bigEndian: true));
        Assert.AreEqual(Guid.Empty, list[0].ToGuid(bigEndian: false));
    }

    [TestMethod]
    public void Uuid7_NoConstructor_ToByteArray() {
        var list = new Uuid7[1];
        Assert.AreEqual("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00", BitConverter.ToString(list[0].ToByteArray()));
        Assert.AreEqual("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00", BitConverter.ToString(list[0].ToByteArray(bigEndian: false)));
        Assert.AreEqual("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00", BitConverter.ToString(list[0].ToByteArray(bigEndian: true)));
    }

    [TestMethod]
    public void Uuid7_NoConstructor_ToDateTime() {
        var list = new Uuid7[1];
        Assert.AreEqual(DateTime.MinValue, list[0].ToDateTime());
        Assert.AreEqual(DateTimeOffset.MinValue, list[0].ToDateTimeOffset());
    }


    [TestMethod]
    public void Uuid7_CompareTo() {
        var uuid1 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid7(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.CompareTo(uuid1) == 0);
        Assert.IsTrue(uuid1.CompareTo(uuid2) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid3) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid4) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid2.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid2.CompareTo(uuid2) == 0);
        Assert.IsTrue(uuid2.CompareTo(uuid3) < 0);
        Assert.IsTrue(uuid2.CompareTo(uuid4) < 0);
        Assert.IsTrue(uuid2.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid3.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid3.CompareTo(uuid2) > 0);
        Assert.IsTrue(uuid3.CompareTo(uuid3) == 0);
        Assert.IsTrue(uuid3.CompareTo(uuid4) < 0);
        Assert.IsTrue(uuid3.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid4.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid2) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid3) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid4) == 0);
        Assert.IsTrue(uuid4.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid5.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid2) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid3) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid4) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid5) == 0);
    }

    [TestMethod]
    public void Uuid7_CompareToGuid() {
        var uuid1 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid7(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.CompareTo(uuid1.ToGuid()) == 0);
        Assert.IsTrue(uuid1.CompareTo(uuid2.ToGuid()) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid2.ToGuid()) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid2.ToGuid()) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid2.ToGuid()) < 0);

        Assert.IsTrue(uuid2.CompareTo(uuid1.ToGuid()) > 0);
        Assert.IsTrue(uuid2.CompareTo(uuid2.ToGuid()) == 0);
        Assert.IsTrue(uuid2.CompareTo(uuid3.ToGuid()) < 0);
        Assert.IsTrue(uuid2.CompareTo(uuid4.ToGuid()) < 0);
        Assert.IsTrue(uuid2.CompareTo(uuid5.ToGuid()) < 0);

        Assert.IsTrue(uuid3.CompareTo(uuid1.ToGuid()) > 0);
        Assert.IsTrue(uuid3.CompareTo(uuid2.ToGuid()) > 0);
        Assert.IsTrue(uuid3.CompareTo(uuid3.ToGuid()) == 0);
        Assert.IsTrue(uuid3.CompareTo(uuid4.ToGuid()) < 0);
        Assert.IsTrue(uuid3.CompareTo(uuid5.ToGuid()) < 0);

        Assert.IsTrue(uuid4.CompareTo(uuid1.ToGuid()) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid2.ToGuid()) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid3.ToGuid()) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid4.ToGuid()) == 0);
        Assert.IsTrue(uuid4.CompareTo(uuid5.ToGuid()) < 0);

        Assert.IsTrue(uuid5.CompareTo(uuid1.ToGuid()) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid2.ToGuid()) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid3.ToGuid()) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid4.ToGuid()) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid5.ToGuid()) == 0);
    }

    [TestMethod]
    public void Uuid7_Equals() {
        var uuid1 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid7(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.Equals(uuid1));
        Assert.IsFalse(uuid1.Equals(uuid2));
        Assert.IsFalse(uuid1.Equals(uuid3));
        Assert.IsFalse(uuid1.Equals(uuid4));
        Assert.IsFalse(uuid1.Equals(uuid5));

        Assert.IsFalse(uuid2.Equals(uuid1));
        Assert.IsTrue(uuid2.Equals(uuid2));
        Assert.IsFalse(uuid2.Equals(uuid3));
        Assert.IsFalse(uuid2.Equals(uuid4));
        Assert.IsFalse(uuid2.Equals(uuid5));

        Assert.IsFalse(uuid3.Equals(uuid1));
        Assert.IsFalse(uuid3.Equals(uuid2));
        Assert.IsTrue(uuid3.Equals(uuid3));
        Assert.IsFalse(uuid3.Equals(uuid4));
        Assert.IsFalse(uuid3.Equals(uuid5));

        Assert.IsFalse(uuid4.Equals(uuid1));
        Assert.IsFalse(uuid4.Equals(uuid2));
        Assert.IsFalse(uuid4.Equals(uuid3));
        Assert.IsTrue(uuid4.Equals(uuid4));
        Assert.IsFalse(uuid4.Equals(uuid5));

        Assert.IsFalse(uuid5.Equals(uuid1));
        Assert.IsFalse(uuid5.Equals(uuid2));
        Assert.IsFalse(uuid5.Equals(uuid3));
        Assert.IsFalse(uuid5.Equals(uuid4));
        Assert.IsTrue(uuid5.Equals(uuid5));
    }

    [TestMethod]
    public void Uuid7_EqualsGuid() {
        var uuid1 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid7(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid7(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid7(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid2.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsTrue(uuid2.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid2.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid2.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid2.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid3.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid3.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsTrue(uuid3.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid3.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid3.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid4.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid4.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid4.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsTrue(uuid4.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid4.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid5.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid5.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid5.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid5.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsTrue(uuid5.Equals(new Guid(uuid5.ToByteArray())));
    }


    [TestMethod]
    public void Uuid7_Fill() {
        var initialTime = DateTimeOffset.FromUnixTimeMilliseconds(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());  // has to be millisecond precision only
        var uuids = new Uuid7[10000];
        Uuid7.Fill(uuids);

        var prevUuid = Uuid7.Empty;
        foreach (var uuid in uuids) {
            var bytes = uuid.ToByteArray();
            Assert.AreNotEqual(bytes[0] + bytes[1] + bytes[2] + bytes[3], 0);  // not full of zeros

            Assert.IsTrue(uuid.ToDateTimeOffset().Ticks >= initialTime.Ticks);

            Assert.AreNotEqual(Uuid7.Empty, uuid);
            Assert.IsTrue(uuid > prevUuid);
            prevUuid = uuid;
        }
    }

    [TestMethod]
    public void Uuid7_FillAtTimestamp() {
        var timestamp = new DateTimeOffset(1979, 07, 20, 20, 17, 00, TimeSpan.Zero);
        var uuids = new Uuid7[10000];
        Uuid7.Fill(uuids, timestamp);

        var prevUuid = Uuid7.Empty;
        foreach (var uuid in uuids) {
            var bytes = uuid.ToByteArray();
            Assert.AreNotEqual(bytes[0] + bytes[1] + bytes[2] + bytes[3], 0);  // not full of zeros

            Assert.AreEqual(timestamp, uuid.ToDateTimeOffset());

            Assert.AreNotEqual(Uuid7.Empty, uuid);
            Assert.IsTrue(uuid > prevUuid);
            prevUuid = uuid;
        }
    }

    [TestMethod]
    public void Uuid7_FillGuid() {
        var guids = new Guid[10000];
        Uuid7.FillGuid(guids);

        var prevGuid = Guid.Empty;
        foreach (var guid in guids) {
            Assert.IsTrue(CompareArrays(prevGuid.ToByteArray(bigEndian: true), guid.ToByteArray(bigEndian: true)) < 0);
            prevGuid = guid;
        }
    }

    [TestMethod]
    public void Uuid7_FillGuidBE() {
        var guids = new Guid[10000];
        Uuid7.FillGuid(guids, bigEndian: true);

        var prevGuid = Uuid7.Empty;
        foreach (var guid in guids) {
            Assert.IsTrue(CompareArrays(prevGuid.ToByteArray(bigEndian: true), guid.ToByteArray(bigEndian: true)) < 0);
            prevGuid = guid;
        }
    }

    [TestMethod]
    public void Uuid7_FillGuidLE() {
        var guids = new Guid[10000];
        Uuid7.FillGuid(guids, bigEndian: false);

        var prevGuid = Guid.Empty;
        foreach (var guid in guids) {
            Assert.IsTrue(CompareArrays(prevGuid.ToByteArray(bigEndian: true), guid.ToByteArray(bigEndian: true)) < 0);
            prevGuid = guid;
        }
    }

    [TestMethod]
    public void Uuid7_FillMsSqlUniqueIdentifier() {
        var guids = new Guid[10000];
        Uuid7.FillMsSqlUniqueIdentifier(guids);

        var prevGuid = Guid.Empty;
        foreach (var guid in guids) {
            Assert.IsTrue(CompareMsSqlUniqueIdentifiers(prevGuid, guid) < 0);
            prevGuid = guid;
        }
    }

    [TestMethod]
    public void Uuid7_FillMsSqlUniqueIdentifierAtTimestamp() {
        var timestamp = new DateTimeOffset(1979, 07, 20, 20, 17, 00, TimeSpan.Zero);

        var guids = new Guid[10000];
        Uuid7.FillMsSqlUniqueIdentifier(guids, timestamp);

        var prevGuid = Guid.Empty;
        foreach (var guid in guids) {
            Assert.IsTrue(CompareMsSqlUniqueIdentifiers(prevGuid, guid) < 0);
            prevGuid = guid;
        }
    }


    [TestMethod]
    public void Uuid7_MarshalBytes() {
        var uuid = Uuid7.NewUuid7();

        int size = Marshal.SizeOf(uuid);
        Assert.AreEqual(16, size);

        var bytes = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(uuid, ptr, true);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);  // it's a test, no need for try/finally block

        Assert.IsTrue(CompareArrays(uuid.ToByteArray(), bytes) == 0);
    }

    [TestMethod]
    public void Uuid7_UnmarshalBytes() {
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        var ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        var uuid = (Uuid7)Marshal.PtrToStructure(ptr, typeof(Uuid7));
        Marshal.FreeHGlobal(ptr);

        Assert.IsTrue(CompareArrays(bytes, uuid.ToByteArray()) == 0);
    }


    [TestMethod]
    public void Guid_Compare1() {
        var guidL = new Guid(new byte[] { 0x7C, 0xE2, 0xEA, 0x6E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var guidM = new Guid(new byte[] { 0x7C, 0xE2, 0xEA, 0x6F, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var guidH = new Guid(new byte[] { 0x7C, 0xE2, 0xEA, 0x70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        Assert.IsTrue(guidL < guidM);
        Assert.IsTrue(guidL < guidH);
        Assert.IsTrue(guidM < guidH);
        Assert.IsTrue(guidH > guidL);
        Assert.IsTrue(guidH > guidM);
        Assert.IsTrue(guidM > guidL);
    }

    [TestMethod]
    public void Guid_Compare2() {
        var guidL = new Guid(new byte[] { 0x7B, 0xE2, 0xEA, 0x6F, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var guidM = new Guid(new byte[] { 0x7C, 0xE2, 0xEA, 0x6F, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var guidH = new Guid(new byte[] { 0x7D, 0xE2, 0xEA, 0x6F, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        Assert.IsTrue(guidL < guidM);
        Assert.IsTrue(guidL < guidH);
        Assert.IsTrue(guidM < guidH);
        Assert.IsTrue(guidH > guidL);
        Assert.IsTrue(guidH > guidM);
        Assert.IsTrue(guidM > guidL);
    }

    [TestMethod]
    public void Uuid_Compare1() {
        var uuidAL = new Uuid7(new byte[] { 0x7C, 0xE2, 0xEA, 0x6E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var uuidAM = new Uuid7(new byte[] { 0x7C, 0xE2, 0xEA, 0x6F, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var uuidAH = new Uuid7(new byte[] { 0x7C, 0xE2, 0xEA, 0x70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        Assert.IsTrue(uuidAL < uuidAM);
        Assert.IsTrue(uuidAL < uuidAH);
        Assert.IsTrue(uuidAM < uuidAH);
        Assert.IsTrue(uuidAH > uuidAL);
        Assert.IsTrue(uuidAH > uuidAM);
        Assert.IsTrue(uuidAM > uuidAL);
    }

    [TestMethod]
    public void Uuid_Compare2() {
        var uuidL = new Uuid7(new byte[] { 0x7C, 0xE2, 0xEA, 0x6E, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var uuidM = new Uuid7(new byte[] { 0x7C, 0xE2, 0xEA, 0x6F, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        var uuidH = new Uuid7(new byte[] { 0x7C, 0xE2, 0xEA, 0x70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        Assert.IsTrue(uuidL < uuidM);
        Assert.IsTrue(uuidL < uuidH);
        Assert.IsTrue(uuidM < uuidH);
        Assert.IsTrue(uuidH > uuidL);
        Assert.IsTrue(uuidH > uuidM);
        Assert.IsTrue(uuidM > uuidL);
    }


    [TestMethod]
    public void Uuid_Ssse3() {
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var shuffle_mask = System.Runtime.Intrinsics.Vector128.Create<byte>([3, 2, 1, 0, 5, 4, 7, 6, 8, 9, 10, 11, 12, 13, 14, 15]);
        var vector1 = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Runtime.Intrinsics.Vector128<byte>>(ref bytes[0]);
        var vectorShuff = System.Runtime.Intrinsics.X86.Ssse3.Shuffle(vector1, shuffle_mask);
        vectorShuff.StoreUnsafe(ref bytes[0]);
        Assert.AreEqual("04-03-02-01-06-05-08-07-09-0A-0B-0C-0D-0E-0F-10", BitConverter.ToString(bytes));
    }


    [TestMethod]
    public void Uuid7_Equals_Null() {  // https://github.com/medo64/Medo.Uuid7/issues/1
        var x = Uuid7.NewUuid7();
        var method = typeof(IEquatable<Uuid7>).GetMethod("Equals");
        var result = method.Invoke(x, new object[] { null });
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Uuid7_Equals_NullGuid() {  // https://github.com/medo64/Medo.Uuid7/issues/1
        var x = Uuid7.NewUuid7();
        var method = typeof(IEquatable<Guid>).GetMethod("Equals");
        var result = method.Invoke(x, new object[] { null });
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Uuid7_CompareTo_Null() {  // https://github.com/medo64/Medo.Uuid7/issues/1
        var x = Uuid7.NewUuid7();
        var method = typeof(IComparable<Uuid7>).GetMethod("CompareTo");
        var result = method.Invoke(x, new object[] { null });
        Assert.AreEqual(+1, result);
    }

    [TestMethod]
    public void Uuid7_CompareTo_NullGuid() {  // https://github.com/medo64/Medo.Uuid7/issues/1
        var x = Uuid7.NewUuid7();
        var method = typeof(IComparable<Guid>).GetMethod("CompareTo");
        var result = method.Invoke(x, new object[] { null });
        Assert.AreEqual(+1, result);
    }

    [TestMethod]
    public void Uuid7_ToByteArray() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);
        Assert.IsTrue(CompareArrays(uuidBytes, uuid.ToByteArray()) == 0);
    }

    [TestMethod]
    public void Uuid7_ToByteArray_BE() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);
        Assert.IsTrue(CompareArrays(uuidBytes, uuid.ToByteArray(bigEndian: true)) == 0);
    }

    [TestMethod]
    public void Uuid7_ToByteArray_LE() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuidBytesLE = new byte[] { 4, 3, 2, 1, 6, 5, 8, 7, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);
        Assert.IsTrue(CompareArrays(uuidBytesLE, uuid.ToByteArray(bigEndian: false)) == 0);
    }


    [TestMethod]
    public void Uuid7_MinMax() {
        Assert.AreEqual("00000000-0000-0000-0000-000000000000", Uuid7.Empty.ToString());
        Assert.AreEqual("00000000-0000-0000-0000-000000000000", Uuid7.MinValue.ToString());
        Assert.AreEqual("ffffffff-ffff-ffff-ffff-ffffffffffff", Uuid7.MaxValue.ToString());
        Assert.AreEqual(Uuid7.Empty, Uuid7.MinValue);
    }


    [TestMethod]
    public void Uuid7_ToDateTime() {
        var uuid = Uuid7.Parse("018c051e-b87a-7404-ae7f-50e3be711102");
        var uuidTime = uuid.ToDateTime();
        Assert.AreEqual(new DateTime(2023, 11, 25, 06, 15, 48, 602, DateTimeKind.Utc), uuidTime);
        Assert.AreEqual(true, uuidTime.Kind == DateTimeKind.Utc);
    }

    [TestMethod]
    public void Uuid7_ToDateTimeOffset() {
        var uuid = Uuid7.Parse("018c051e-b87a-7404-ae7f-50e3be711102");
        var uuidTime = uuid.ToDateTimeOffset();
        Assert.AreEqual(new DateTimeOffset(2023, 11, 25, 06, 15, 48, 602, TimeSpan.Zero), uuidTime);
        Assert.AreEqual(TimeSpan.Zero, uuidTime.Offset);
    }


    [TestMethod]
    public void Uuid7_Version() {
#if NET9_0_OR_GREATER
        var guid = Guid.CreateVersion7();
        Assert.AreEqual(7, guid.Version);
#else
        var guid = Uuid7.NewUuid7().ToGuid();
#endif
        var uuid = (Uuid7)guid;
        Assert.AreEqual(7, uuid.Version);
    }

#if NET9_0_OR_GREATER
    [TestMethod]
    public void Uuid7_Variant() {
        var guid = Guid.CreateVersion7();
        var variant = guid.Variant;

        var uuid = (Uuid7)guid;
        Assert.AreEqual(variant, uuid.Variant);
    }
#endif


    [TestMethod]
    public void Uuid7_ToDateTimeInvalidVersion() {
        var uuid = Uuid7.Parse("018c051e-b87a-6404-ae7f-50e3be711102");
        Assert.ThrowsException<InvalidOperationException>(() => uuid.ToDateTimeOffset());
    }

    [TestMethod]
    public void Uuid7_ToDateTimeOffsetInvalidVersion() {
        var uuid = Uuid7.Parse("018c051e-b87a-6404-ae7f-50e3be711102");
        Assert.ThrowsException<InvalidOperationException>(() => uuid.ToDateTimeOffset());
    }


    #region Helpers

#if NET6_0_OR_GREATER
    private BigInteger UuidToNumber(Uuid7 uuid) {
        return new BigInteger(uuid.ToByteArray(), isUnsigned: true, isBigEndian: true);
    }
#else
    private BigInteger UuidToNumber(Uuid7 uuid) {
        var bytes = uuid.ToByteArray();
        if (BitConverter.IsLittleEndian) {
            Array.Reverse(bytes);
        }
        var bytes2 = new byte[bytes.Length + 1];
        Buffer.BlockCopy(bytes, 0, bytes2, 1, bytes.Length);
        return new BigInteger(bytes2);
    }
#endif

    private static int CompareArrays(byte[] buffer1, byte[] buffer2) {
        if (buffer1.Length != buffer2.Length) { throw new InvalidOperationException(); }  // should not really happen
        for (int i = 0; i < buffer1.Length; i++) {
            if (buffer1[i] < buffer2[i]) { return -1; }
            if (buffer1[i] > buffer2[i]) { return +1; }
        }
        return 0;
    }

    private static int CompareMsSqlUniqueIdentifiers(Guid guid1, Guid guid2) {
        var bytes1 = guid1.ToByteArray();
        var bytes2 = guid2.ToByteArray();
        foreach (var i in new[] { 10, 11, 12, 13, 14, 15, 8, 9, 6, 7, 4, 5, 0, 1, 2, 3 }) {  // https://web.archive.org/web/20120628234912/http://blogs.msdn.com/b/sqlprogrammability/archive/2006/11/06/how-are-guids-compared-in-sql-server-2005.aspx
            if (bytes1[i] < bytes2[i]) { return -1; }
            if (bytes1[i] > bytes2[i]) { return +1; }
        }
        return 0;
    }

    #endregion Helpers

}
