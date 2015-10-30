using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class DocumentDef : IDocumentElementContainer
    {
        public IList<IDocumentElement> Children
        {
            get { return children; }
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(children != null);
        }

        private readonly List<IDocumentElement> children = new List<IDocumentElement>();
    }
}