using LibroLib.ConsoleShells;
using NUnit.Framework;

namespace SelfSignedHttpsListener
{
    public class GenerateCertificateCommandTests
    {
        [Test]
        public void Test()
        {
            IConsoleEnvironment shell = new ConsoleShell ("whatever");

            GenerateCertificateCommand cmd = new GenerateCertificateCommand ();
            int exitCode = cmd.Execute(shell);

            Assert.AreEqual(0, exitCode);
        }
    }
}