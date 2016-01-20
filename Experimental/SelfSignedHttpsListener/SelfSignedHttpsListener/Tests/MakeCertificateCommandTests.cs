using LibroLib.ConsoleShells;
using NUnit.Framework;

namespace SelfSignedHttpsListener.Tests
{
    public class MakeCertificateCommandTests
    {
        [Test, Explicit]
        public void Test()
        {
            IConsoleEnvironment shell = new ConsoleShell ("whatever");

            MakeCertificateCommand cmd = new MakeCertificateCommand ();
            int exitCode = cmd.Execute(shell);

            Assert.AreEqual(0, exitCode);
        }
    }
}