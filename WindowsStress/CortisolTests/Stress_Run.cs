using NUnit.Framework;

namespace Cortisol.Tests;

[TestFixture]
public class Run_RunSuccess
{
    private Options options;
    [SetUp]
    public  void SetUp()
    { 
        options = new Options
        {
            Usage = 100,
            Time = 1000,
            Threads = 1
        };
    }
    
    [Test]
    public void PrimeProgramSuccess()
    {
        options.Prime = true;
        Assert.IsTrue(StressForWindows.Run(options));
    }

    [Test]
    public void WatchProgramSuccess()
    {
        options.Prime = false;
        Assert.IsTrue(StressForWindows.Run(options));
    }
    
    
}