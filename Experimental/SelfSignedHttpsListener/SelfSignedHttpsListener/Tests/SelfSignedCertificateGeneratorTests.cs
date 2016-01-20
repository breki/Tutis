using NUnit.Framework;

namespace SelfSignedHttpsListener.Tests
{
    public class SelfSignedCertificateGeneratorTests
    {
        [Test]
        public void Test()
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator();
            generator.GenerateCertificate();
        } 
    }
}