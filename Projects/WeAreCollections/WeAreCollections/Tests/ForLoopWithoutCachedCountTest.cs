using System;
using System.Collections.Generic;

namespace WeAreCollections.Tests
{
    public class ForLoopWithoutCachedCountTest : IPerformanceTest
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
            return "Runs for loop on a List<> without cached list size";
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
            for (int i = 0; i < list.Count; i++)
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