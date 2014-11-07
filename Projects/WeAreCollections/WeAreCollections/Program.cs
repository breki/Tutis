using System.Collections.Generic;
using WeAreCollections.Tests;

namespace WeAreCollections
{
    public class Program
    {
        public static void Main (string[] args)
        {
            List<IPerformanceTest> tests = new List<IPerformanceTest>();
            //tests.Add(new SortedSetAddingUniqueTest());
            //tests.Add(new SortedDictionaryAddingUniqueTest());
            //tests.Add(new SortedListAddingUniqueTest());
            //tests.Add(new HeapAddingUniqueTest());

            //tests.Add(new BufferBlockCopyTest());
            //tests.Add(new ArrayCopyTest());

            int listSize = 100000000;
            int steps = 10;
            tests.Add(new ForEachLoopTest(listSize, steps));
            tests.Add (new ForLoopWithCachedCountTest (listSize, steps));
            tests.Add (new ForLoopWithoutCachedCountTest (listSize, steps));

            listSize = 100;
            steps = 1000000;
            tests.Add(new ForEachLoopTest(listSize, steps));
            tests.Add (new ForLoopWithCachedCountTest (listSize, steps));
            tests.Add (new ForLoopWithoutCachedCountTest (listSize, steps));

            listSize = 10;
            steps = 10000000;
            tests.Add(new ForEachLoopTest(listSize, steps));
            tests.Add (new ForLoopWithCachedCountTest (listSize, steps));
            tests.Add (new ForLoopWithoutCachedCountTest (listSize, steps));

            ExecuteTests(tests);
        }

        private static void ExecuteTests(IList<IPerformanceTest> tests)
        {
            PerformanceTestsExecutor executor = new PerformanceTestsExecutor();
            executor.RunTests(tests);
        }
    }
}
