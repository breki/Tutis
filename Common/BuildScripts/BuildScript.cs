using System;
using Flubu;
using Flubu.Builds;
using Flubu.Packaging;
using Flubu.Targeting;

//css_ref Flubu.dll;
//css_ref Flubu.Contrib.dll;
//css_ref log4net.dll;
//css_ref ICSharpCode.SharpZipLib.dll;

namespace BuildScripts
{
    public class BuildScript
    {
        public static int Main(string[] args)
        {
            TargetTree targetTree = new TargetTree();
            BuildTargets.FillBuildTargets(targetTree);

            int testsRunCounter = 0;

            targetTree.AddTarget("rebuild")
                .SetDescription("Compiles the code and runs tests.")
                .SetAsDefault().DependsOn("compile", "fxcop", "tests");

            targetTree.AddTarget ("release")
                .SetDescription ("Runs the complete release build and publishes it. The build number must be provided as a command line argument.")
                .DependsOn ("compile", "fxcop", "tests", "package.treasurechest");

            targetTree.GetTarget ("fetch.build.version")
                .Do (TargetFetchBuildVersion);

            targetTree.AddTarget("tests")
                .SetDescription("Runs tests on the project")
                .Do (r =>
                            {
                                BuildTargets.TargetRunTests(r, "Clavis.Tests", null, ref testsRunCounter); 
                                BuildTargets.TargetRunTests(r, "TreasureChest.Tests", null, ref testsRunCounter); 
                            }).DependsOn ("load.solution");

            targetTree.AddTarget ("tests.debug")
                .SetDescription ("Runs tests on the project")
                .Do (r =>
                {
                    r.Properties.Set (BuildProps.BuildConfiguration, "Debug");
                    BuildTargets.TargetRunTests (r, "Clavis.Tests", null, ref testsRunCounter);
                    BuildTargets.TargetRunTests (r, "TreasureChest.Tests", null, ref testsRunCounter);
                }).DependsOn ("load.solution");

            //targetTree.AddTarget("docs")
            //    .SetDescription("Builds documentation and uploads it to Maperitive server if the build is run under Hudson")
            //    .Do(x => TargetDocs(x, false));

            //targetTree.AddTarget("docs.force.update")
            //    .SetDescription("Builds Maperitive documentation and uploads it to Maperitive server")
            //    .Do(x => TargetDocs(x, true));

            targetTree.AddTarget ("package.treasurechest")
                .SetDescription("Packages TreasureChest binaries into ZIP file")
                .Do (TargetPackageTreasureChest).DependsOn ("load.solution", "fetch.build.version");

            using (TaskSession session = new TaskSession(new SimpleTaskContextProperties(), args, targetTree))
            {
                BuildTargets.FillDefaultProperties(session);
                session.Start(BuildTargets.OnBuildFinished);

                session.AddLogger(new MulticoloredConsoleLogger(Console.Out));

                session.Properties.Set(BuildProps.CompanyName, "igorbrejc.net");
                session.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2011 Igor Brejc.");
                session.Properties.Set(BuildProps.ProductId, "Common");
                session.Properties.Set (BuildProps.ProductName, "Common");
                session.Properties.Set (BuildProps.SolutionFileName, "Common.sln");
                session.Properties.Set(BuildProps.VersionControlSystem, VersionControlSystem.Mercurial);

                try
                {
                    // actual run
                    if (args.Length == 0)
                        targetTree.RunTarget(session, targetTree.DefaultTarget.TargetName);
                    else
                    {
                        string targetName = args[0];
                        if (false == targetTree.HasTarget(targetName))
                        {
                            session.WriteError("ERROR: The target '{0}' does not exist", targetName);
                            targetTree.RunTarget(session, "help");
                            return 2;
                        }

                        targetTree.RunTarget(session, args[0]);
                    }

                    session.Complete();

                    return 0;
                }
                catch (TaskExecutionException)
                {
                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return 1;
                }
            }
        }

