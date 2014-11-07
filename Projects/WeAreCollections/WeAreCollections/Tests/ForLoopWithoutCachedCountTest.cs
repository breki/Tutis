using System.Collections.Generic;
using System.Globalization;

namespace WeAreCollections.Tests
{
    public class ForLoopWithoutCachedCountTest : ForLoopTestBase
    {
        public ForLoopWithoutCachedCountTest(int listSize, int steps) : base(listSize, steps)
        {
        }

        public int Sum
        {
            get { return sum; }
        }

        public override string GetTestDescription(int expectedTestStepsCount)
        {
            return string.Format (CultureInfo.InvariantCulture, "Runs for loop on a List<> of {0} elements (without cached list size)", ListSize);
        }

        public override void ExecuteStep(int step)
        {
            sum = 0;
            List<int> list = List;
            for (int i = 0; i < list.Count; i++)
            {
                int item = list[i];
                sum += item;
            }
        }

        private int sum;
    }
}