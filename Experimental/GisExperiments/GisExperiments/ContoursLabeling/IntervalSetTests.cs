using MbUnit.Framework;

namespace GisExperiments.ContoursLabeling
{
    public class IntervalSetTests
    {
        [Test]
        public void LengthOfEmpty()
        {
            Assert.AreEqual(0, set.TotalIntervalsLength);
        }

        [Test]
        public void LengthOfSingle()
        {
            set.AddInterval(10, 30);
            Assert.AreEqual (20, set.TotalIntervalsLength);
        }

        [Test]
        public void Whole()
        {
            set.AddInterval(0, 100);
            Assert.AreEqual (100, set.TotalIntervalsLength);
        }

        [Test]
        public void InsertIntervalBefore()
        {
            set.AddInterval(40, 50);
            set.AddInterval (10, 30);
            Assert.AreEqual (30, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtendExistingIntervalToTheRight1 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (40, 65);
            Assert.AreEqual (25, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtendExistingIntervalToTheRight2 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (50, 65);
            Assert.AreEqual (25, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtendExistingIntervalToTheLeft1 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (25, 50);
            Assert.AreEqual (25, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtendExistingIntervalToTheLeft2 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (25, 40);
            Assert.AreEqual (25, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtendIntervalToRight ()
        {
            set.AddInterval (10, 30);
            set.AddInterval (20, 40);
            Assert.AreEqual (30, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtendIntervalToLeft ()
        {
            set.AddInterval (10, 30);
            set.AddInterval (5, 25);
            Assert.AreEqual (25, set.TotalIntervalsLength);
        }

        [Test]
        public void OverrideTwoIntervals1 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (10, 30);
            set.AddInterval (5, 55);
            Assert.AreEqual (50, set.TotalIntervalsLength);
        }

        [Test]
        public void OverrideTwoIntervals2 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (10, 30);
            set.AddInterval (10, 50);
            Assert.AreEqual (40, set.TotalIntervalsLength);
        }

        [Test]
        public void InsertBetweenTwoIntervals ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (10, 30);
            set.AddInterval (15, 45);
            Assert.AreEqual (40, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtraTest1 ()
        {
            set.AddInterval (40, 50);
            set.AddInterval (45, 50);
            Assert.AreEqual (10, set.TotalIntervalsLength);
        }

        [Test]
        public void ExtraTest2 ()
        {
            set.AddInterval (20, 30);
            set.AddInterval (40, 50);
            set.AddInterval (15, 50);
            Assert.AreEqual (35, set.TotalIntervalsLength);
        }

        [SetUp]
        public void Setup()
        {
            set = new IntervalSet (100);
        }

        [TearDown]
        public void Teardown()
        {
            set.Validate();
        }

        private IntervalSet set;
    }
}