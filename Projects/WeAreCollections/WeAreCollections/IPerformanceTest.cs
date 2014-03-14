using System;

namespace WeAreCollections
{
    public interface IPerformanceTest
    {
        int SuggestedTestStepsCount { get; }

        string GetTestDescription(int expectedTestStepsCount);
        void Initialize(Random rnd, int expectedTestStepsCount);
        void ExecuteStep(int step);
        void AssertValidity();
    }
}