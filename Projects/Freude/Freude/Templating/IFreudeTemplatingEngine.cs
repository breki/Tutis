using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.Templating
{
    [ContractClass(typeof(IFreudeTemplatingEngineContract))]
    public interface IFreudeTemplatingEngine
    {
        string ExpandTemplate(string templateText, DocumentDef doc);
    }

    [ContractClassFor(typeof(IFreudeTemplatingEngine))]
    internal abstract class IFreudeTemplatingEngineContract : IFreudeTemplatingEngine
    {
        string IFreudeTemplatingEngine.ExpandTemplate(string templateText, DocumentDef doc)
        {
            Contract.Requires(templateText != null);
            Contract.Requires(doc != null);
            Contract.Ensures (Contract.Result<string> () != null);

            throw new System.NotImplementedException();
        }
    }
}