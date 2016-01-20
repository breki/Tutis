using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace SelfSignedHttpsListener.Tests
{
    public class CertificateToolsTests
    {
        [Test]
        public void SelfSignedCertificateGeneratorTest()
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator();
            X509Certificate2 cert = generator.GenerateCertificate();
            Assert.IsNotNull(cert);
        }

        [Test]
        public void InstallCertificateInStore()
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator ();
            X509Certificate2 cert = generator.GenerateCertificate ();
    
            CertificateStoreInstaller installer = new CertificateStoreInstaller();
            installer.InstallCertificate(cert);
        }
    }
}