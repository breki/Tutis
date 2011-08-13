using System;
using Flubu;
using Flubu.Builds;
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
                .DependsOn ("compile", "fxcop", "tests", "docs", "package");

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

            targetTree.AddTarget("package")
                .SetDescription("Packages all the build products into ZIP files")
                .Do (TargetPackage).DependsOn ("load.solution", "fetch.build.version");

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

        private static void TargetPackage(ITaskContext context)
        {
            //IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            //ICopier copier = new Copier(context);
            //IZipper zipper = new Zipper(context);

            //StandardPackageDef packageDef = new StandardPackageDef("Maperitive", context, directoryFilesLister);

            //FilterCollection mainFilter = new FilterCollection();
            //mainFilter
            //    .Add(new NegativeFilter(new RegexFileFilter(@"(.*\.(dll|exe|exe.config|config|sh|ico))$")))
            //    .Add(new RegexFileFilter(@".*vshost.exe.*"));
            //packageDef.AddFolderSource("main", new FullPath("bin/Release/Maperitive"), true, mainFilter);

            //FilesList otherFiles = new FilesList("other files");
            //otherFiles.AddFile(new PackagedFileInfo(new FileFullPath("bin/Release/Maperitive/ReadMe.txt")));
            //otherFiles.AddFile(new PackagedFileInfo(new FileFullPath("bin/Release/Maperitive/ReleaseHistory.txt")));

            //packageDef.AddFilesSource(otherFiles);

            //packageDef.AddFolderSource("rules", new FullPath("bin/Release/Maperitive/Rules"), true, new NegativeFilter(new RegexFileFilter(@".*\.mrules$")));
            //packageDef.AddFolderSource("scripts", new FullPath("bin/Release/Maperitive/Scripts"), true, new NegativeFilter(new RegexFileFilter(@".*\.mscript$")));
            //packageDef.AddFolderSource("textures", new FullPath("Textures"), true, new RegexFileFilter(@"\.svn"));
            //packageDef.AddFolderSource("icons.sjjb", new FullPath("Icons/SJJB"), true, new RegexFileFilter(@"\.svn"));
            //packageDef.AddFolderSource("icons", new FullPath("Icons/application"), true, new RegexFileFilter(@"\.svn"));
            //packageDef.AddFolderSource("samples", new FullPath("Data/MaperitiveSamples"), true, new RegexFileFilter(@"\.svn"));

            //packageDef.AddFilesSource(new SingleFileSource("license.Maperitive", new FileFullPath("Licenses/Maperitive License.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.GetText", new FileFullPath("Licenses/LGPLv3.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.Cairo", new FileFullPath("Licenses/LGPLv3.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.GLib", new FileFullPath("Licenses/LGPLv3.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.Pango", new FileFullPath("Licenses/LGPLv3.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.QuickGraph", new FileFullPath("Licenses/MsPL.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.SharpZipLib", new FileFullPath(@"lib\ICSharpCode.SharpZipLib.2.0\COPYING.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.log4net", new FileFullPath(@"lib\log4net-1.2.10\log4net.LICENSE.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.PowerCollections", new FileFullPath(@"lib\PowerCollections\Binaries\License.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.Mono", new FileFullPath(@"lib\Cairo\Mono-LICENSE")));
            //packageDef.AddFilesSource(new SingleFileSource("license.JsonExSerializer", new FileFullPath(@"lib\JsonExSerializer\license.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.DotNetZip", new FileFullPath(@"lib\DotNetZip\license.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.protobuf", new FileFullPath(@"lib\protobuf-net\licence.txt")));
            //packageDef.AddFilesSource(new SingleFileSource("license.IronPython", new FileFullPath(@"lib\IronPython\License.html")));

            //FilesList iconLicenseFiles = new FilesList("licenses.icons");
            //iconLicenseFiles.AddFile(new PackagedFileInfo(new FileFullPath("Licenses/fugue Icons License.txt")));
            //iconLicenseFiles.AddFile(new PackagedFileInfo(new FileFullPath("Licenses/fugue Icons License.txt")));
            //iconLicenseFiles.AddFile(new PackagedFileInfo(new FileFullPath("Licenses/Silk Icons License.txt")));
            //packageDef.AddFilesSource(iconLicenseFiles);

            //FullPath buildPackagesDir = new FullPath(context.Properties[BuildProps.BuildDir]).CombineWith("packages");
            //CopyProcessor copyProcessor = new CopyProcessor(
            //    context,
            //    copier,
            //    buildPackagesDir.CombineWith("Maperitive"));
            //copyProcessor
            //    .AddTransformation("main", new LocalPath(String.Empty))
            //    .AddTransformation("other files", new LocalPath(String.Empty))
            //    .AddTransformation("rules", new LocalPath("Rules"))
            //    .AddTransformation("scripts", new LocalPath("Scripts"))
            //    .AddTransformation("textures", new LocalPath("Textures"))
            //    .AddTransformation("icons", new LocalPath("Icons"))
            //    .AddTransformation("icons.sjjb", new LocalPath("Icons/SJJB"))
            //    .AddTransformation("samples", new LocalPath("Samples"))
            //    .AddTransformation("license.Maperitive", new LocalPath("License.txt"))
            //    .AddTransformation("license.GetText", new LocalPath("licenses/license-GetText.txt"))
            //    .AddTransformation("license.Cairo", new LocalPath("licenses/license-Cairo.txt"))
            //    .AddTransformation("license.GLib", new LocalPath("licenses/license-GLib.txt"))
            //    .AddTransformation("license.Pango", new LocalPath("licenses/license-Pango.txt"))
            //    .AddTransformation("license.QuickGraph", new LocalPath("licenses/license-QuickGraph.txt"))
            //    .AddTransformation("license.SharpZipLib", new LocalPath("licenses/license-SharpZipLib.txt"))
            //    .AddTransformation("license.log4net", new LocalPath("licenses/license-log4net.txt"))
            //    .AddTransformation("license.PowerCollections", new LocalPath("licenses/license-PowerCollections.txt"))
            //    .AddTransformation("license.Mono", new LocalPath("licenses/license-Mono.txt"))
            //    .AddTransformation("license.JsonExSerializer", new LocalPath("licenses/license-JsonExSerializer.txt"))
            //    .AddTransformation("license.DotNetZip", new LocalPath("licenses/license-DotNetZip.txt"))
            //    .AddTransformation("license.protobuf", new LocalPath("licenses/license-protobuf-net.txt"))
            //    .AddTransformation("license.IronPython", new LocalPath("licenses/license-IronPython.txt"))
            //    .AddTransformation("licenses.icons", new LocalPath("licenses"));

            //IPackageDef copiedPackageDef = copyProcessor.Process(packageDef);

            //Version buildVersion = context.Properties.Get<Version>(BuildProps.BuildVersion); 

            //ZipProcessor zipProcessor = new ZipProcessor(
            //    context,
            //    zipper,
            //    buildPackagesDir.AddFileName("Maperitive-{0}.zip", buildVersion),
            //    buildPackagesDir,
            //    null,
            //    "main",
            //    "other files",
            //    "rules",
            //    "scripts",
            //    "textures",
            //    "icons",
            //    "icons.sjjb",
            //    "samples",
            //    "license.Maperitive",
            //    "license.GetText",
            //    "license.Cairo",
            //    "license.GLib",
            //    "license.Pango",
            //    "license.QuickGraph",
            //    "license.SharpZipLib",
            //    "license.log4net",
            //    "license.PowerCollections",
            //    "license.Mono",
            //    "license.JsonExSerializer",
            //    "license.DotNetZip",
            //    "license.protobuf",
            //    "license.IronPython",
            //    "licenses.icons");
            //zipProcessor.Process(copiedPackageDef);

            //BuildTargets.IncrementBuildNumberInFile (context);
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