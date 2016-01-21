using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

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

        public X509Certificate2 GenerateCertificate(string friendlyName)
        {
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator ();
            //certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
            //certificateGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment));
            //certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, true, new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth));
            //certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName.Id, false, new GeneralNames(new GeneralName(GeneralName.Rfc822Name, "test@test.com")));

            //Asn1Encodable[] subjectAlternativeNames = new Asn1Encodable[1];
            //// first subject alternative name is the same as the subject
            //subjectAlternativeNames[0] = new GeneralName (new X509Name ("CN=localhost"));
            //DerSequence subjectAlternativeNamesExtension = new DerSequence (subjectAlternativeNames);
            //certificateGenerator.AddExtension (X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension);

            IRandomGenerator randomGenerator = new CryptoApiRandomGenerator ();
            SecureRandom random = new SecureRandom (randomGenerator);

            BigInteger serialNumber = GenerateCertSerialNumber (certificateGenerator, random);
            SetSignatureAlgorithm (certificateGenerator);
            X509Name subjectDn;
            SetCertificateSubjectDn (certificateGenerator, out subjectDn);
            SetCertificateDates (certificateGenerator);

            AsymmetricCipherKeyPair subjectKeyPair = GenerateKeyPair (random);

            certificateGenerator.SetPublicKey (subjectKeyPair.Public);

            //certificateGenerator.AddExtension (X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifier(subjectKeyPair.Public.));

            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;
            SetAuthorityKeyIdentifier (certificateGenerator, subjectDn, issuerKeyPair, serialNumber);

            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate (issuerKeyPair.Private, random);
            
            //X509Certificate convertedCertificate = ConvertToX509Certificate2 (certificate, issuerKeyPair.Private, random);
            X509Certificate2 convertedCertificate = ConvertToX509Certificate2(certificate);
            convertedCertificate.FriendlyName = friendlyName;

            convertedCertificate.PrivateKey = DotNetUtilities.ToRSA (subjectKeyPair.Private as RsaPrivateCrtKeyParameters); 
            return convertedCertificate;
        }

        private static BigInteger GenerateCertSerialNumber (X509V3CertificateGenerator certificateGenerator, SecureRandom random)
        {
            BigInteger serialNumber = BigIntegers.CreateRandomInRange (BigInteger.One, BigInteger.ValueOf (long.MaxValue), random);
            certificateGenerator.SetSerialNumber (serialNumber);
            return serialNumber;
        }

        private void SetSignatureAlgorithm (X509V3CertificateGenerator certificateGenerator)
        {
            certificateGenerator.SetSignatureAlgorithm (signatureAlgorithm);
        }

        private static void SetCertificateSubjectDn (X509V3CertificateGenerator certificateGenerator, out X509Name subjectDn)
        {
            subjectDn = new X509Name ("CN=localhost");
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

        private static void SetAuthorityKeyIdentifier(
            X509V3CertificateGenerator certificateGenerator, 
            X509Name issuerDn,
            AsymmetricCipherKeyPair issuerKeyPair,
            BigInteger serialNumber)
        {
            // Self-signed, so it's all the same.
            BigInteger issuerSerialNumber = serialNumber;

            AuthorityKeyIdentifier authorityKeyIdentifier = new AuthorityKeyIdentifier(
                SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKeyPair.Public),
                new GeneralNames(new GeneralName(issuerDn)),
                issuerSerialNumber);
            certificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifier);
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

        private static X509Certificate2 ConvertToX509Certificate2 (
            Org.BouncyCastle.X509.X509Certificate certificate)
        {
            return new X509Certificate2(DotNetUtilities.ToX509Certificate(certificate));
        }

        private string signatureAlgorithm = "SHA512WithRSA";
        private DateTime certificateExpiryTime = DateTime.Now.AddYears(1);
    }
}