using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Test
{
    [TestFixture]
    public class ExpressionCalculatorTests
    {
        [MbUnit.Framework.Test]
        public void Calculate ()
        {
            Assert.AreEqual (0, ExpressionCalculator.CalculateExpression (String.Empty, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual (20, ExpressionCalculator.CalculateExpression (" 20 ", System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual (20.344m, ExpressionCalculator.CalculateExpression (" 20.344 ", System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual (2.0344m, ExpressionCalculator.CalculateExpression (" 20344 / 10000 ", System.Globalization.CultureInfo.InvariantCulture));
        }

        [MbUnit.Framework.Test]
        public void GetDecimalValue ()
        {
            Assert.AreEqual (new DecimalValue (20344, 10000),
                ExpressionCalculator.GetDecimalValue (" 20344 / 10000 ", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
