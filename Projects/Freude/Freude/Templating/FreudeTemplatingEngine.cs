using System;
using System.Diagnostics.Contracts;
using System.Web;
using Freude.DocModel;
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

        public ICompiledRazorTemplate CompileTemplate(string templateText)
        {
            RazorEngineCompileSettings razorEngineCompileSettings = new RazorEngineCompileSettings ();
            razorEngineCompileSettings.DefaultNamespace = "Freude";
            razorEngineCompileSettings.DefaultClassName = "FreudeRazorTemplate";
            razorEngineCompileSettings.NamespaceImports.Add ("System");
            razorEngineCompileSettings.NamespaceImports.Add ("System.Collections");
            razorEngineCompileSettings.NamespaceImports.Add ("System.Collections.Generic");
            razorEngineCompileSettings.DefaultBaseClass = typeof(RazorTemplateBase).FullName;
            razorEngineCompileSettings.ReferenceAssemblies.Add (typeof(HtmlString).Assembly);

            return razorCompiler.Compile (templateText, razorEngineCompileSettings);
        }

        public string ExpandTemplate (ICompiledRazorTemplate template, DocumentDef doc, string docHtml, FreudeProject project)
        {
            RazorEngineExecutionSettings executionSettings = new RazorEngineExecutionSettings ();
            executionSettings.Properties.Add("Doc", doc);
            executionSettings.Properties.Add("DocHtml", docHtml);
            executionSettings.Properties.Add("Project", project);
            return template.Execute (executionSettings);
        }

        private readonly IRazorCompiler razorCompiler;
    }
}