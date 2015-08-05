using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Reflection;
using Brejc.Common.Console;
using Brejc.Common.FileSystem;
using Brejc.Common.Ftp;
using Freude.DocModel;
using Freude.ProjectServices;
using log4net;

namespace Freude.Commands
{
    public class DeployCommand : StandardConsoleCommandBase
    {
        public DeployCommand(
            IFileSystem fileSystem,
            IProjectBuilder projectBuilder,
            IFtpChannelFactory ftpChannelFactory, 
            IFtpCommunicator ftpCommunicator)
        {
            Contract.Requires(fileSystem != null);
            Contract.Requires(projectBuilder != null);
            Contract.Requires(ftpChannelFactory != null);
            Contract.Requires(ftpCommunicator != null);

            this.fileSystem = fileSystem;
            this.projectBuilder = projectBuilder;
            this.ftpChannelFactory = ftpChannelFactory;
            this.ftpCommunicator = ftpCommunicator;
            AddArg ("project build dir", "path to the directory where the project was built").Value ((x, env) => buildDirectory = x);
            AddArg ("server", "server IP or hostname").Value ((x, env) => server = x);
            AddArg ("FTP username", null).Value ((x, env) => userName = x);
            AddArg ("FTP password", null).Value ((x, env) => password = x);
            AddSetting("port", "FTP port number").IntValue((x, env) => port = x);
        }

        public override string CommandId
        {
            get { return "deploy"; }
        }

        public override object Description
        {
            get { return "deploy the built project on a web server using FTP protocol"; }
        }

        public override int Execute(IConsoleEnvironment env)
        {
            FreudeProject project = new FreudeProject();
            project.BuildDir = buildDirectory;

            using (IFtpSession ftpSession = new FtpSession(ftpChannelFactory, ftpCommunicator, fileSystem))
            {
                FtpConnectionData connData = new FtpConnectionData();

                connData.Credentials = new NetworkCredential(userName, password);
                connData.Host = server;
                connData.Port = port;

                log.InfoFormat(CultureInfo.InvariantCulture, "Connecting to the FTP server {0}:{1}...", connData.Host, connData.Port);

                ftpSession.BeginSession(connData);

                foreach (string sourceFileName in projectBuilder.ListBuiltFiles(project))
                {
                    log.InfoFormat (CultureInfo.InvariantCulture, "Uploading file {0} to {1}...", sourceFileName, destFile);
                    ftpSession.UploadFile (sourceFileName, destFile);
                    
                }

            }
        }

        private readonly IFileSystem fileSystem;
        private readonly IProjectBuilder projectBuilder;
        private readonly IFtpChannelFactory ftpChannelFactory;
        private readonly IFtpCommunicator ftpCommunicator;
        private string buildDirectory;
        private string server;
        private string userName;
        private string password;
        private int? port;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}