using Moq;
using NUnit.Framework;

namespace CalculatorTdd.Tests
{
    public class BasicTests
    {
        [Test]
        public void ShouldShow0OnStart()
        {
            calculator.Init ();

            displayMock.AssertFinalText("0");
        }

        [Test]
        public void PressingNumericKeyShowsThatDigit()
        {
            calculator.Init ();

            calculator.HandleKeyPress(Key.N5);
            calculator.HandleKeyPress(Key.N2);
            calculator.HandleKeyPress(Key.N0);

            displayMock.AssertFinalText ("520");
        }

        [Test]
        public void LeadingZerosShouldNotBeShown()
        {
            calculator.Init ();

            calculator.HandleKeyPress(Key.N0);
            calculator.HandleKeyPress(Key.N0);
            calculator.HandleKeyPress(Key.N0);

            displayMock.AssertLog (new[] { "0" });
        }

        [Test]
        public void AddTwoNumbers()
        {
            calculator.Init ();

            calculator.HandleKeyPress (Key.N5);
            calculator.HandleKeyPress (Key.N8);
            calculator.HandleKeyPress (Key.Add);
            calculator.HandleKeyPress (Key.N7);
            calculator.HandleKeyPress (Key.Equals);

            displayMock.AssertLog (new[] { "0", "5", "58", "7", "65" });
        }

        [Test]
        public void MultiplyTwoNumbers()
        {
            calculator.Init ();

            calculator.HandleKeyPress (Key.N5);
            calculator.HandleKeyPress (Key.N8);
            calculator.HandleKeyPress (Key.Mul);
            calculator.HandleKeyPress (Key.N7);
            calculator.HandleKeyPress (Key.Equals);

            displayMock.AssertLog (new[] { "0", "5", "58", "7", "406" });
        }

        [Test]
        public void DivisionByZeroShouldShowError()
        {
            calculator.Init ();

            calculator.HandleKeyPress (Key.N5);
            calculator.HandleKeyPress (Key.N8);
            calculator.HandleKeyPress (Key.Div);
            calculator.HandleKeyPress (Key.N0);
            calculator.HandleKeyPress (Key.Equals);

            displayMock.AssertLog (new[] { "0", "5", "58", "0", "ERR" });
        }

        [Test]
        public void PressingEqualsRightAtStartShouldNotCauseError()
        {
            calculator.Init ();

            calculator.HandleKeyPress (Key.Equals);
        }

        [Test]
        public void PressingEqualsAfterOperationButtonShouldReuseValue()
        {
            calculator.Init ();

            calculator.HandleKeyPress (Key.N5);
            calculator.HandleKeyPress (Key.N8);
            calculator.HandleKeyPress (Key.Add);
            calculator.HandleKeyPress (Key.Equals);

            displayMock.AssertLog (new[] { "0", "5", "58", "116" });
        }

        [SetUp]
        public void Setup()
        {
            displayMock = new MockDisplay();
            calculator = new Calculator(displayMock);
        }

        private Calculator calculator;
        private MockDisplay displayMock;
    }
}