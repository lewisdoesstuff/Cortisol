using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace Cortisol.Tests;

public class Memory
{
    [TestCase(512)]
    [TestCase(1024)]
    public static void HogRam(int amount)
    {
        long startMem = GC.GetTotalMemory(false);
        long endMem = startMem + amount * 1000 * 1000;

        var task = Task.Run(() => Stress.Memory.RamHog(amount));
        task.Wait();
        Assert.AreEqual(GC.GetTotalMemory(false), endMem, 65536);
    }
}