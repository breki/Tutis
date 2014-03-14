using System;
using System.Collections.Generic;
using System.Linq;

namespace WeAreCollections.Tests
{
    public class SortedSetAddingUniqueTest : IPerformanceTest
    {
        public string TestDescription
        {
            get { return "SortedSet: adding a lot of unique elements"; }
        }

        public int SuggestedTestStepsCount
        {
            get { return 10000000; }
        }

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            this.rnd = rnd;
            this.expectedTestStepsCount = expectedTestStepsCount;
            testedSet = new SortedSet<int> ();

            IEnumerable<int> orderedValues = Enumerable.Range (0, expectedTestStepsCount);
            valuesToUse = orderedValues.OrderBy (a => rnd.Next()).ToArray();
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

        private Random rnd;
        private SortedSet<int> testedSet;
        private int[] valuesToUse;
        private int expectedTestStepsCount;
    }
}