﻿using System.Collections.Generic;
using WeAreCollections.Tests;

namespace WeAreCollections
{
    public class Program
    {
        public static void Main (string[] args)
        {
            List<IPerformanceTest> tests = new List<IPerformanceTest>();
            tests.Add(new SortedSetAddingUniqueTest());

            ExecuteTests(tests);
        }

        private static void ExecuteTests(IList<IPerformanceTest> tests)
        {
            PerformanceTestsExecutor executor = new PerformanceTestsExecutor();
            executor.RunTests(tests);
        }
    }
}
