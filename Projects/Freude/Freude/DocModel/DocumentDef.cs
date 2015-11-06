using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class DocumentDef : IDocumentElementContainer
    {
        public int ChildrenCount
        {
            get { return children.Count; }
        }

        public IList<IDocumentElement> Children
        {
            get { return children; }
        }

        public void AddChild(IDocumentElement child)
        {
            children.Add(child);
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(children != null);
        }

        private readonly List<IDocumentElement> children = new List<IDocumentElement>();
    }
}