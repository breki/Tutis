using System.Collections.Generic;
using System.Globalization;

namespace WeAreCollections.Tests
{
    public class ForLoopWithCachedCountTest : ForLoopTestBase
    {
        public ForLoopWithCachedCountTest(int listSize, int steps) : base(listSize, steps)
        {
        }

        public override string GetTestDescription(int expectedTestStepsCount)
        {
            return string.Format (CultureInfo.InvariantCulture, "Runs for loop on a List<> of {0} elements (with cached list size)", ListSize);
        }

        public int Sum
        {
            get { return sum; }
        }

        public override void ExecuteStep(int step)
        {
            sum = 0;
            List<int> list = List;
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                int item = list[i];
                sum += item;
            }
        }

        private int sum;
    }
}