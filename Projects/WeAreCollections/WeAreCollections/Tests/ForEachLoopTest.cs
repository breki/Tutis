using System;
using System.Collections.Generic;

namespace WeAreCollections.Tests
{
    public class ForEachLoopTest : IPerformanceTest
    {
        public const int ListSize = 100000000;

        public int SuggestedTestStepsCount
        {
            get { return 10; }
        }

        public int Sum
        {
            get { return sum; }
        }

        public string GetTestDescription(int expectedTestStepsCount)
        {
            return "Runs foreach loop on a List<>";
        }

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            list = new List<int>(ListSize);

            for (int i = 0; i < ListSize; i++)
                list.Add(rnd.Next());
        }

        public void ExecuteStep(int step)
        {
            sum = 0;
            foreach (int item in list)
                sum += item;
        }

        public void AssertValidity()
        {
        }

        private List<int> list;
        private int sum;
    }
}