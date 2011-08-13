using System.Collections.Generic;

namespace OMetaSharp.Examples.Prolog
{
    public interface ISupportUnify
    {
        void Unify(IPrologItem that, IDictionary<string, IPrologItem> env);
    }
}
