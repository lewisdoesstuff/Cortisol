using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using CommandLine;

///<summary>
/// CommandLineParser's Options class. Handles passed arguments UNIX style.
/// </summary>
public class Options
    {
        public Options(int usage, int threads, int time, bool temps)
        {
            Usage = usage;
            Threads = threads;
            Time = time;
            Temps = temps;
        }

        public Options(){} // empty constructor for when no args passed
        
        [Option('u', "usage", Required = false, 
            HelpText = "Set the desired CPU usage% for the test. Does not apply to the Mersenne Prime test.")]

        public int Usage { get; set; }

        [Option('c', "threads", Required = false,
            HelpText =
                "Set the desired amount of threads for the test to run on. 0 will run on all available threads.")]
        public int Threads { get; set; }

        [Option('d', "time", Required = false,
            HelpText = "Set the desired time for the test to run. 0 will run until cancelled.")]
        public int Time { get; set; }

        [Option('t', "temp", Required = false, HelpText = "Display CPU temperature while test is running.")]
        public bool Temps { get; set; }
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

            if (options.Temps) output.Temps = true;
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