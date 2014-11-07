using System;
using System.Collections.Generic;

namespace WeAreCollections.Tests
{
    public abstract class ForLoopTestBase : IPerformanceTest
    {
        public abstract string GetTestDescription(int expectedTestStepsCount);

        public void Initialize(Random rnd, int expectedTestStepsCount)
        {
            list = new List<int>(listSize);

            for (int i = 0; i < listSize; i++)
                list.Add(rnd.Next());
        }

        public abstract void ExecuteStep(int step);

        public int SuggestedTestStepsCount
        {
            get { return steps; }
        }

        public void AssertValidity()
        {
        }

        protected ForLoopTestBase(int listSize, int steps)
        {
            this.listSize = listSize;
            this.steps = steps;
        }

        protected int ListSize
        {
            get { return listSize; }
        }

        protected int Steps
        {
            get { return steps; }
        }

        protected List<int> List
        {
            get { return list; }
        }

        private int listSize;
        private int steps;
        private List<int> list;
    }
}