using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

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
        var uuid1 = Uuid7.NewGuid();
        var uuid2 = Uuid7.NewGuid();
        Assert.AreNotEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid7_NewFromBytes() {
        var uuid = new Uuid7(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
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

        Assert.IsTrue(CompareArrays(bytes, uuid.ToByteArray()));
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

#if NET6_0 || NET7_0
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
    }

    [TestMethod]
    public void Uuid7_GuidToString() {
        var uuid = new Uuid7(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        var guid = uuid.ToGuid();
        Assert.AreEqual("01020304-0506-0708-090a-0b0c0d0e0f10", uuid.ToString());
        Assert.AreEqual("04030201-0605-0807-090a-0b0c0d0e0f10", guid.ToString());  // GUIDs are little-endian so string representation will not be the same
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
        Assert.AreEqual(uuid, guid);  // binary equality
        if (!BitConverter.IsLittleEndian) {
            Assert.AreEqual(guid, uuid);  // same only on BE platforms
        }
    }

    [TestMethod]
    public void Uuid7_OperatorToGuid() {
        Uuid7 uuid = Uuid7.NewUuid7();
        Guid guid = uuid;
        Assert.AreEqual(uuid, guid);  // binary equality
        if (!BitConverter.IsLittleEndian) {
            Assert.AreEqual(guid, uuid);  // same only on BE platforms
        }
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

        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid1.ToByteArray())) == 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);

        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid2.ToByteArray())) == 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid3.ToByteArray())) < 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid4.ToByteArray())) < 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid5.ToByteArray())) < 0);

        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid2.ToByteArray())) > 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid3.ToByteArray())) == 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid4.ToByteArray())) < 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid5.ToByteArray())) < 0);

        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid2.ToByteArray())) > 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid3.ToByteArray())) > 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid4.ToByteArray())) == 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid5.ToByteArray())) < 0);

        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid2.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid3.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid4.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid5.ToByteArray())) == 0);
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
    public void Uuid7_MarshalBytes() {
        var uuid = Uuid7.NewUuid7();

        int size = Marshal.SizeOf(uuid);
        Assert.AreEqual(16, size);

        var bytes = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(uuid, ptr, true);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);  // it's a test, no need for try/finally block

        Assert.IsTrue(CompareArrays(uuid.ToByteArray(), bytes));
    }

    [TestMethod]
    public void Uuid7_UnmarshalBytes() {
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        var ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        var uuid = (Uuid7)Marshal.PtrToStructure(ptr, typeof(Uuid7));
        Marshal.FreeHGlobal(ptr);

        Assert.IsTrue(CompareArrays(bytes, uuid.ToByteArray()));
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
        Assert.IsTrue(CompareArrays(uuidBytes, uuid.ToByteArray()));
    }

    [TestMethod]
    public void Uuid7_ToByteArray_BE() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);
        Assert.IsTrue(CompareArrays(uuidBytes, uuid.ToByteArray(bigEndian: true)));
    }

    [TestMethod]
    public void Uuid7_ToByteArray_LE() {
        var uuidBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuidBytesLE = new byte[] { 4, 3, 2, 1, 6, 5, 8, 7, 9, 10, 11, 12, 13, 14, 15, 16 };
        var uuid = new Uuid7(uuidBytes);
        Assert.IsTrue(CompareArrays(uuidBytesLE, uuid.ToByteArray(bigEndian: false)));
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

    private static bool CompareArrays(byte[] buffer1, byte[] buffer2) {
        var comparer = EqualityComparer<byte>.Default;
        if (buffer1.Length != buffer2.Length) { return false; }  // should not really happen
        for (int i = 0; i < buffer1.Length; i++) {
            if (!comparer.Equals(buffer1[i], buffer2[i])) {
                return false;
            }
        }
        return true;
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
