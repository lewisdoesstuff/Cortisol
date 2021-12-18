namespace Cortisol.Stress;

public static class Memory
{

    public static void RamHog(int amount)
    {
        
        long runningTotal = GC.GetTotalMemory(false);
        long endingMemoryLimit = runningTotal + amount * 1000 * 1000;
        Console.WriteLine(amount);
        List<byte[]> hugeList = new List<byte[]>();
        while (runningTotal <= endingMemoryLimit)
        {
            byte[] bytes = new byte[100 * 100];
            hugeList.Add(bytes);
            runningTotal = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory Usage: {runningTotal}");
            Console.WriteLine($"Target usage {endingMemoryLimit}");

        }
    }
    

}