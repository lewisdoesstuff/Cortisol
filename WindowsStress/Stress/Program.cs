using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace Stress
{
    /**
     * The aim of this program is to load the cpu to a specific load. It is similar in that regard to the Linux tool Stress
     */

    public class Options
    {
        public Options(int usage, int threads, int time)
        {
            this.Usage = usage;
            this.Threads = threads;
            this.Time = time;

        }
        public Options(){}
        [Option('u', "usage", Required = false, 
            HelpText = "Set the desired CPU usage% for the test. Does not apply to the Mersenne Prime test.")]

        public int Usage { get; set; }

        [Option('t', "threads", Required = false,
            HelpText =
                "Set the desired amount of threads for the test to run on. 0 will run on all available threads.")]
        public int Threads { get; set; }

        [Option('d', "time", Required = false,
            HelpText = "Set the desired time for the test to run. 0 will run until cancelled.")]
        public int Time { get; set; }

        [Option('p', "prime", Required = false,
            HelpText = "Run the Mersenne Prime CPU stress test. Generates more heat.")]
        public bool Prime { get; set; }
    }

    class StressForWindows
    {
        /**
         * The parameters passed should be CPU Usage, Core count and finally duration
         */
        static void Main(string[] args)
        {
            int cpuUsage = 100;
            int targetThreadCount = 0;
            int duration = 0;
            bool prime = false;

            var parser = new Parser(with => with.EnableDashDash = true);
            var result = parser.ParseArguments<Options>(args);
            result.WithParsed(o =>
            {
                Console.WriteLine(o.Threads);
                Console.WriteLine(o.Time);
                Console.WriteLine(o.Usage);
                prime = o.Prime;
                if (o.Usage != 100 && o.Usage != 0)
                    cpuUsage = o.Usage;
                else if (o.Usage == 0 && !o.Prime)
                {
                    Console.WriteLine("Please enter CPU usage amount (1-100)");
                    var res = Console.ReadLine();
                    if (int.TryParse(res, out _))
                    {
                        cpuUsage = Convert.ToInt32(res);
                    }
                }

                if (o.Threads != Environment.ProcessorCount && o.Threads != 0)
                    targetThreadCount = o.Threads;
                else if (o.Threads == 0)
                {
                    Console.WriteLine($"Please enter Thread Count (1-{Environment.ProcessorCount}");
                    var res = Console.ReadLine();
                    if (int.TryParse(res, out _) && Convert.ToInt32(res) <= Environment.ProcessorCount)
                    {
                        targetThreadCount = Convert.ToInt32(res);
                    }
                }

                if (o.Time > 0)
                    duration = o.Time;
                else if (o.Time == 0)
                {
                    Console.WriteLine($"Please enter Test Duration (s). Enter 0 for unlimited time.");
                    var res = Console.ReadLine();
                    if (int.TryParse(res, out _))
                    {
                        duration = Convert.ToInt32(res) * 1000;
                    }
                }

                Console.WriteLine(o.Usage);

            })
                .WithNotParsed(o => o.ToList().ForEach(l => Console.WriteLine(l)));
            /*
             * The code here is a modifed (to be parameterizable) version of the code at: 
             * http://stackoverflow.com/questions/2514544/simulate-steady-cpu-load-and-spikes
             * another interesting article covering this topic is:
             * http://stackoverflow.com/questions/5577098/how-to-run-cpu-at-a-given-load-cpu-utilization
             */


            Console.WriteLine(
                $"Parameters: CPU: {cpuUsage},Thread Count: {targetThreadCount}, Duration: {duration / 1000}");
            List<Thread> threads = new List<Thread>();
            List<Task> tasks = new List<Task>();
            //Ensure the current process takes presendence thus (hopefully) holidng the utilisation steady
            Process Proc = Process.GetCurrentProcess();
            Proc.PriorityClass = ProcessPriorityClass.RealTime;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            long AffinityMask = (long)Proc.ProcessorAffinity;
            for (int i = 0; i < targetThreadCount; i++)
            {
                Thread t;
                Task task;
                if (prime)
                {
                    // t = new Thread(PrimeKill);
                    //  t.Start();
                    task = new Task(() => PrimeKill(token), token);
                    task.Start();
                }
                else
                {
                    // t = new Thread(CpuKill);
                    // t.Start(cpuUsage, duration);
                    task = new Task(() => CpuKill(cpuUsage, duration, token), token);
                    task.Start();

                }

                tasks.Add(task);
            }

            Thread.Sleep(duration);
            foreach (var t in tasks)
            {
                tokenSource.Cancel();
            }
        }

        public static void CpuKill(int cpuUsage, int duration, CancellationToken token)
        {
            Parallel.For(0, 1, i =>
            {
                Stopwatch time = new Stopwatch();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                time.Start();
                while (time.ElapsedMilliseconds < duration)
                {
                    if (watch.ElapsedMilliseconds > (int)cpuUsage)
                    {
                        token.ThrowIfCancellationRequested();
                        Thread.Sleep(100 - (int)cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            });
        }

        public static void PrimeKill(CancellationToken token)
        {
            Parallel.For(0, 1, i =>
            {
                long power = 2000000;
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    power += 1;
                    Console.WriteLine($"n={power}");
                    if (IsMersenne(power -1))
                    {
                        Console.WriteLine($"{power} is prime!");
                    }
                }
            });
        }

        public static bool IsMersenne(BigInteger num)
        {
            if (num == 2) return true;

            long sqrt = (long)num.Sqrt();
            for (long i = 3; i < sqrt; i += 2)
                if (num % i == 2)
                    return false;
            return true;

        }

    }

    public static class Extension
    {
        public static BigInteger Sqrt(this BigInteger n)
        {
            if (n == 0) return 0;
            if (n <= 0) throw new ArithmeticException("NaN");
            int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
            BigInteger root = BigInteger.One << (bitLength / 2);

            while (!IsSqrt(n, root))
            {
                root += n / root;
                root /= 2;
            }

            return root;

        }

        private static Boolean IsSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }
    }
}

