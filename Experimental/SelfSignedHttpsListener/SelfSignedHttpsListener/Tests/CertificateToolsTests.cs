using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using SelfSignedHttpsListener.Commands;

namespace SelfSignedHttpsListener.Tests
{
    public class CertificateToolsTests
    {
        [Test]
        public void SelfSignedCertificateGeneratorTest()
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator();
            X509Certificate2 cert = generator.GenerateCertificate ("SelfSignedHttpsListener certificate");
            Assert.IsNotNull(cert);
        }

        [Test]
        public void InstallCertificateInStore()
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator ();
            X509Certificate2 cert = generator.GenerateCertificate ("SelfSignedHttpsListener certificate");
    
            CertificateStoreInstaller installer = new CertificateStoreInstaller();
            installer.InstallCertificate(cert);
        }

        [Test]
        public void BindCertificateToPort()
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator ();
            X509Certificate2 cert = generator.GenerateCertificate ("SelfSignedHttpsListener certificate");

            CertificateStoreInstaller installer = new CertificateStoreInstaller ();
            installer.InstallCertificate (cert);

            CertificateBinder binder = new CertificateBinder();
            binder.BindCertificate(cert, RunWebServerCommand.DefaultPort);
        }
    }
}