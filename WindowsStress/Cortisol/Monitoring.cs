using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;
using LibreHardwareMonitor.Hardware;


namespace Cortisol
{


    public static class Monitoring
    {

        private class UpdateVisitor : IVisitor // this is boilerplate OpenHardwareMonitor code
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }

            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }

            public void VisitSensor(ISensor sensor)
            {
            }

            public void VisitParameter(IParameter parameter)
            {
            }
        }

        private static float GetTemp()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.IsCpuEnabled = true;
            computer.Accept(updateVisitor);
            foreach (var t in computer.Hardware)
            {
                if (t.HardwareType != HardwareType.Cpu) continue;
                foreach (var t1 in t.Sensors)
                {
                    if (t1.SensorType == SensorType.Temperature)
                        return (float) t1.Value; // return string value as opposed to printing it to the console.
                }
            }

            computer.Close();
            return 0; // this shouldn't happen.
        }

        public static void CpuTemp(int time)
        {
            Console.WriteLine("Called");
            float[] cpu = new float[time / 1000];
            for (int i = 0; i <= time / 1000; i++)
            {
                cpu[i] = (float) GetTemp();
                Functions.WriteTemps(cpu[i]);
                Console.WriteLine(cpu[i]);

                Thread.Sleep(1000);

            }

        }
    }
}

    
    
    
