using System;
using System.Collections.Generic;
using System.Globalization;

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
            displayState = DisplayState.Cleared;
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

                case Key.Add:
                case Key.Sub:
                case Key.Mul:
                case Key.Div:
                    HandleOperationKey(key);
                    return;

                case Key.Equals:
                    HandleEquals();
                    return;

                case Key.Dot:
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

            switch (displayState)
            {
                case DisplayState.Uninitialized:
                    throw new InvalidOperationException();
                case DisplayState.Cleared:
                    if (numChar == '0')
                        return;
                    displayState = DisplayState.EnteringNumber;
                    currentlyDisplayedString = string.Empty;
                    break;
                case DisplayState.EnteringNumber:
                    break;
                case DisplayState.AfterOperation:
                    displayState = DisplayState.EnteringNumber;
                    currentlyDisplayedString = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentlyDisplayedString += numChar;
            UpdateDisplay();
        }

        private void HandleOperationKey(Key operationKey)
        {
            ParseAndStoreCurrentValue();
            displayState = DisplayState.AfterOperation;

            switch (operationKey)
            {
                case Key.Add:
                    selectedOperation = SelectedOperation.Addition;
                    break;
                case Key.Sub:
                    selectedOperation = SelectedOperation.Subtraction;
                    break;
                case Key.Mul:
                    selectedOperation = SelectedOperation.Multiplication;
                    break;
                case Key.Div:
                    selectedOperation = SelectedOperation.Division;
                    break;
                default:
                    throw new InvalidOperationException("BUG");
            }
        }

        private void HandleEquals()
        {
            if (selectedOperation == SelectedOperation.None)
                return;
            
            ParseAndStoreCurrentValue();
            PerformOperation();

            displayState = DisplayState.AfterOperation;
            selectedOperation = SelectedOperation.None;
        }

        private void PerformOperation()
        {
            double val2 = valueStack.Pop();
            double val1 = valueStack.Pop();

            double result;
            switch (selectedOperation)
            {
                case SelectedOperation.Addition:
                    result = val1 + val2;
                    break;
                case SelectedOperation.Subtraction:
                    result = val1 - val2;
                    break;
                case SelectedOperation.Multiplication:
                    result = val1 * val2;
                    break;
                case SelectedOperation.Division:
                    result = val1 / val2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PushValueOnDisplay (result);
        }

        private void PushValueOnDisplay(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                currentlyDisplayedString = "ERR";
            else
            {
                valueStack.Push (value);
                currentlyDisplayedString = value.ToString (CultureInfo.InvariantCulture);
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            display.SetText(currentlyDisplayedString);
        }

        private void ParseAndStoreCurrentValue()
        {
            double currentValue = double.Parse(currentlyDisplayedString, NumberStyles.Any,
                CultureInfo.InvariantCulture);
            valueStack.Push(currentValue);
        }

        private readonly IDisplay display;
        private DisplayState displayState = DisplayState.Uninitialized;
        private string currentlyDisplayedString;
        private readonly Stack<double> valueStack = new Stack<double>();
        private SelectedOperation selectedOperation = SelectedOperation.None;

        private enum DisplayState
        {   
            Uninitialized,
            Cleared,
            EnteringNumber,
            AfterOperation
        }

        private enum SelectedOperation
        {
            None,
            Addition,
            Subtraction,
            Multiplication,
            Division,
        }
    }
}