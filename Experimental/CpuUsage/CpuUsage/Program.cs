using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace CpuUsage
{
    class Program
    {
        static void Main (string[] args)
        {
            const int Interval = 2000;
            Dictionary<int, ProcessUsageMeasurement> processTimes = new Dictionary<int, ProcessUsageMeasurement> ();

            while (true)
            {
                foreach (Process process in Process.GetProcesses ())
                {
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
