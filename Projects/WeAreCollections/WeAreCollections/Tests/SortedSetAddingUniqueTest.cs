﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace WeAreCollections.Tests
{
    public class SortedSetAddingUniqueTest : IPerformanceTest
    {
        public int SuggestedTestStepsCount
        {
            get { return 2000000; }
        }

        public string GetTestDescription(int expectedTestStepsCount)
        {
            return string.Format (CultureInfo.InvariantCulture, "SortedSet: adding {0:n0} unique elements", expectedTestStepsCount);
        }

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            this.expectedTestStepsCount = expectedTestStepsCount;
            testedSet = new SortedSet<int> ();

            valuesToUse = TestHelper.GenerateRandomUniqueTestSet(rnd, expectedTestStepsCount);
        }

        public void ExecuteStep (int step)
        {
            int value = valuesToUse[step];

            // tested code
            testedSet.Add (value);
        }

        public void AssertValidity()
        {
            if (testedSet.Count != expectedTestStepsCount)
                throw new InvalidOperationException ("testedSet.Count != expectedTestStepsCount");
        }

        private SortedSet<int> testedSet;
        private int[] valuesToUse;
        private int expectedTestStepsCount;
    }
}