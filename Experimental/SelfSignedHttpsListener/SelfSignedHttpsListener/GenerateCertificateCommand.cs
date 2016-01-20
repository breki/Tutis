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
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator ();

            IRandomGenerator randomGenerator = new CryptoApiRandomGenerator ();
            SecureRandom random = new SecureRandom (randomGenerator);

            GenerateCertSerialNumber(certificateGenerator, random);
            SetSignatureAlgorithm(certificateGenerator);
            SetCertificateSubjectDn(certificateGenerator);
            SetCertificateDates(certificateGenerator);

            AsymmetricCipherKeyPair subjectKeyPair = GenerateKeyPair(random);

            certificateGenerator.SetPublicKey (subjectKeyPair.Public);

            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;
            X509Certificate certificate = certificateGenerator.Generate (issuerKeyPair.Private, random);

            X509Certificate2 convertedCertificate = ConvertToX509Certificate2(certificate, issuerKeyPair.Private, random);

            return 0;
        }

        private static void GenerateCertSerialNumber(X509V3CertificateGenerator certificateGenerator, SecureRandom random)
        {
            BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
        }

        private static void SetSignatureAlgorithm(X509V3CertificateGenerator certificateGenerator)
        {
            const string SignatureAlgorithm = "SHA256WithRSA";
            certificateGenerator.SetSignatureAlgorithm(SignatureAlgorithm);
        }

        private static void SetCertificateSubjectDn(X509V3CertificateGenerator certificateGenerator)
        {
            X509Name subjectDn = new X509Name("CN=localhost");
            X509Name issuerDn = subjectDn;
            certificateGenerator.SetIssuerDN(issuerDn);
            certificateGenerator.SetSubjectDN(subjectDn);
        }

        private static void SetCertificateDates(X509V3CertificateGenerator certificateGenerator)
        {
            DateTime notBefore = DateTime.UtcNow.Date;
            DateTime notAfter = notBefore.AddYears(1);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);
        }

        private static AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random)
        {
            const int Strength = 2048;
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, Strength);

            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            return subjectKeyPair;
        }

        private static X509Certificate2 ConvertToX509Certificate2(
            X509Certificate certificate, 
            AsymmetricKeyParameter privateKey,
            SecureRandom random)
        {
            Pkcs12Store store = new Pkcs12Store ();

            //string friendlyName = certificate.SubjectDN.ToString ();
            string friendlyName = "xyz";
            X509CertificateEntry certificateEntry = new X509CertificateEntry (certificate);
            store.SetCertificateEntry (friendlyName, certificateEntry);
            store.SetKeyEntry (friendlyName, new AsymmetricKeyEntry (privateKey), new[] { certificateEntry });

            using (MemoryStream stream = new MemoryStream())
            {
                const string Password = "password";

                store.Save (stream, Password.ToCharArray (), random);

                return new X509Certificate2 (
                    stream.ToArray (), 
                    Password, 
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
        }
    }
}