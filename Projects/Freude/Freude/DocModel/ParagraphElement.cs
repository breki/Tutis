using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class ParagraphElement : IDocumentElementContainer
    {
        public IList<IDocumentElement> Children
        {
            get { return children; }
        }

        public void Trim()
        {
            if (children.Count <= 0) 
                return;

            TextElement tel = children[0] as TextElement;
            if (tel != null)
                tel.TrimStart();

            tel = children[children.Count - 1] as TextElement;
            if (tel != null)
                tel.TrimEnd();
        }

        public void AddChild(IDocumentElement child)
        {
            children.Add (child);
        }

        [ContractInvariantMethod]
        private void Invariant ()
        {
            Contract.Invariant (children != null);
        }

        private readonly List<IDocumentElement> children = new List<IDocumentElement> ();
    }
}