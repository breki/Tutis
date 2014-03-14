using System;

namespace WeAreCollections
{
    public interface IPerformanceTest
    {
        string TestDescription { get; }
        int SuggestedTestStepsCount { get; }

        void Initialize(Random rnd, int expectedTestStepsCount);
        void ExecuteStep(int step);
        void AssertValidity();
    }
}