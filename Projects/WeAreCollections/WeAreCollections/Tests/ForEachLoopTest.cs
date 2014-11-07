using System.Collections.Generic;
using System.Globalization;

namespace WeAreCollections.Tests
{
    public class ForEachLoopTest : ForLoopTestBase
    {
        public ForEachLoopTest(int listSize, int steps) : base(listSize, steps)
        {
        }

        public int Sum
        {
            get { return sum; }
        }

        public override string GetTestDescription(int expectedTestStepsCount)
        {
            return string.Format(CultureInfo.InvariantCulture, "Runs foreach loop on a List<> of {0} elements", ListSize);
        }

        public override void ExecuteStep (int step)
        {
            sum = 0;
            List<int> list = List;
            foreach (int item in list)
                sum += item;
        }

        private int sum;
    }
}