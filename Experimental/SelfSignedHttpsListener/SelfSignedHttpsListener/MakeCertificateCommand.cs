using System;
using System.Security.Cryptography.X509Certificates;
using LibroLib;
using LibroLib.ConsoleShells;

namespace SelfSignedHttpsListener
{
    // http://stackoverflow.com/questions/11403333/httplistener-with-https-support
    // http://stackoverflow.com/questions/4004/how-do-i-add-ssl-to-a-net-application-that-uses-httplistener-it-will-not-be
    // http://blog.differentpla.net/blog/2013/03/18/using-bouncy-castle-from-net/
    public class MakeCertificateCommand : StandardConsoleCommandBase
    {
        public MakeCertificateCommand()
        {
            AddSetting ("port", "port number the web server should run on. The default is {0}".Fmt (RunWebServerCommand.DefaultPort))
                .IntValue ((portToUse, env) => port = portToUse);           
        }

        public override string CommandId
        {
            get { return "makecert"; }
        }

        public override object Description
        {
            get { return "Generates a self-signed certificate for localhost and registers it for a specific port"; }
        }

        public override int Execute(IConsoleEnvironment env)
        {
            try
            {
                Console.Out.WriteLine("Generating self-signed certificate for localhost...");
                SelfSignedCertificateGenerator generator = new SelfSignedCertificateGenerator ();
                X509Certificate2 cert = generator.GenerateCertificate ("SelfSignedHttpsListener certificate");
                Console.Out.WriteLine ("Generated self-signed certificate for localhost, certificate hash (thumbprint) is {0}", cert.GetCertHashString());

                Console.Out.WriteLine ("Installing the certificate into local machine Personal store...");
                CertificateStoreInstaller installer = new CertificateStoreInstaller ();
                installer.InstallCertificate (cert);

                Console.Out.WriteLine("Binding the certificate to port {0}...", port);
                CertificateBinder binder = new CertificateBinder ();
                binder.BindCertificate (cert, RunWebServerCommand.DefaultPort);

                Console.Out.WriteLine ("DONE!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return 0;
        }

        private int port = RunWebServerCommand.DefaultPort;
    }
}