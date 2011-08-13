using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework
{
    public sealed class ExpressionCalculator
    {
        static public decimal CalculateExpression (string expression, IFormatProvider formatProvider)
        {
            if (expression == null)
                throw new ArgumentNullException ("expression");

            if (formatProvider == null)
                throw new ArgumentNullException ("formatProvider");                
            
            decimal expressionResult = decimal.Zero;
            char lastOperator = '+';

            char[] operators = {'+', '-', '*', '/'};

            int i = 0;
            while (i < expression.Length)
            {
                int nextOperatorIndex = expression.IndexOfAny (operators, i);

                int nextTokenIndex = nextOperatorIndex;
                if (nextTokenIndex == -1)
                    nextTokenIndex = expression.Length;

                decimal value = decimal.Parse (expression.Substring (i, nextTokenIndex-i), formatProvider);

                switch (lastOperator)
                {
                    case '+':
                        expressionResult += value;
                        break;
                    case '-':
                        expressionResult -= value;
                        break;
                    case '/':
                        expressionResult /= value;
                        break;
                    case '*':
                        expressionResult *= value;
                        break;
                }

                if (nextOperatorIndex == -1)
                    break;
                lastOperator = expression[nextOperatorIndex];
                i = nextOperatorIndex+1;
            }

            return expressionResult;
        }

        static public DecimalValue GetDecimalValue (string expression, IFormatProvider formatProvider)
        {
            if (expression == null)
                throw new ArgumentNullException ("expression");                

            if (formatProvider == null)
                throw new ArgumentNullException ("formatProvider");                
               
            string[] splits = expression.Split ('/');

            if (splits.Length != 2)
                throw new ArgumentException (String.Format (System.Globalization.CultureInfo.InvariantCulture,
                    "Invalid decimal value '{0}'", expression));

            return new DecimalValue (int.Parse (splits[0], formatProvider),
                int.Parse (splits[1], formatProvider));
        }

        private ExpressionCalculator () { }
    }
}
