using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace CpuUsage
{
    class Program
    {
        [DllImport ("kernel32.dll")]
        static extern bool GetProcessTimes (IntPtr hProcess,
            out FILETIME lpCreationTime,
            out FILETIME lpExitTime,
            out FILETIME lpKernelTime,
            out FILETIME lpUserTime);

        static void Main (string[] args)
        {
            const int Interval = 2000;
            Dictionary<int, ProcessUsageMeasurement> processTimes = new Dictionary<int, ProcessUsageMeasurement> ();

            while (true)
            {
                foreach (Process process in Process.GetProcesses ())
                {
                    //FILETIME lpCreationTime;
                    //FILETIME lpExitTime;
                    //FILETIME lpKernelTime;
                    //FILETIME lpUserTime;
                    //bool result = GetProcessTimes (process.Handle, out lpCreationTime, out lpExitTime, out lpKernelTime, out lpUserTime);
                    //Console.Out.WriteLine("{0} - {1} {2}", process.ProcessName, lpKernelTime, lpUserTime);       

                    try
                    {
                        int processId = process.Id;
                        DateTime measurementTime = DateTime.Now;
                        TimeSpan currentProcessorTime = process.TotalProcessorTime;
                        double currentUsage = 0;

                        if (processTimes.ContainsKey(processId))
                        {
                            ProcessUsageMeasurement previousMeasurement = processTimes[processId];
                            currentUsage = (currentProcessorTime.TotalMilliseconds - previousMeasurement.ProcessorTime.TotalMilliseconds)
                                / (measurementTime - previousMeasurement.MeasurementTime).TotalMilliseconds 
                                / Environment.ProcessorCount * 100;
                        }
                        
                        if (currentUsage > 0)
                            Console.Out.WriteLine ("{0} - {1:N0}%", process.ProcessName, currentUsage);

                        processTimes[processId] = new ProcessUsageMeasurement () 
                            { MeasurementTime = measurementTime, ProcessorTime = currentProcessorTime };
                    }
                    catch (Win32Exception)
                    {
                    }
                    catch(InvalidOperationException)
                    {
                    }
                }

                Console.Out.WriteLine();
                Thread.Sleep(Interval);
            }
        }
    }
}
