using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.Parsing
{
    [ContractClass(typeof(IFreudeTextParserContract))]
    public interface IFreudeTextParser
    {
        DocumentDef ParseText(string text, ParsingContext context);
    }

    [ContractClassFor(typeof(IFreudeTextParser))]
    internal abstract class IFreudeTextParserContract : IFreudeTextParser
    {
        DocumentDef IFreudeTextParser.ParseText(string text, ParsingContext context)
        {
            Contract.Requires(text != null);
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<DocumentDef>() != null);
            throw new System.NotImplementedException();
        }
    }
}