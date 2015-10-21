using System;
using Flubu;
using Flubu.Builds;
using Flubu.Builds.Tasks.AnalysisTasks;
using Flubu.Builds.VSSolutionBrowsing;
using Flubu.Targeting;
using Flubu.Tasks.Processes;

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

            targetTree.AddTarget("rebuild")
                .SetDescription("Builds the product, packages it, deploys and tests it on a local instance")
                .DependsOn("compile", "dupfinder", "tests")
                .SetAsDefault();

            targetTree.GetTarget ("fetch.build.version")
                .Do (TargetFetchBuildVersion);

            targetTree.AddTarget("tests")
                .SetDescription("Runs tests on the project")
                .Do (r =>
                    {
                        TargetRunTests(r, "Freude.Tests");
                    }).DependsOn ("load.solution");

            targetTree.AddTarget("dupfinder")
                .SetDescription("Runs R# dupfinder to find code duplicates")
                .Do(TargetDupFinder);

            using (TaskSession session = new TaskSession(new SimpleTaskContextProperties(), args, targetTree))
            {
                BuildTargets.FillDefaultProperties(session);
                session.Start(BuildTargets.OnBuildFinished);

                session.AddLogger(new MulticoloredConsoleLogger(Console.Out));

                session.Properties.Set(BuildProps.CompanyName, "Igor Brejc");
                session.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2015 Igor Brejc");
                session.Properties.Set(BuildProps.ProductId, "Freude");
                session.Properties.Set(BuildProps.ProductName, "Freude");
                session.Properties.Set(BuildProps.SolutionFileName, "Freude.sln");
                session.Properties.Set(BuildProps.TargetDotNetVersion, FlubuEnvironment.Net40VersionNumber);
                session.Properties.Set(BuildProps.VersionControlSystem, VersionControlSystem.Mercurial);
                session.Properties.Set(BuildProps.FxcopDir, "Microsoft Fxcop 10.0");

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
            context.Properties.Set (BuildProps.BuildVersion, version);
            context.WriteInfo ("The build version will be {0}", version);
        }

        private static void TargetRunTests(ITaskContext context, string projectName)
        {
            VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);
            string buildConfiguration = context.Properties.Get<string>(BuildProps.BuildConfiguration);

            VSProjectWithFileInfo project =
                (VSProjectWithFileInfo)solution.FindProjectByName(projectName);
            FileFullPath projectTarget = project.ProjectDirectoryPath.CombineWith(project.GetProjectOutputPath(buildConfiguration))
                .AddFileName("{0}.dll", project.ProjectName);

            RunProgramTask task = new RunProgramTask(
                @"packages\NUnit.Runners.2.6.4\tools\nunit-console-x86.exe")
                .AddArgument(projectTarget.ToString())
                .AddArgument("/labels")
                .AddArgument("/trace=Verbose")
                .AddArgument("/nodots");
            task.Execute(context);
        }

        private static void TargetDupFinder(ITaskContext context)
        {
            RunDupFinderAnalysisTask task = new RunDupFinderAnalysisTask();
            task.Execute(context);
        }
    }
}