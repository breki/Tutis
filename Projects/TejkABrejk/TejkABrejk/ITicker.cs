using System;

namespace TejkABrejk
{
    public interface ITicker
    {
        TimeSpan ActiveTime { get; }

        void Tick (DateTime time, bool isIdle);
    }

    public class Ticker : ITicker
    {
        public Ticker(
            TabConfiguration configuration, 
            IExerciseAlarm exerciseAlarm)
        {
            this.configuration = configuration;
            this.exerciseAlarm = exerciseAlarm;

            exerciseAlarm.ExerciseFinished += OnExerciseFinished;
        }

        public TimeSpan ActiveTime
        {
            get { return TimeSpan.FromSeconds(activeTimeInSeconds); }
        }

        public void Tick(DateTime time, bool isIdle)
        {
            if (exercising)
                return;

            ResetIfNewDay(time);

            if (!isIdle)
                activeTimeInSeconds += configuration.TickPeriod.TotalSeconds;

            lastTick = time;

            if (activeTimeInSeconds >= configuration.MaxPeriodBetweenExercises.TotalSeconds)
            {
                exercising = true;
                exerciseAlarm.ShowAlarm();
                activeTimeInSeconds = 0;
            }
        }

        private void ResetIfNewDay(DateTime time)
        {
            if (lastTick.HasValue)
            {
                if (lastTick.Value.Date == time.Date)
                {
                    if (lastTick.Value.TimeOfDay < configuration.NewDayTime
                        && time.TimeOfDay >= configuration.NewDayTime)
                    {
                        activeTimeInSeconds = 0;
                    }
                }
                else
                {
                    if (time.TimeOfDay >= configuration.NewDayTime)
                        activeTimeInSeconds = 0;
                }
            }
        }

        private void OnExerciseFinished (object sender, EventArgs e)
        {
            exercising = false;
        }

        private readonly TabConfiguration configuration;
        private readonly IExerciseAlarm exerciseAlarm;
        private double activeTimeInSeconds;
        private DateTime? lastTick;
        private bool exercising;
    }
}