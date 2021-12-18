using System.IO;
using Cortisol.Stress;
using NUnit.Framework;

namespace Cortisol.Tests;

public class Disks
{
    [Test]
    public static void CreateTempFile()
    {
        Temp.Create(1024);

        if (File.Exists(Path.GetTempPath() + "/cortisol.tmp"))
        {
            File.Delete(Path.GetTempPath() + "/cortisol.tmp");
            Assert.Pass();
        }
        Assert.Fail();
    }
    [Test]
    public static void CreateTempFileExists()
    {
        File.Create(Path.GetTempPath() + "/cortisol.tmp");
        Temp.Create(1024);

        if (File.Exists(Path.GetTempPath() + "/cortisol.tmp"))
        {
            File.Delete(Path.GetTempPath() + "/cortisol.tmp");
            Assert.Pass();
        }
        Assert.Fail();
    }
    [Test]
    public static void DeleteTempFile()
    {
        if (!File.Exists(Path.GetTempPath() + "/cortisol.tmp"))
        {
            File.Create(Path.GetTempPath() + "/cortisol.tmp");
        }
        Temp.Delete();

        if (File.Exists(Path.GetTempPath() + "/cortisol.tmp"))
        {
            File.Delete(Path.GetTempPath() + "/cortisol.tmp");
            Assert.Fail();
        }
        Assert.Pass();
    }
    [Test]
    public static void DeleteTempFileNotExist()
    {
        if (!File.Exists(Path.GetTempPath() + "/cortisol.tmp"))
        {
            File.Delete(Path.GetTempPath() + "/cortisol.tmp");
        }
        Temp.Delete();

        if (File.Exists(Path.GetTempPath() + "/cortisol.tmp"))
        {
            File.Delete(Path.GetTempPath() + "/cortisol.tmp");
            Assert.Fail();
        }
        Assert.Pass();
    }
}