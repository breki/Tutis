using System;
using Brejc.Common.Console;
using Brejc.Common.Ftp;
using Freude.Commands;
using Freude.ProjectServices;
using NUnit.Framework;
using Rhino.Mocks;

namespace Freude.Tests.CommandsTests
{
    public class DeployCommandTests
    {
        [Test]
        public void TestDeployingAProjectWithSpecifyingRemoteDir()
        {
            IFtpSession ftpSession = MockRepository.GenerateMock<IFtpSession>();
            ftpSessionFactory.Stub(x => x.CreateSession()).Return(ftpSession);

            projectBuilder.Stub(x => x.ListBuiltFiles(null))
                .IgnoreArguments().Return(new[] { @"builds\somewhere\file1.txt", @"builds\somewhere\dir2\file2" });

            ftpSession.Expect(x => x.BeginSession(null))
                .IgnoreArguments()
                .Callback(new Func<FtpConnectionData, bool>(x => x.Host == Server && x.Port == null && x.Credentials.UserName == "user" && x.Credentials.Password == "password"));
            ftpSession.Expect (x => x.UploadFile (@"builds\somewhere\file1.txt", "somewhere/remote/file1.txt"));
            ftpSession.Expect (x => x.UploadFile (@"builds\somewhere\dir2\file2", "somewhere/remote/dir2/file2"));

            cmd.ParseArgs(consoleEnv, new[] { BuildDir, Server, "user", "password", "-remote-dir=somewhere/remote" });
            cmd.Execute(consoleEnv);

            ftpSession.VerifyAllExpectations();
        }

        [Test]
        public void TestDeployingAProjectWithoutSpecifyingRemoteDir()
        {
            IFtpSession ftpSession = MockRepository.GenerateMock<IFtpSession>();
            ftpSessionFactory.Stub(x => x.CreateSession()).Return(ftpSession);

            projectBuilder.Stub(x => x.ListBuiltFiles(null))
                .IgnoreArguments().Return(new[] { @"builds\somewhere\file1.txt", @"builds\somewhere\dir2\file2" });

            ftpSession.Expect (x => x.UploadFile (@"builds\somewhere\file1.txt", "file1.txt"));
            ftpSession.Expect (x => x.UploadFile (@"builds\somewhere\dir2\file2", "dir2/file2"));

            cmd.ParseArgs(consoleEnv, new[] { BuildDir, Server, "user", "password" });
            cmd.Execute(consoleEnv);

            ftpSession.VerifyAllExpectations();
        }

        [SetUp]
        public void Setup()
        {
            projectBuilder = MockRepository.GenerateStub<IProjectBuilder>();
            ftpSessionFactory = MockRepository.GenerateMock<IFtpSessionFactory>();

            consoleEnv = new ConsoleShell ("x");

            cmd = new DeployCommand(projectBuilder, ftpSessionFactory);
        }

        private DeployCommand cmd;
        private IProjectBuilder projectBuilder;
        private IFtpSessionFactory ftpSessionFactory;
        private IConsoleEnvironment consoleEnv;
        private const string BuildDir = @"builds\somewhere";
        private const string Server = @"google.com";
    }
}