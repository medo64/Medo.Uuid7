using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
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

    [Ignore]
    [TestMethod]
    public void Uuid4_TestMany() {
        var sw = Stopwatch.StartNew();
        var i = 0;
        while (sw.ElapsedMilliseconds < 1000) {
            _ = Uuid7.NewUuid4();
            i++;
        }
        //Console.WriteLine($"Generated {i:#,##0} UUIDs in 1 second");
    }

}
