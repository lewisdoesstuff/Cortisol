namespace Cortisol;

public class Functions
{
    /// <summary>
    /// display temperatures with nice colors! 
    /// </summary>
    public static void WriteTemps(float cpu)
    {
        Console.Clear();
        
        Console.Write("CPU: ");
        // var cpu = Common.Truncate(temps.Key, 2);
        if (Convert.ToInt32(cpu) <= 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{cpu}C\n");
            Console.ResetColor();
        }
        else if (Convert.ToInt32(cpu) <= 70)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{cpu}C\n");
            Console.ResetColor();
        }
        else if (Convert.ToInt32(cpu) <= 100)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{cpu}C\n");
            Console.ResetColor();
        }
    }
    
}