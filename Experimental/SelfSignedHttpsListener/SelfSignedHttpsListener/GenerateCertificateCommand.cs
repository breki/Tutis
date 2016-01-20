using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using LibroLib.ConsoleShells;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace SelfSignedHttpsListener
{
    // http://stackoverflow.com/questions/11403333/httplistener-with-https-support
    // http://stackoverflow.com/questions/4004/how-do-i-add-ssl-to-a-net-application-that-uses-httplistener-it-will-not-be
    // http://blog.differentpla.net/blog/2013/03/18/using-bouncy-castle-from-net/
    public class GenerateCertificateCommand : StandardConsoleCommandBase
    {
        public override string CommandId
        {
            get { return "makecert"; }
        }

        public override object Description
        {
            get { return "Generates a self-signed certificate for localhost"; }
        }

        public override int Execute(IConsoleEnvironment env)
        {
            SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator();
            System.Security.Cryptography.X509Certificates.X509Certificate selfSignedCertificate = generator.GenerateCertificate();

            return 0;
        }
    }
}