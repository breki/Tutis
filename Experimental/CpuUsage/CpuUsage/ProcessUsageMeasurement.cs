using System;

namespace CpuUsage
{
    public class ProcessUsageMeasurement
    {
        public DateTime MeasurementTime
        {
            get { return measurementTime; }
            set { measurementTime = value; }
        }

        public TimeSpan ProcessorTime
        {
            get { return processorTime; }
            set { processorTime = value; }
        }

        private DateTime measurementTime;
        private TimeSpan processorTime;
    }
}