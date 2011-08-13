using System;
using System.Threading;

namespace Clavis.Tests
{
    public class TestThreadAction
    {
        public TestThreadAction(
            MultithreadedTestRunner runner,
            Func<object, bool> threadAction,
            WaitHandle startSignal,
            WaitHandle stopSignal,
            object data)
        {
            this.runner = runner;
            this.threadAction = threadAction;
            this.startSignal = startSignal;
            this.stopSignal = stopSignal;
            this.data = data;
        }

        public void ThreadLoop()
        {
            try
            {
                bool received = startSignal.WaitOne(TimeSpan.FromSeconds(5));
                if (false == received)
                    return;

                while (!stopSignal.WaitOne(TimeSpan.Zero))
                {
                    if (!threadAction(data))
                        break;
                }
            }
            catch (Exception ex)
            {
                runner.RegisterThreadFailure(ex);
            }

            runner.ReportThreadHasStopped();
        }

        private readonly MultithreadedTestRunner runner;
        private readonly Func<object, bool> threadAction;
        private readonly WaitHandle startSignal;
        private readonly WaitHandle stopSignal;
        private readonly object data;
    }
}