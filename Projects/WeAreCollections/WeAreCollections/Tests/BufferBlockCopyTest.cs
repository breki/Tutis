using System;

namespace WeAreCollections.Tests
{
    public class BufferBlockCopyTest : IPerformanceTest
    {
        public int SuggestedTestStepsCount
        {
            get { return 100000; }
        }

        public string GetTestDescription(int expectedTestStepsCount)
        {
            return "Buffer.BlockCopy() between two byte arrays";
        }

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            source = new byte[100000];
            dest = new byte[120000];
        }

        public void ExecuteStep(int step)
        {
            Buffer.BlockCopy(source, 0, dest, 10000, 100000);
        }

        public void AssertValidity()
        {
        }

        private byte[] source;
        private byte[] dest;
    }
}