using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;


namespace Cortisol.Tests;

public static class Memory
{

    public static void RamHog(int amount)
    {
        Parallel.For(0, 1, i =>
        {
            long runningTotal = GC.GetTotalMemory(false);
            long endingMemoryLimit =1024 * 1000 * 100;
            List<XmlNode> memList = new List<XmlNode>();
           // Marshal.AllocHGlobal(amount * 1000 * 1000);
            
            while (runningTotal <= endingMemoryLimit)
            {
                Marshal.AllocHGlobal(8192);
                runningTotal = GC.GetTotalMemory(false);
                Console.WriteLine("Memory Usage:" + Convert.ToString(GC.GetTotalMemory(false)));
                Console.WriteLine($"Target usage {endingMemoryLimit}");
               // Thread.Sleep(10);
            }
            
            
        });
    }

}