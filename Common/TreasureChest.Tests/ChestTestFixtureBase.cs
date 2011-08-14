using System.Reflection;
using Gallio.Framework;
using log4net;
using MbUnit.Framework;
using TreasureChest.Logging;

namespace TreasureChest.Tests
{
    public class ChestTestFixtureBase
    {
        protected Chest Chest
        {
            get { return chest; }
            set { chest = value; }
        }

        [SetUp]
        protected virtual void Setup()
        {
            chest = new Chest(new Log4NetLogger());
            log.DebugFormat ("Running {0}", TestContext.CurrentContext.TestStep.FullName);
        }

        [TearDown]
        protected virtual void Teardown()
        {
            if (chest != null)
                chest.Dispose();

            log.DebugFormat ("Finished {0} ({1})", TestContext.CurrentContext.TestStep.FullName, TestContext.CurrentContext.Outcome);
            log.DebugFormat (string.Empty);
        }

        private Chest chest;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}