using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using log4net;
using Microsoft.CSharp;

namespace FinanceReport.Razor
{
    public class InMemoryRazorEngine
    {
        public static ExecutionResult Execute<TModel> (
            string viewName,
            string razorTemplateText,
            TModel model,
            dynamic viewBag,
            params Assembly[] referenceAssemblies)
        {
            var razorEngineHost = new RazorEngineHost (new CSharpRazorCodeLanguage ());
            razorEngineHost.DefaultNamespace = "FinanceReport";
            razorEngineHost.DefaultClassName = "Template";
            razorEngineHost.NamespaceImports.Add ("System");
            razorEngineHost.NamespaceImports.Add ("FinanceReport");
            razorEngineHost.NamespaceImports.Add ("FinanceReport.DataModel");
            razorEngineHost.NamespaceImports.Add ("FinanceReport.Reporting");
            razorEngineHost.NamespaceImports.Add ("FinanceReport.Reporting.Models");
            razorEngineHost.DefaultBaseClass = typeof (ReportRazorTemplateBase<TModel>).FullName;

            RazorTemplateEngine razorTemplateEngine = new RazorTemplateEngine (razorEngineHost);

            using (StringReader templateReader = new StringReader (razorTemplateText))
            {
                GeneratorResults generatorResult = razorTemplateEngine.GenerateCode (templateReader);

                CompilerParameters compilerParameters = new CompilerParameters ();
                compilerParameters.GenerateInMemory = true;
                compilerParameters.ReferencedAssemblies.Add (typeof (InMemoryRazorEngine).Assembly.Location);
                compilerParameters.ReferencedAssemblies.Add (typeof (SortedList<,>).Assembly.Location);
                if (referenceAssemblies != null)
                    foreach (var referenceAssembly in referenceAssemblies)
                        compilerParameters.ReferencedAssemblies.Add (referenceAssembly.Location);

                CSharpCodeProvider codeProvider = new CSharpCodeProvider ();
                CompilerResults compilerResult = codeProvider.CompileAssemblyFromDom (compilerParameters, generatorResult.GeneratedCode);

                string runtimeResult = null;
                if (compilerResult.Errors.HasErrors)
                {
                    log.ErrorFormat ("View {0} compiling produced {1} errors:", viewName, compilerResult.Errors.Count);
                    foreach (CompilerError error in compilerResult.Errors)
                        log.ErrorFormat (error.ToString ());
                }
                else
                {
                    var compiledTemplateType = compilerResult.CompiledAssembly.GetExportedTypes ().Single ();
                    var compiledTemplate = Activator.CreateInstance (compiledTemplateType);

                    var modelProperty = compiledTemplateType.GetProperty ("Model");
                    modelProperty.SetValue (compiledTemplate, model, null);

                    var viewBagProperty = compiledTemplateType.GetProperty ("ViewBag");
                    viewBagProperty.SetValue (compiledTemplate, viewBag, null);

                    var executeMethod = compiledTemplateType.GetMethod ("Execute");
                    executeMethod.Invoke (compiledTemplate, null);

                    var builderProperty = compiledTemplateType.GetProperty ("OutputBuilder");
                    var outputBuilder = (StringBuilder)builderProperty.GetValue (compiledTemplate, null);
                    runtimeResult = outputBuilder.ToString ();
                }

                return new ExecutionResult (generatorResult, compilerResult, runtimeResult);
            }
        }

        public sealed class ExecutionResult
        {
            public ExecutionResult (GeneratorResults generatorResult, CompilerResults compilerResult, string runtimeResult)
            {
                GeneratorResult = generatorResult;
                CompilerResult = compilerResult;
                RuntimeResult = runtimeResult;
            }

            public GeneratorResults GeneratorResult { get; private set; }
            public CompilerResults CompilerResult { get; private set; }
            public string RuntimeResult { get; private set; }
        }

        private static readonly ILog log = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);
    }
}