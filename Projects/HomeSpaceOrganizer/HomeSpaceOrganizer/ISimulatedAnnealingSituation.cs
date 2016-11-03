namespace HomeSpaceOrganizer
{
    public interface ISimulatedAnnealingSituation
    {
        /// <summary>
        /// Indicates whether the solution is optimal, i.e. no better solution can be found. 
        /// </summary>
        /// <remarks>This property is used by the annealing algorithm to stop further processing
        /// if the optimal solution has been found. It is only useful for (rare) problems 
        /// for which the optimal solution
        /// can be determined when it is found. In all other cases the property should return
        /// <c>false</c>.</remarks>
        bool IsOptimal { get; }
        double SituationValue { get; }

        void AcceptChange();
        ISimulatedAnnealingSolution GenerateSolutionCandidate();
        void RejectChange();
    }
}