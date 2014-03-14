using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WeAreCollections
{
    public class PerformanceTestsExecutor
    {
        public void RunTests(IList<IPerformanceTest> tests)
        {
            foreach (IPerformanceTest test in tests)
            {
                Random rnd = new Random(1);
                Stopwatch watch = new Stopwatch();

                Console.Out.WriteLine("Test: {0}...", test.GetTestDescription(test.SuggestedTestStepsCount));
                Console.Out.WriteLine("Initializing test...");

                watch.Start ();

                test.Initialize(rnd, test.SuggestedTestStepsCount);
                test.ExecuteStep(0);

                Console.Out.WriteLine ("Test initialized in {0} ms", watch.ElapsedMilliseconds);

                Console.Out.WriteLine ("Executing test...");

                watch.Restart();
                for (int i = 1; i < test.SuggestedTestStepsCount; i++)
                    test.ExecuteStep(i);

                test.AssertValidity();
                
                Console.Out.WriteLine("Time taken: {0} ms", watch.ElapsedMilliseconds);
            }
        }
    }
}