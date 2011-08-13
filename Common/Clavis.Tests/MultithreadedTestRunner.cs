using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using log4net;
using MbUnit.Framework;

namespace Clavis.Tests
{
    public class MultithreadedTestRunner : IDisposable
    {
        public ManualResetEvent StartSignal
        {
            get { return startSignal; }
        }

        public EventWaitHandle StopSignal
        {
            get { return stopSignal; }
        }

        public void AddThread(Func<object, bool> threadAction, object data)
        {
            TestThreadAction testThreadAction = new TestThreadAction(
                this,
                threadAction,
                startSignal,
                stopSignal,
                data);
            Thread thread = new Thread(testThreadAction.ThreadLoop);
            threads.Add(thread);
        }

        public void AddThreads(int threadCount, Func<object, bool> threadAction)
        {
            for (int i = 0; i < threadCount; i++)
            {
                TestThreadAction testThreadAction = new TestThreadAction(
                    this,
                    threadAction,
                    startSignal,
                    stopSignal,
                    null);
                Thread thread = new Thread(testThreadAction.ThreadLoop);
                threads.Add(thread);
            }
        }

        public void AssertNoFailures()
        {
            foreach (Exception failure in threadFailures)
                Assert.Fail("Thread failed: {0}", failure);
        }

        public void RegisterThreadFailure(Exception failure)
        {
            threadFailures.Add(failure);
        }

        public void ReportThreadHasStopped()
        {
            lock (this)
            {
                runningThreadsCount--;
                if (runningThreadsCount == 0)
                    stopSignal.Set();
            }
        }

        public void Run(TimeSpan duration)
        {
            log.Debug("Starting threads");

            runningThreadsCount = threads.Count;
            foreach (Thread thread in threads)
                thread.Start();

            log.Debug("Setting the start signal");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            startSignal.Set();

            log.DebugFormat("Running for {0} seconds", duration.Seconds);

            if (false == stopSignal.WaitOne(duration))
            {
                stopSignal.Set();
                log.Debug("Setting the stop signal");

                WaitForAllThreadsToStop(TimeSpan.FromSeconds(10));
            }
            else
                log.Debug("All threads have finished their work before the maximum duration.");

            log.DebugFormat("Total execution time: {0} s", stopwatch.Elapsed.TotalSeconds);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    startSignal.Close();
                    stopSignal.Close();
                }

                disposed = true;
            }
        }

        private void WaitForAllThreadsToStop(TimeSpan timeout)
        {
            Stopwatch waitTime = new Stopwatch();
            waitTime.Start();
            while (waitTime.Elapsed < timeout)
            {
                lock (this)
                {
                    if (runningThreadsCount == 0)
                        return;

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            }

            throw new InvalidOperationException("Not all threads could be stopped.");
        }

        private bool disposed;
        private static readonly ILog log = LogManager.GetLogger(typeof(MultithreadedTestRunner));
        private int runningThreadsCount;
        private ManualResetEvent startSignal = new ManualResetEvent(false);
        private ManualResetEvent stopSignal = new ManualResetEvent(false);
        private List<Thread> threads = new List<Thread>();
        private List<Exception> threadFailures = new List<Exception>();
    }
}