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
            IProjectBuilder projectBuilder,
            IFtpSessionFactory ftpSessionFactory)
        {
            Contract.Requires(projectBuilder != null);
            Contract.Requires(ftpSessionFactory != null);

            this.projectBuilder = projectBuilder;
            this.ftpSessionFactory = ftpSessionFactory;
            AddArg ("project build dir", "path to the directory where the project was built").Value ((x, env) => buildDirectory = x);
            AddArg ("server", "server IP or hostname").Value ((x, env) => server = x);
            AddArg ("FTP username", null).Value ((x, env) => userName = x);
            AddArg ("FTP password", null).Value ((x, env) => password = x);
            AddSetting("port", "FTP port number").IntValue((x, env) => port = x);
            AddSetting("remote-dir", "the root remote dir where the project files will be deployed (uploaded)").Value ((x, env) => remoteRootDirectory = x);
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

            using (IFtpSession ftpSession = ftpSessionFactory.CreateSession())
            {
                FtpConnectionData connData = new FtpConnectionData();

                connData.Credentials = new NetworkCredential(userName, password);
                connData.Host = server;
                connData.Port = port;

                log.InfoFormat(CultureInfo.InvariantCulture, "Connecting to the FTP server {0}:{1}...", connData.Host, connData.Port);

                ftpSession.BeginSession(connData);

                PathBuilder buildDirPath = new PathBuilder(buildDirectory);
                foreach (string sourceFileName in projectBuilder.ListBuiltFiles(project))
                {
                    PathBuilder sourceFileNameDebasedBuilder = buildDirPath.DebasePath(sourceFileName, false);
                    PathBuilder destinationFileName;
                    if (remoteRootDirectory != null)
                        destinationFileName = new PathBuilder(remoteRootDirectory).CombineWith(sourceFileNameDebasedBuilder);
                    else
                        destinationFileName = sourceFileNameDebasedBuilder;

                    string destinationFileNameUnixStyle = destinationFileName.ToUnixPath();

                    log.InfoFormat (CultureInfo.InvariantCulture, "Uploading file {0} to {1}...", sourceFileName, destinationFileNameUnixStyle);
                    ftpSession.UploadFile (sourceFileName, destinationFileNameUnixStyle);
                }
            }

            return 0;
        }

        private readonly IProjectBuilder projectBuilder;
        private readonly IFtpSessionFactory ftpSessionFactory;
        private string buildDirectory;
        private string server;
        private string userName;
        private string password;
        private string remoteRootDirectory;
        private int? port;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}