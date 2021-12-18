using CommandLine;

namespace Cortisol
{
    public static class StressForWindows
    {
        /// <summary>
        /// Get input args, deal with them, run the program.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(Options.HandleParseError);

            var parsed = (parserResult as Parsed<Options>)?.Value;
            var options = Options.ParseSuccess(parsed);

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

                if (options.Memory > 0)
                {
                    var task = new Task(() => Tests.Memory.RamHog(options.Memory));
                    task.Start();
                    tasks.Add(task);
                }
                
                for (var i = 0; i < options.Threads * 2; i++)
                {
                    Task task;
                    if (options.Prime)
                    {
                        task = new Task(Tests.Cpu.PrimeKill, token);
                        task.Start();
                    }
                    else
                    {
                        task = new Task(() => Tests.Cpu.CpuKill(options.Usage, options.Time), token);
                        task.Start();

                    }

                    tasks.Add(task);
                }

                // After completion, cancel the tasks.
                if (options.Temps)
                {
                    //Console.Write("TEMPS!");
                    Monitoring.CpuTemp(options.Time);
                }

                if (options.Time != 0)
                {
                    Thread.Sleep(options.Time);
                    tokenSource.Cancel();
                    return true; 
                }
                if (options.Time == 0) Console.WriteLine("Test will run until closed. Press 'q' / Ctrl + C to exit.");
                while (Console.ReadKey().Key != ConsoleKey.Q )
                {
                }
                tokenSource.Cancel();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception! Something went wrong while running the test. Exception: \n {e}");
                return false;
            }
            
        }





    }


}

