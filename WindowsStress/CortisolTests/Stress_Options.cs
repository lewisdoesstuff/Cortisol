using System;
using NUnit.Framework;


namespace Cortisol.Tests;

[TestFixture]
public class Options_HandleOptions
{
    private Options options;
    
    [SetUp]
    public void SetUp()
    {
        options = new Options();
    }
    
    [Test]
    public void ReturnIsSameWhenPassed()
    {
        options.Usage = 100;
        options.Threads = 4;
        options.Time = 10;
        options.Prime = true;

        var ret = Options.GetOptions(options);
        options.Time *= 1000; // GetOptions does time * 1000 to convert to ms.
        Console.WriteLine(ret);
        //Assert.Warn($"orig: {options.Usage} {options.Threads} {options.Time} {options.Prime}. Ret: {ret.Usage} {ret.Threads} {ret.Time} {ret.Prime} {options == ret}");
        
        Assert.AreEqual(options.Usage,ret.Usage);
        Assert.AreEqual(options.Threads,ret.Threads);
        Assert.AreEqual(options.Time,ret.Time);
        Assert.AreEqual(options.Prime,ret.Prime);
    }
    
    [Test]
    public void PrimeOffWhenBelowMax()
    {
        options.Usage = 99;
        options.Threads = 1;
        options.Time = 1;

        var ret = Options.GetOptions(options);
        
        Assert.AreEqual(false,ret.Prime);
    }

    [Test]
    public void PrimeOnWhenMax()
    {
        options.Usage = 100;
        options.Threads = 1;
        options.Time = 1;

        var ret = Options.GetOptions(options);
        
        Assert.AreEqual(true,ret.Prime);
    }
    
}