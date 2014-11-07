using System;
using System.Collections.Generic;

namespace WeAreCollections.Tests
{
    public class ForLoopWithCachedCountTest : IPerformanceTest
    {
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
            return "Runs for loop on a List<> with cached list size";
        }

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            list = new List<int> (ForEachLoopTest.ListSize);

            for (int i = 0; i < ForEachLoopTest.ListSize; i++)
                list.Add(rnd.Next());
        }

        public void ExecuteStep(int step)
        {
            sum = 0;
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                int item = list[i];
                sum += item;
            }
        }

        public void AssertValidity()
        {
        }

        private List<int> list;
        private int sum;
    }
}