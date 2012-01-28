using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace TejkABrejk.Tests
{
    public class TickerTests
    {
        [Test]
        public void TickerShouldCountActiveTime ()
        {
            ticker.Tick (new DateTime (2012, 01, 27, 18, 21, 00), false);
            ticker.Tick (new DateTime (2012, 01, 27, 19, 21, 00), false);
            ticker.Tick (new DateTime (2012, 01, 27, 19, 21, 00), true);
            Assert.AreEqual(TimeSpan.FromMinutes(2), ticker.ActiveTime);
        }

        [Test]
        public void TickerShouldBeResetAfterEachDay()
        {
            ticker.Tick(new DateTime (2012, 01, 27, 18, 21, 00), false);
            ticker.Tick(new DateTime (2012, 01, 27, 20, 21, 00), false);
            ticker.Tick(new DateTime (2012, 01, 28, 06, 21, 00), false);
            Assert.AreEqual (TimeSpan.FromMinutes (1), ticker.ActiveTime);
        }

        [Test]
        public void ShowAlarmAfterEnoughActiveTime()
        {
            DateTime baseTime = new DateTime (2012, 01, 27, 18, 21, 00);

            for (int i = 0; i < 59; i++)
                ticker.Tick (baseTime.AddMinutes(i), false);

            alarm.Expect(x => x.ShowAlarm()).Repeat.Once();

            ticker.Tick (baseTime.AddMinutes (60), false);

            alarm.VerifyAllExpectations();
        }

        [Test]
        public void DoNotCountTicksWhenExercising()
        {
            DateTime baseTime = new DateTime (2012, 01, 27, 18, 21, 00);

            for (int i = 0; i < 80; i++)
                ticker.Tick (baseTime.AddMinutes (i), false);

            alarm.Raise(x => x.ExerciseFinished += null, null, EventArgs.Empty);

            ticker.Tick (baseTime.AddMinutes (100), false);
            Assert.AreEqual (TimeSpan.FromMinutes (1), ticker.ActiveTime);
        }

        [SetUp]
        public void Setup()
        {
            TabConfiguration config = new TabConfiguration();
            alarm = MockRepository.GenerateMock<IExerciseAlarm> ();
            ticker = new Ticker (config, alarm);
        }

        private Ticker ticker;
        private IExerciseAlarm alarm;
    }
}