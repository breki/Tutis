using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.HtmlGenerating
{
    [ContractClass(typeof(IHtmlGeneratorContract))]
    public interface IHtmlGenerator
    {
        string GenerateHtml(DocumentDef doc);
    }

    [ContractClassFor(typeof(IHtmlGenerator))]
    internal abstract class IHtmlGeneratorContract : IHtmlGenerator
    {
        string IHtmlGenerator.GenerateHtml(DocumentDef doc)
        {
            Contract.Requires(doc != null);
            Contract.Ensures(Contract.Result<string>() != null);

            throw new System.NotImplementedException();
        }
    }
}