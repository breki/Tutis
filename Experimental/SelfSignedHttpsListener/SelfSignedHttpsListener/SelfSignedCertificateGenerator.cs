using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

namespace SelfSignedHttpsListener
{
    public class SelfSignedCertificateGenerator
    {
        public DateTime CertificateExpiryTime
        {
            get { return certificateExpiryTime; }
            set { certificateExpiryTime = value; }
        }

        public string SignatureAlgorithm
        {
            get { return signatureAlgorithm; }
            set { signatureAlgorithm = value; }
        }

        public X509Certificate GenerateCertificate()
        {
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator ();

            IRandomGenerator randomGenerator = new CryptoApiRandomGenerator ();
            SecureRandom random = new SecureRandom (randomGenerator);

            GenerateCertSerialNumber (certificateGenerator, random);
            SetSignatureAlgorithm (certificateGenerator);
            SetCertificateSubjectDn (certificateGenerator);
            SetCertificateDates (certificateGenerator);

            AsymmetricCipherKeyPair subjectKeyPair = GenerateKeyPair (random);

            certificateGenerator.SetPublicKey (subjectKeyPair.Public);

            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate (issuerKeyPair.Private, random);

            //X509Certificate convertedCertificate = ConvertToX509Certificate2 (certificate, issuerKeyPair.Private, random);
            X509Certificate convertedCertificate = DotNetUtilities.ToX509Certificate(certificate);
            return convertedCertificate;
        }

        private static void GenerateCertSerialNumber (X509V3CertificateGenerator certificateGenerator, SecureRandom random)
        {
            BigInteger serialNumber = BigIntegers.CreateRandomInRange (BigInteger.One, BigInteger.ValueOf (long.MaxValue), random);
            certificateGenerator.SetSerialNumber (serialNumber);
        }

        private void SetSignatureAlgorithm (X509V3CertificateGenerator certificateGenerator)
        {
            certificateGenerator.SetSignatureAlgorithm (signatureAlgorithm);
        }

        private static void SetCertificateSubjectDn (X509V3CertificateGenerator certificateGenerator)
        {
            X509Name subjectDn = new X509Name ("CN=localhost");
            X509Name issuerDn = subjectDn;
            certificateGenerator.SetIssuerDN (issuerDn);
            certificateGenerator.SetSubjectDN (subjectDn);
        }

        private void SetCertificateDates (X509V3CertificateGenerator certificateGenerator)
        {
            DateTime notBefore = DateTime.UtcNow.Date;
            DateTime notAfter = certificateExpiryTime;

            certificateGenerator.SetNotBefore (notBefore);
            certificateGenerator.SetNotAfter (notAfter);
        }

        private static AsymmetricCipherKeyPair GenerateKeyPair (SecureRandom random)
        {
            const int Strength = 2048;
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters (random, Strength);

            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator ();
            keyPairGenerator.Init (keyGenerationParameters);
            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair ();
            return subjectKeyPair;
        }

        // ReSharper disable once UnusedMember.Local
        private static X509Certificate2 ConvertToX509Certificate2 (
            Org.BouncyCastle.X509.X509Certificate certificate,
            AsymmetricKeyParameter privateKey,
            SecureRandom random)
        {
            Pkcs12Store store = new Pkcs12Store ();

            //string friendlyName = certificate.SubjectDN.ToString ();
            string friendlyName = "xyz";
            X509CertificateEntry certificateEntry = new X509CertificateEntry (certificate);
            store.SetCertificateEntry (friendlyName, certificateEntry);
            store.SetKeyEntry (friendlyName, new AsymmetricKeyEntry (privateKey), new[] { certificateEntry });

            using (MemoryStream stream = new MemoryStream ())
            {
                const string Password = "password";

                store.Save (stream, Password.ToCharArray (), random);

                return new X509Certificate2 (
                    stream.ToArray (),
                    Password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
        }

        private string signatureAlgorithm = "SHA256WithRSA";
        private DateTime certificateExpiryTime = DateTime.Now.AddYears(1);
    }
}