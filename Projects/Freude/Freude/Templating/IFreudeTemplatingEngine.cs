using System.Diagnostics.Contracts;
using Freude.DocModel;
using Syborg.Razor;

namespace Freude.Templating
{
    [ContractClass(typeof(IFreudeTemplatingEngineContract))]
    public interface IFreudeTemplatingEngine
    {
        ICompiledRazorTemplate CompileTemplate(string templateText);
        string ExpandTemplate (ICompiledRazorTemplate template, DocumentDef doc, string docHtml, FreudeProject project);
    }

    [ContractClassFor(typeof(IFreudeTemplatingEngine))]
    internal abstract class IFreudeTemplatingEngineContract : IFreudeTemplatingEngine
    {
        public ICompiledRazorTemplate CompileTemplate(string templateText)
        {
            Contract.Requires(templateText != null);
            Contract.Ensures(Contract.Result<ICompiledRazorTemplate>() != null);

            throw new System.NotImplementedException();
        }

        string IFreudeTemplatingEngine.ExpandTemplate (ICompiledRazorTemplate template, DocumentDef doc, string docHtml, FreudeProject project)
        {
            Contract.Requires(template != null);
            Contract.Requires(doc != null);
            Contract.Requires(docHtml != null);
            Contract.Requires(project != null);
            Contract.Ensures (Contract.Result<string> () != null);

            throw new System.NotImplementedException();
        }
    }
}