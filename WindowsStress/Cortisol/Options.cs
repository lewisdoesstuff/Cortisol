using System.Collections;
using CommandLine;

///<summary>
/// CommandLineParser's Options class. Handles passed arguments UNIX style.
/// </summary>
public class Options : IEquatable<Options>
{
        public Options(int usage, int threads, int time, bool temps, int memory)
        {
            Usage = usage;
            Threads = threads;
            Time = time;
            Temps = temps;
            Memory = memory;

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
        
        [Option('m',"memory", Required = false, HelpText = "Consume the given (MB) amount of RAM")]
        public int Memory { get; set; }
        public bool Prime { get; set; }
        

        public static Options ParseSuccess(Options options)
        {
            
            var output = new Options
            {
                Memory = options.Memory,
                Temps = options.Temps,
                Threads = options.Threads,
                Time = options.Time * 1000,
                Usage = options.Usage
            };

            // if options are default (clp sets them all to 0/false), get options from user.
            if (options.Equals(new Options()))
            {
                output = GetOptions();
            }


            return output;
        }

        public static void HandleParseError(IEnumerable<Error> errors)
        {
            if (errors.IsHelp() || errors.IsVersion()) Environment.Exit(0);
            
            Console.WriteLine("CommandLineParser reported errors with the passed arguments! \nErrors:");
            var errList = errors.ToList();
            foreach (var error in errList)
            {
                Console.WriteLine(error);
            }
            Environment.Exit(2);
        }

        public static Options GetOptions()
        {
            var options = new Options();
            bool retry;
            do
            {
                Console.WriteLine("Enter CPU usage amount (1-100)");
                {
                    retry = false;
                    var input = Console.ReadLine();
                    var usage = Convert.ToInt32(input);
                    if (usage > 0 &&
                        usage <= 100)
                    {
                        options.Usage = usage;
                    }
                    else
                    {
                        retry = true;
                    }
                }
            } while (retry);

            do
            {
                Console.WriteLine($"Enter thread count (1-{Environment.ProcessorCount})");
                {
                    retry = false;
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out var threads) &&
                        threads > 0 &&
                        threads <= Environment.ProcessorCount)
                    {
                        options.Threads = threads;
                    }
                    else retry = true;
                }
            } while (retry);

            do
            {
                //TODO: allow unlimited time
                Console.WriteLine("Enter test duration (s)");
                {
                    retry = false;
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out var time))
                    {
                        options.Threads = time * 1000;
                    }
                    else retry = true;
                }
            } while (retry);
            
            do
            {
                Console.WriteLine("Display temperatures? Y/N:");
                {
                    retry = false;
                    var input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input) &&
                        input.ToLower() == "y")
                    {
                        options.Temps = true;
                    }
                    else if (!string.IsNullOrWhiteSpace(input) &&
                             input.ToLower() == "n")
                    {

                        options.Temps = false;
                    }
                    else retry = true;
                }
            } while (retry);
            return options;


        }

        public bool Equals(Options? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Usage == other.Usage && Threads == other.Threads && Time == other.Time && Memory == other.Memory; // this doesn't actually check all of the values, we ignore prime and temps for this.
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Options) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Usage, Threads, Time, Temps, Memory, Prime);
        }
}