        private static void TargetFetchBuildVersion (ITaskContext context)
        {
            Version version = BuildTargets.FetchBuildVersionFromFile (context);
            version = new Version (version.Major, version.Minor, BuildTargets.FetchBuildNumberFromFile (context));
            context.Properties.Set (BuildProps.BuildVersion, version);
            context.WriteInfo ("The build version will be {0}", version);
        }

        private static void TargetPackageTreasureChest(ITaskContext context)
        {
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister ();
            ICopier copier = new Copier (context);
            IZipper zipper = new Zipper (context);

            StandardPackageDef packageDef = new StandardPackageDef ("TreasureChest", context, directoryFilesLister);

            FilterCollection mainFilter = new FilterCollection ();
            mainFilter
                .Add (new NegativeFilter (new RegexFileFilter (@"(.*\.(dll|exe|exe.config|config|sh|ico))$")))
                .Add (new RegexFileFilter (@".*vshost.exe.*"));
            packageDef.AddFolderSource ("main", new FullPath ("TreasureChest/bin/Release"), true, mainFilter);

            FullPath buildPackagesDir = new FullPath (context.Properties[BuildProps.BuildDir]).CombineWith ("packages");
            CopyProcessor copyProcessor = new CopyProcessor (
                context,
                copier,
                buildPackagesDir);
            copyProcessor
                .AddTransformation("main", new LocalPath(String.Empty));

            IPackageDef copiedPackageDef = copyProcessor.Process (packageDef);

            Version buildVersion = context.Properties.Get<Version> (BuildProps.BuildVersion);

            ZipProcessor zipProcessor = new ZipProcessor (
                context,
                zipper,
                buildPackagesDir.AddFileName ("TreasureChest-{0}.zip", buildVersion),
                buildPackagesDir,
                null,
                "main");
            zipProcessor.Process (copiedPackageDef);

            BuildTargets.IncrementBuildNumberInFile (context);
        }

        //private static void TargetDocs(ITaskContext context, bool forceUpload)
        //{
        //    string docsSourceDir = @"Docs\Maperitive\Manual";

        //    FullPath buildPackagesDir = new FullPath(context.Properties[BuildProps.BuildDir])
        //        .CombineWith("packages");

        //    RunProgramTask runTask = new RunProgramTask(@"lib\Quiki\Quiki.Console.exe");
        //    runTask
        //        .AddArgument("html")
        //        .AddArgument(@"-i={0}", docsSourceDir)
        //        .AddArgument(@"-t={0}\template.vm.html", docsSourceDir)
        //        .AddArgument(@"-o={0}\docs", buildPackagesDir)
        //        .AddArgument(@"-lf=Maperitive.css")
        //        .AddArgument(@"-lf=Images\BannerSmaller.jpg")
        //        .AddArgument(@"-lf=Images\BannerFooter.jpg");

        //    //HudsonExternsionsBuildRunner extendedRunner = (HudsonExternsionsBuildRunner)runner;
        //    //if (extendedRunner.IsRunningUnderHudson)
        //    //    runner.ProgramRunner.AddArgument(@"-p=MaperitiveVersion:{0}", extendedRunner.BuildVersion);
        //    //else
        //    //    runner.ProgramRunner.AddArgument(@"-p=MaperitiveVersion:CURRENT BUILD");

        //    //if (forceUpload)// || ShouldBuildBePublished())
        //    //{
        //    //    string ftpUser;
        //    //    string ftpPassword;

        //    //    if (ReadFtpUserAndPasswordFromFile(runner, out ftpUser, out ftpPassword))
        //    //    {
        //    //        runner.ProgramRunner
        //    //            .AddArgument("-ftpserver=ftp.igorbrejc.net")
        //    //            .AddArgument("-ftpuser={0}", ftpUser)
        //    //            .AddArgument("-ftppwd={0}", ftpPassword)
        //    //            .AddArgument("-ftpdir=/public_html/maperitive/docs/manual");
        //    //    }
        //    //    else
        //    //        runner.Log("Since we could not fetch FTP credentials, nothing will be uploaded");
        //    //}

        //    runTask.Execute(context);
        //}
    }
}