using System;

namespace TejkABrejk
{
    public interface IExerciseAlarm
    {
        event EventHandler ExerciseFinished;

        void ShowAlarm();
    }
}