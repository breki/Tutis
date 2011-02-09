using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace OMetaSharp
{
    /// <summary>
    /// The whole purpose of this program is to compile grammars and run tests in the right order
    /// and to use the right DLLs in the original directories so that debugging works.
    /// </summary>
    /// <remarks>
    /// It is mainly for double checking things before checking-in something to CodePlex.
    /// In addition, it is important that everything builds before you run this.
    /// </remarks>
    public static class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            CompileOMetaCSGrammars();
            CompileExampleGrammars();

            TestRuntime();
            TestOMetaCS();
            TestExamples();

            // NOTE: Running this again should prove that things bootstrapped ok.
        }

        private static void CompileRuntimeGrammars()
        {
            CompileGrammarsInRoot(OMetaFolders.Runtime);
        }

        private static void PerformActionForSubdirsInRoot(string root, Action<string> subdirAction)
        {
            foreach (string currentDirectory in Directory.GetDirectories(root))
            {
                subdirAction(currentDirectory);
            }
        }

        private static void CompileGrammarsInRoot(string root)
        {
            PerformActionForSubdirsInRoot(root, d => CompileGrammarsIn(d));            
        }

        private static void PerformActionIn<T>(string currentDirectory, Action<T> itemTypeAction) where T:class
        {
            string singleDirectoryName = Path.GetFileName(currentDirectory);

            var debugBinDir = Path.Combine(currentDirectory, @"bin\Debug\");
            if (!Directory.Exists(debugBinDir))
            {
                return;
            }

            string exeName = Path.Combine(debugBinDir, singleDirectoryName + ".exe");

            if (!File.Exists(exeName))
            {
                return;
            }
            
            var a = Assembly.LoadFile(exeName);
            // HACK
            Grammars.GrammarResolutionAssembly = a;
            var entryPointType = a.EntryPoint.DeclaringType;
            if (!entryPointType.GetInterfaces().Contains(typeof(T)))
            {
                return;
            }

            T itemInstance = Activator.CreateInstance(entryPointType) as T;

            if (itemInstance == null)
            {
                return;
            }
            itemTypeAction(itemInstance);            
        }

        private static void CompileGrammarsIn(string currentDirectory)
        {
            PerformActionIn(currentDirectory, (ICompileGrammars c) => c.CompileGrammars());            
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // HACK
            if (args.Name.StartsWith("OMetaSharp.OMetaCS"))
            {
                var path = OMetaFolders.HostLanguages(@"OMetaCS\OMetaCS\bin\Debug\OMetaSharp.OMetaCS.dll");
                return Assembly.LoadFile(path);
            }
            else if (args.Name.StartsWith("OMetaCSUnitTests"))
            {
                var path = OMetaFolders.HostLanguages(@"OMetaCS\OMetaCSUnitTests\bin\Debug\OMetaCSUnitTests.exe");
                return Assembly.LoadFile(path);
            }
            else if (args.Name.StartsWith("OMetaSharpUnitTests"))
            {
                var path = Path.Combine(OMetaFolders.Runtime,@"OMetaSharpUnitTests\bin\Debug\OMetaSharpUnitTests.exe");
                return Assembly.LoadFile(path);
            }
            else if (args.Name.StartsWith("Prolog.Library,"))
            {
                var path = Path.Combine(OMetaFolders.Examples, @"Prolog\PrologLibrary\bin\Debug\Prolog.Library.dll");
                return Assembly.LoadFile(path);
            }

            return null;         
        }        
        
        private static void CompileExampleGrammars()
        {
            foreach (var currentExampleRoot in Directory.GetDirectories(OMetaFolders.Examples))
            {
                foreach (var exampleProject in Directory.GetDirectories(currentExampleRoot))
                {
                    CompileGrammarsIn(exampleProject);
                }
            }
        }

        private static void CompileOMetaCSGrammars()
        {
            CompileGrammarsInRoot(OMetaFolders.HostLanguages("OMetaCS"));
        }

        private static void TestRuntime()
        {
            PerformActionForSubdirsInRoot(OMetaFolders.Runtime, s => PerformActionIn<IPerformTests>(s, item => item.PerformTests()));
        }

        private static void TestOMetaCS()
        {
            PerformActionForSubdirsInRoot(OMetaFolders.HostLanguages("OMetaCS"), s => PerformActionIn<IPerformTests>(s, item => item.PerformTests()));
        }

        private static void TestExamples()
        {
            PerformActionForSubdirsInRoot(
                OMetaFolders.Examples,
                        example => PerformActionForSubdirsInRoot(example, 
                        s => PerformActionIn<IPerformTests>(s, item => item.PerformTests()))
                );
        }
    }
}
