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

        public string ExpandTemplate(string templateText, DocumentDef doc)
        {
            RazorEngineCompileSettings razorEngineCompileSettings = new RazorEngineCompileSettings ();
            razorEngineCompileSettings.DefaultNamespace = "Syborg.Tests";
            razorEngineCompileSettings.DefaultClassName = "SyborgTestRazorTemplate";
            razorEngineCompileSettings.NamespaceImports.Add ("System");
            razorEngineCompileSettings.NamespaceImports.Add ("System.Collections");
            razorEngineCompileSettings.NamespaceImports.Add ("System.Collections.Generic");
            razorEngineCompileSettings.DefaultBaseClass = typeof(RazorTemplateBase).FullName;
            razorEngineCompileSettings.ReferenceAssemblies.Add (typeof(HtmlString).Assembly);

            ICompiledRazorTemplate compiledTemplate = razorCompiler.Compile (templateText, razorEngineCompileSettings);
            RazorEngineExecutionSettings executionSettings = new RazorEngineExecutionSettings ();
            return compiledTemplate.Execute (executionSettings);
        }

        private readonly IRazorCompiler razorCompiler;
    }
}