using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GnuCashUtils.Framework
{
    public struct DecimalValue
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer")]
        public int IntegerValue
        {
            get { return this.integerValue; }
        }

        public int Fraction
        {
            get { return fraction; }
        }

        public decimal Value
        {
            get { return ((decimal)IntegerValue) / Fraction; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer")]
        public DecimalValue(int integerValue, int fraction)
        {
            this.integerValue = integerValue;
            this.fraction = fraction;
        }

        public DecimalValue (decimal value, int fraction)
        {
            this.integerValue = (int)value * fraction;
            this.fraction = fraction;
        }

        public override string ToString ()
        {
            return String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "{0}", Value);
        }

        public string ToGnuCashValueString ()
        {
            return String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "{0}/{1}", integerValue, fraction);
        }

        /// <summary>
        /// Compares the current <see cref="DecimalValue"/> object to the specified object for equivalence.
        /// </summary>
        /// <param name="obj">The <see cref="DecimalValue"/> object to test for equivalence with the current object.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="DecimalValue"/> objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;

            DecimalValue that = (DecimalValue)obj;

            return this.Value.Equals (that.Value);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="DecimalValue"/> object.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode ()
        {
            return integerValue.GetHashCode () ^ fraction.GetHashCode ();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="valueA">The first object.</param>
        /// <param name="valueB">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator == (DecimalValue valueA, DecimalValue valueB)
        {
            return valueA.Equals (valueB);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="valueA">The first object.</param>
        /// <param name="valueB">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator != (DecimalValue valueA, DecimalValue valueB)
        {
            return !valueA.Equals (valueB);
        }
                
        private int integerValue;
        private int fraction;
    }
}
