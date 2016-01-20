using LibroLib.ConsoleShells;
using NUnit.Framework;

namespace SelfSignedHttpsListener.Tests
{
    public class GenerateCertificateCommandTests
    {
        [Test, Explicit]
        public void Test()
        {
            IConsoleEnvironment shell = new ConsoleShell ("whatever");

            GenerateCertificateCommand cmd = new GenerateCertificateCommand ();
            int exitCode = cmd.Execute(shell);

            Assert.AreEqual(0, exitCode);
        }
    }
}