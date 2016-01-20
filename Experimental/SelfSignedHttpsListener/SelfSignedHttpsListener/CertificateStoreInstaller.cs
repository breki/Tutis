using System.Security.Cryptography.X509Certificates;

namespace SelfSignedHttpsListener
{
    public class CertificateStoreInstaller
    {
        public void InstallCertificate(X509Certificate2 certificate)
        {
            X509Store store = new X509Store (StoreName.My, StoreLocation.LocalMachine);
            store.Open (OpenFlags.ReadWrite);
            store.Add (certificate);
            store.Close (); 
        }
    }
}
