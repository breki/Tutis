using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace GnuCashUtils.Test
{
    [TestFixture]
    public class ConsoleTests
    {
        [MbUnit.Framework.Test,Explicit]
        public void Test ()
        {
            string[] args = {""};

            GnuCashUtils.Console.Program.Main (args);
        }
    }
}
