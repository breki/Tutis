using System;
using System.Collections.Generic;

namespace HomeSpaceOrganizer
{
    public interface ISimulatedAnnealingProblem<TSituation, in TSolution>
        where TSituation : ISimulatedAnnealingSituation
        where TSolution : class, ISimulatedAnnealingSolution
    {
        string ProblemDescription { get; }
        bool IsProblemValid { get; }

        int TransitionsPerTemperature { get; }
        TimeSpan MaxRunningTime { get; }
        double CoolingFraction { get; }

        /// <summary>
        /// Generates an initial situation (solution) and returns it.
        /// </summary>
        /// <param name="randomizer"></param>
        /// <returns>An initial situation (solution).</returns>
        TSituation GenerateInitialSituation(IRandomizer randomizer);

        /// <summary>
        /// Generates a random solution transition on the current situation.
        /// </summary>
        /// <param name="currentSituation"></param>
        /// <param name="randomizer"></param>
        /// <returns>
        /// <c>true</c> if a transition was found; 
        /// <c>false</c> if there are no other possible transitions and the search should end.
        /// </returns>
        bool GenerateRandomTransition(TSituation currentSituation, IRandomizer randomizer);

        bool StopEvaluationFunction(
            TSituation currentSituation, 
            TSolution bestSolutionSoFar, 
            double improvementSinceLastStep, 
            int coolingStepsExecuted, 
            int stepsWithoutImprovementCount, 
            out string stopConditionDescription);

        void OnNewSituationAccepted();
        void OnBetterSolutionFound(TSolution betterSolution);
        void AfterSolutionFound();

        void FillLogValues(IList<object> logValues, TSolution solution);
    }
}