using System.Diagnostics;
using System.Numerics;

namespace Cortisol;

public static class Tests
{
    /// <summary>
    /// Pin the CPU at a requested usage using Stopwatches.
    /// </summary>
    /// <param name="cpuUsage">Requested CPU usage</param>
    /// <param name="duration">Duration of test</param>
    public static void CpuKill(int cpuUsage, int duration)
    {
        // I didn't write this, so I'm not 100% on it.
        // While running, create a new watch, sleep x seconds to lower the CPU usage amount, reset the watch. 
        // Doing this fast enough and in parallel causes very high CPU usage.
        Parallel.For(0, 1, i =>
        {
            var time = new Stopwatch();
            var watch = new Stopwatch();
            watch.Start();
            time.Start();
            while (time.ElapsedMilliseconds < duration)
            {
                if (watch.ElapsedMilliseconds <= cpuUsage) continue;
                Thread.Sleep(100 - cpuUsage);
                watch.Reset();
                watch.Start();
            }
        });
    }

    /// <summary>
    /// Stress the CPU by finding Mersenne Primes.
    /// </summary>
    public static void PrimeKill()
    {
        // While running, check if the given exponent of 2 is a Mersenne Prime, increasing the exp on each loop.
        Parallel.For(0, 1, i =>
        {
            var pow = 2;
            while (true)
            {
                if (Math.IsPrime(pow))
                {
                    BigInteger p = BigInteger.Pow(2, pow) - 1;
                    if (Math.IsPrime(p))
                    {
                      //  Console.WriteLine($"{p} is prime!");
                    }
                }

                pow++;
            }
        });
    }
}