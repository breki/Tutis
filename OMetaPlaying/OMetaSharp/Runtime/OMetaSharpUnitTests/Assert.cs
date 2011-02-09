using System;
using System.Diagnostics;
using System.Linq;

namespace OMetaSharp.UnitTests
{
    public static class Assert
    {           
        public static void AreEqual(string expected, string actual)
        {
            IsTrue(string.Equals(expected, actual, StringComparison.Ordinal));
        }

        public static void AreEqual(string expected, object actual)
        {
            if (actual == null)
            {
                IsTrue(false);
            }

            if (actual.Equals(expected))
            {
                return;
            }

            AreEqual(expected, actual.ToString());
        }

        public static void AreEqual(object expected, object actual)
        {
            if (actual != null)
            {
                IsTrue(actual.Equals(expected));
            }
            else
            {
                IsTrue(expected.Equals(actual));
            }
        }

        public static void IsTrue(bool value)
        {
            if (!value)
            {
                Debugger.Break();
            }
            Debug.Assert(value);
        }

        public static void IsFalse(bool value)
        {
            if (value)
            {
                Debugger.Break();
            }
            Debug.Assert(!value);
        }        
    }
}
