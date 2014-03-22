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
            tests.Add(new BufferBlockCopyTest());
            tests.Add(new ArrayCopyTest());
            tests.Add(new BufferBlockCopyTest());

            ExecuteTests(tests);
        }

        private static void ExecuteTests(IList<IPerformanceTest> tests)
        {
            PerformanceTestsExecutor executor = new PerformanceTestsExecutor();
            executor.RunTests(tests);
        }
    }
}
