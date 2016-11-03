using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeSpaceOrganizer
{
    public class OrganizingProblem : ISimulatedAnnealingProblem<PlacementSolution, PlacementSolution>
    {
        public OrganizingProblem(IEnumerable<Place> places, IEnumerable<Stuff> stuffs)
        {
            this.places = places.AsQueryable().ToDictionary(x => x.Name);
            this.stuffs = stuffs.AsQueryable().ToDictionary(x => x.Name);
        }

        public string ProblemDescription { get; }

        public bool IsProblemValid { get { return true; } }

        public int TransitionsPerTemperature { get; }

        public TimeSpan MaxRunningTime { get; }

        public double CoolingFraction { get; }

        public PlacementSolution GenerateInitialSituation(IRandomizer randomizer)
        {
            return PlacementSolution.Initial(places.Values, stuffs.Values);
        }

        public bool GenerateRandomTransition(PlacementSolution currentSituation, IRandomizer randomizer)
        {
            throw new NotImplementedException();
        }

        public bool StopEvaluationFunction(PlacementSolution currentSituation, PlacementSolution bestSolutionSoFar, double improvementSinceLastStep, int coolingStepsExecuted, int stepsWithoutImprovementCount, out string stopConditionDescription)
        {
            throw new NotImplementedException();
        }

        public void OnNewSituationAccepted()
        {
            throw new NotImplementedException();
        }

        public void OnBetterSolutionFound(PlacementSolution betterSolution)
        {
            throw new NotImplementedException();
        }

        public void AfterSolutionFound()
        {
            throw new NotImplementedException();
        }

        public void FillLogValues(IList<object> logValues, PlacementSolution solution)
        {
            throw new NotImplementedException();
        }

        private readonly Dictionary<string, Place> places;
        private readonly Dictionary<string, Stuff> stuffs;
    }
}