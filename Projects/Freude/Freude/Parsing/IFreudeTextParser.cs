using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.Parsing
{
    [ContractClass(typeof(IFreudeTextParserContract))]
    public interface IFreudeTextParser
    {
        DocumentDef ParseText(string text, out ParsingContext context);
    }

    [ContractClassFor(typeof(IFreudeTextParser))]
    internal abstract class IFreudeTextParserContract : IFreudeTextParser
    {
        DocumentDef IFreudeTextParser.ParseText(string text, out ParsingContext context)
        {
            Contract.Requires(text != null);
            Contract.Ensures(Contract.Result<DocumentDef>() != null);
            Contract.Ensures(Contract.ValueAtReturn(out context) != null);
            throw new System.NotImplementedException();
        }
    }
}