using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

namespace Tests;

[TestClass]
public class Uuid4_Tests {

    [TestMethod]
    public void Uuid4_New() {
        var uuid1 = Uuid7.NewUuid4();
        var uuid2 = Uuid7.NewUuid4();
        Assert.AreNotEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid4_GuidAndBack() {
        var uuid1 = Uuid7.NewUuid4();
        var guid = uuid1.ToGuid();
        var uuid2 = new Uuid7(guid);
        Assert.AreEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid4_TestUniqueness() {
        var set = new HashSet<Uuid7>();

        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 1000) {
            if (!set.Add(Uuid7.NewUuid4())) {
                throw new InvalidOperationException("Duplicate UUIDv4 generated.");
            }
        }
    }

    [TestMethod]
    public void Uuid4_TestThreadedUniqueness() {
        var set = new ConcurrentDictionary<Uuid7, nint>();

        Parallel.For(0, Math.Max(1, Environment.ProcessorCount / 2), (i) => {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 1000) {
                if (!set.TryAdd(Uuid7.NewUuid4(), 0)) {
                    throw new InvalidOperationException("Duplicate UUIDv4 generated.");
                }
            }
        });
    }

    [TestMethod]
    public void Uuid4_Fill() {
        var uuids = new Uuid7[10000];
        Uuid7.FillUuid4(uuids);

        foreach (var uuid in uuids) {
            Assert.AreNotEqual(Uuid7.Empty, uuid);
        }
    }

}
