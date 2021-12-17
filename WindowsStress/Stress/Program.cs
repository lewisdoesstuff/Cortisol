using System.Diagnostics;
using System.Numerics;
using CommandLine;

namespace Stress
{
    /**
     * The aim of this program is to load the cpu to a specific load. It is similar in that regard to the Linux tool Stress
     */
    
    ///<summary>
    /// CommandLineParser's Options class. Handles passed arguments UNIX style.
    /// </summary>
    public class Options
    {
        public Options(int usage, int threads, int time)
        {
            this.Usage = usage;
            this.Threads = threads;
            this.Time = time;
        }

        public Options(){} // empty constructor for when no args passed
        
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

        public bool Prime { get; set; }
        

        public static Options GetOptions(Options? options)
        {
            var output = new Options();

            if (options != null && options.Usage != 0)
            {
                output.Usage = options.Usage;
                output.Prime = output.Usage >= 100;
            }
            // Otherwise, get usage from stdin.
            else
            {
                Console.WriteLine("Please enter CPU usage amount (1-100)");
                var res = Console.ReadLine();
                if (int.TryParse(res, out _))
                {
                    output.Usage = Convert.ToInt32(res);
                    // If we don't want 100% usage, use the Stopwatch based test.
                    output.Prime = output.Usage >= 100;
                }
            }

            // If threads is changed from default, and within limits (cpu thread count), set to passed value.
            if (options != null && options.Threads != Environment.ProcessorCount && options.Threads != 0)
                output.Threads = options.Threads;
            // Otherwise, get threads from stdin.
            else
            {
                Console.WriteLine($"Please enter Thread Count (1-{Environment.ProcessorCount})");
                var res = Console.ReadLine();
                if (int.TryParse(res, out _) && Convert.ToInt32(res) <= Environment.ProcessorCount)
                {
                    output.Threads = Convert.ToInt32(res);
                }
            }

            // If time is changed from default, * 1000 (s => ms), set to passed value.
            if (options != null && options.Time > 0)
                output.Time = options.Time * 1000;
            // Otherwise, get from stdin (and * 1000)
            else
            {
                Console.WriteLine($"Please enter Test Duration (s). Enter 0 for unlimited time.");
                var res = Console.ReadLine();
                if (int.TryParse(res, out _))
                {
                    output.Time = Convert.ToInt32(res) * 1000;
                }
            }

            return output;
        }

        public static void HandleParseError(IEnumerable<Error> errors)
        {
            Console.WriteLine("CommandLineParser reported errors with the passed arguments! \nErrors:");
            var errList = errors.ToList();
            foreach (var error in errList)
            {
                Console.WriteLine(error);
            }
        }
        
    }

    
    public static class StressForWindows
    {

        public static void Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(o => Options.HandleParseError(o));

            var parsed = (parserResult as Parsed<Options>)?.Value;
            var options = Options.GetOptions(parsed);

            Run(options);
        }
        
        /// <summary>
        /// Main program body, handles argument parsing and running tests.
        /// </summary>
        /// <param name="options">Passed launch options.</param>
        public static bool Run(Options options)
        {
            // The code here is a modified (to be parameterizable) version of the code at: 
            // http://stackoverflow.com/questions/2514544/simulate-steady-cpu-load-and-spikes
            // another interesting article covering this topic is:
            // http://stackoverflow.com/questions/5577098/how-to-run-cpu-at-a-given-load-cpu-utilization
             
            // Print options to screen.
            Console.WriteLine($"Parameters: CPU: {options.Usage},Thread Count: {options.Threads}, Duration: {options.Time / 1000}");
            
            // Create new list of tasks, one/thread.
            var tasks = new List<Task>();
            
            // Proc.PriorityClass = ProcessPriorityClass.RealTime; -- this was a terrible idea.
            
            // Create CancellationToken and Source to kill the tasks. thread.Abort() was depreciated. (and apparently a terrible idea)
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            
            
            // Loop for amount of threads, creating a new task of the chosen stress type with the cancellation token, then add it to the list..
            try
            {
                for (var i = 0; i < options.Threads * 2; i++)
                {
                    Task task;
                    if (options.Prime)
                    {
                        task = new Task(PrimeKill, token);
                        task.Start();
                    }
                    else
                    {
                        task = new Task(() => CpuKill(options.Usage, options.Time), token);
                        task.Start();

                    }

                    tasks.Add(task);
                }

                // After completion, cancel the tasks.
                Thread.Sleep(options.Time);
                tasks.ForEach(_ => tokenSource.Cancel());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception! Something went wrong while running the test. Exception: \n {e}");
                return false;
            }
            
        }

        /// <summary>
        /// Pin the CPU at a requested usage using Stopwatches.
        /// </summary>
        /// <param name="cpuUsage">Requested CPU usage</param>
        /// <param name="duration">Duration of test</param>
        private static void CpuKill(int cpuUsage, int duration)
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
        private static void PrimeKill()
        {
            // While running, check if the given exponent of 2 is a Mersenne Prime, increasing the exp on each loop.
            Parallel.For(0, 1, i =>
            {
                var pow = 2;
                while (true) {
                    if (IsPrime(pow)) {
                        BigInteger p = BigInteger.Pow(2, pow) - 1;
                        if (IsPrime(p)) {
                            Console.WriteLine($"{p} is prime!");
                        }
                    }
                    pow++;
                }
            });
        }

        /// <summary>
        /// Check if a given number is a Mersenne Prime.
        /// https://rosettacode.org/wiki/Mersenne_primes#C.23
        /// </summary>
        /// <param name="num">Number to test.</param>
        /// <returns></returns>
        public static bool IsPrime(BigInteger num) {
            // Compare against short list.
            if (num < 2) return false;
            if (num % 2 == 0) return num == 2;
            if (num % 3 == 0) return num == 3;
            if (num % 5 == 0) return num == 5;
            if (num % 7 == 0) return num == 7;
            if (num % 11 == 0) return num == 11;
            if (num % 13 == 0) return num == 13;
            if (num % 17 == 0) return num == 17;
            if (num % 19 == 0) return num == 19;
 
            // We use BigIntegers as the numbers get very big very fast.
            BigInteger limit = num.Sqrt();
            BigInteger test = 23;
            
            while (test < limit) {
                if (num % test == 0) return false;
                test += 2;
                if (num % test == 0) return false;
                test += 4;
            }
 
            return true;
        }

    }

    public static class Extension
    { 
        /// <summary>
        /// Numerics.BigInteger Square Root extension method.
        /// </summary>
        /// <param name="x">Input BigInteger</param>
        /// <returns>Square Root of x</returns>
        /// <exception cref="ArgumentException">Negative numbers are not permitted.</exception>
        public static BigInteger Sqrt (this BigInteger x) {
            if (x < 0) throw new ArgumentException("Negative argument.");
            if (x < 2) return x;
            BigInteger y = x / 2;
            while (y > x / y) {
                y = ((x / y) + y) / 2;
            }
            return y;
        }
    }
}

