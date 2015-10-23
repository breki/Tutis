using System.CodeDom.Compiler;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Web;
using System.Web.Razor.Parser.SyntaxTree;
using Freude.DocModel;
using log4net;
using Syborg.Razor;

namespace Freude.Templating
{
    public class FreudeTemplatingEngine : IFreudeTemplatingEngine
    {
        public FreudeTemplatingEngine(IRazorCompiler razorCompiler)
        {
            Contract.Requires(razorCompiler != null);
            this.razorCompiler = razorCompiler;
        }

        public ICompiledRazorTemplate CompileTemplate (string templateName, string templateFileName, string templateText)
        {
            RazorEngineCompileSettings razorEngineCompileSettings = new RazorEngineCompileSettings ();
            razorEngineCompileSettings.DefaultNamespace = "Freude";
            razorEngineCompileSettings.DefaultClassName = "FreudeRazorTemplate";
            razorEngineCompileSettings.NamespaceImports.Add ("System");
            razorEngineCompileSettings.NamespaceImports.Add ("System.Collections");
            razorEngineCompileSettings.NamespaceImports.Add ("System.Collections.Generic");
            razorEngineCompileSettings.DefaultBaseClass = typeof(RazorTemplateBase).FullName;
            razorEngineCompileSettings.ReferenceAssemblies.Add (typeof(HtmlString).Assembly);
            razorEngineCompileSettings.ReferenceAssemblies.Add (typeof(FreudeRazorTemplateBase).Assembly);

            try
            {
                return razorCompiler.Compile (templateText, razorEngineCompileSettings);
            }
            catch (RazorException ex)
            {
                LogTemplateErrors(ex, templateName, templateFileName);
                throw;
            }
        }

        public string ExpandTemplate (ICompiledRazorTemplate template, DocumentDef doc, string docHtml, FreudeProject project)
        {
            RazorEngineExecutionSettings executionSettings = new RazorEngineExecutionSettings ();
            executionSettings.Properties.Add("Doc", doc);
            executionSettings.Properties.Add("DocHtml", docHtml);
            executionSettings.Properties.Add("Project", project);
            return template.Execute (executionSettings);
        }

        private static void LogTemplateErrors (RazorException ex, string templateName, string templateFileName)
        {
            if (!ex.GeneratorResults.Success)
            {
                log.ErrorFormat("Template parse errors in '{0}' Razor template:", templateFileName);
                foreach (RazorError error in ex.GeneratorResults.ParserErrors)
                    log.Error(error);
            }
            else if (ex.CompilerResults.Errors.HasErrors)
            {
                log.ErrorFormat ("Compile errors in '{0}' Razor template:", templateFileName);
                foreach (CompilerError error in ex.CompilerResults.Errors)
                    log.Error(error);
            }
        }

        private readonly IRazorCompiler razorCompiler;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}