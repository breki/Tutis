using Moq;
using NUnit.Framework;

namespace CalculatorTdd.Tests
{
    public class BasicTests
    {
        [Test]
        public void ShouldShow0OnStart()
        {
            displayMock.Setup(x => x.SetText("0"));
            
            calculator.Init ();

            displayMock.Verify();
        }

        [Test]
        public void PressingNumericKeyShowsThatDigit()
        {
            calculator.Init ();

            displayMock.Setup (x => x.SetText ("5"));
            displayMock.Setup (x => x.SetText ("52"));
            displayMock.Setup (x => x.SetText ("520"));

            calculator.HandleKeyPress(Key.N5);
            calculator.HandleKeyPress(Key.N2);
            calculator.HandleKeyPress(Key.N0);

            displayMock.Verify ();
        }

        [Test]
        public void LeadingZerosShouldNotBeShown()
        {
            calculator.Init ();

            //displayMock.Setup (x => x.SetText (It.IsAny<string>()));

            calculator.HandleKeyPress(Key.N0);
            calculator.HandleKeyPress(Key.N0);
            calculator.HandleKeyPress(Key.N0);

            displayMock.Verify(x => x.SetText("0"), Times.Once);
        }

        [SetUp]
        public void Setup()
        {
            displayMock = new Mock<IDisplay>();
            calculator = new Calculator(displayMock.Object);
        }

        private Calculator calculator;
        private Mock<IDisplay> displayMock;
    }
}