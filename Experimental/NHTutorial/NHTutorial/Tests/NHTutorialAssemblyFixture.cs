using System;
using Gallio.Framework;
using log4net;
using log4net.Config;
using MbUnit.Framework;

namespace NHTutorial.Tests
{
    [AssemblyFixture]
    public class NHTutorialAssemblyFixture : IDisposable
    {
        [FixtureSetUp]
        private void FixtureSetup()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [SetUp]
        private void Setup()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">If <code>false</code>, cleans up native resources. 
        /// If <code>true</code> cleans up both managed and native resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                    log4net.LogManager.Shutdown();

                disposed = true;
            }
        }

        private bool disposed;
        private static readonly ILog log = LogManager.GetLogger(typeof(NHTutorialAssemblyFixture));
    }
}