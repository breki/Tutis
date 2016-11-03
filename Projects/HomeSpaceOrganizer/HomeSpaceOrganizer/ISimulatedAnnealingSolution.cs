using System;

namespace HomeSpaceOrganizer
{
    /// <summary>
    /// Represents a possible solution of a simulated annealing problem defined by
    /// <see cref="SimulatedAnnealingWithSolutionUndo{TSituation,TSolution}"/>.
    /// </summary>
    public interface ISimulatedAnnealingSolution
    {
        double SolutionValue { get; }
        TimeSpan RunningTime { get; }
        bool RunOutOfTime { get; }

        void AcceptAsFinalSolution(TimeSpan runningTime, bool runOutOfTime);
    }
}