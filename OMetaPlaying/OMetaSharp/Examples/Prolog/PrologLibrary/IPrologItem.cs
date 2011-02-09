using System.Collections.Generic;

namespace OMetaSharp.Examples.Prolog
{
    public interface IPrologItem
    {
        IPrologItem Rename(string nm);
        IPrologItem Rewrite(IDictionary<string, IPrologItem> env);
    }
}
