using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeAreCollections.Tests
{
    public class SortedListAddingUniqueTest : IPerformanceTest
    {
        public int SuggestedTestStepsCount
        {
            get { return 200000; }
        }

        public string GetTestDescription (int expectedTestStepsCount)
        {
            return string.Format (CultureInfo.InvariantCulture, "SortedList: adding {0:n0} unique elements", expectedTestStepsCount);
        }

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            this.expectedTestStepsCount = expectedTestStepsCount;
            testedSet = new SortedList<int, int>();

            valuesToUse = TestHelper.GenerateRandomUniqueTestSet(rnd, expectedTestStepsCount);
        }

        public void ExecuteStep (int step)
        {
            int value = valuesToUse[step];

            // tested code
            testedSet.Add (value, value);
        }

        public void AssertValidity()
        {
            if (testedSet.Count != expectedTestStepsCount)
                throw new InvalidOperationException ("testedSet.Count != expectedTestStepsCount");
        }

        private SortedList<int, int> testedSet;
        private int[] valuesToUse;
        private int expectedTestStepsCount;
    }
}