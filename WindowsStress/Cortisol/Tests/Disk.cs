namespace Cortisol.Stress;

public static class Temp
{
    private static string _path = Path.GetTempPath() + "/cortisol.tmp";
    public static void Create(int size)
    {
        
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }

        byte[] data = new byte[8192];
        Random rng = new Random();
        using (FileStream stream = File.OpenWrite(_path))
        {
            for (int i = 0; i < size * 128; i++)
            {
                rng.NextBytes(data);
                stream.Write(data, 0, data.Length);
            }
        }
    }

    public static void Delete()
    {
        if (!File.Exists(_path)) return;
        File.Delete(_path);

    }
}

public class Disk
{
    
}