using System;

namespace CalculatorTdd
{
    public class Calculator
    {
        public Calculator(IDisplay display)
        {
            this.display = display;
        }

        public void Init()
        {
            inClearedMode = true;
            currentlyDisplayedString = "0";
            UpdateDisplay();
        }

        public void HandleKeyPress(Key key)
        {
            switch (key)
            {
                case Key.N0:
                case Key.N1:
                case Key.N2:
                case Key.N3:
                case Key.N4:
                case Key.N5:
                case Key.N6:
                case Key.N7:
                case Key.N8:
                case Key.N9:
                {
                    HandleNumericKeyPress(key);
                    return;
                }

                case Key.Dot:
                case Key.Add:
                case Key.Sub:
                case Key.Mul:
                case Key.Div:
                case Key.Equals:
                case Key.C:
                case Key.MRC:
                case Key.MPlus:
                case Key.MMinus:
                default:
                    throw new NotImplementedException("todo next:");
            }
        }

        private void HandleNumericKeyPress(Key key)
        {
            char numChar = (char) (key - Key.N0 + '0');

            if (inClearedMode)
            {
                if (numChar == '0')
                    return;

                currentlyDisplayedString = string.Empty;
                inClearedMode = false;
            }

            currentlyDisplayedString += numChar;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            display.SetText (currentlyDisplayedString);
        }

        private readonly IDisplay display;
        private bool inClearedMode;
        private string currentlyDisplayedString;
    }
}