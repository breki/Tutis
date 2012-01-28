using System;

namespace TejkABrejk
{
    public class TabConfiguration
    {
        public TimeSpan NewDayTime
        {
            get { return newDayTime; }
            set { newDayTime = value; }
        }

        public TimeSpan TickPeriod
        {
            get { return tickPeriod; }
            set { tickPeriod = value; }
        }

        public TimeSpan MaxPeriodBetweenExercises
        {
            get { return maxPeriodBetweenExercises; }
            set { maxPeriodBetweenExercises = value; }
        }

        private TimeSpan newDayTime = TimeSpan.FromHours(5);
        private TimeSpan tickPeriod = TimeSpan.FromMinutes(1);
        private TimeSpan maxPeriodBetweenExercises = TimeSpan.FromMinutes(60);
    }
}