using System;
using System.Globalization;
using WeAreCollections.CustomDataStructures.Heaps;

namespace WeAreCollections.Tests
{
    public class HeapAddingUniqueTest : IPerformanceTest
    {
        public int SuggestedTestStepsCount
        {
            get { return 2000000; }
        }

        public string GetTestDescription (int expectedTestStepsCount)
        {
            return string.Format (CultureInfo.InvariantCulture, "Heap: adding {0:n0} unique elements", expectedTestStepsCount);
        }

        public void Initialize (Random rnd, int expectedTestStepsCount)
        {
            this.expectedTestStepsCount = expectedTestStepsCount;
            testedHeap = new MaxHeap<int>();

            valuesToUse = TestHelper.GenerateRandomUniqueTestSet (rnd, expectedTestStepsCount);
        }

        public void ExecuteStep (int step)
        {
            int value = valuesToUse[step];

            // tested code
            testedHeap.Add (value);
        }

        public void AssertValidity ()
        {
            if (testedHeap.Count != expectedTestStepsCount)
                throw new InvalidOperationException ("testedSet.Count != expectedTestStepsCount");
        }

        private MaxHeap<int> testedHeap;
        private int[] valuesToUse;
        private int expectedTestStepsCount;
    }
}