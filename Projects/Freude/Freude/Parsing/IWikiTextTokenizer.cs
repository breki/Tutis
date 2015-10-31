using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.Parsing
{
    [ContractClass(typeof(WikiTextTokenizerContract))]
    public interface IWikiTextTokenizer
    {
        IList<WikiTextToken> TokenizeWikiText(string wikiText);
    }

    [ContractClassFor(typeof(IWikiTextTokenizer))]
    internal abstract class WikiTextTokenizerContract : IWikiTextTokenizer
    {
        IList<WikiTextToken> IWikiTextTokenizer.TokenizeWikiText(string wikiText)
        {
            Contract.Requires(wikiText != null);
            Contract.Ensures(Contract.Result<IList<WikiTextToken>>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<IList<WikiTextToken>>(), x => x != null));

            throw new System.NotImplementedException();
        }
    }
}