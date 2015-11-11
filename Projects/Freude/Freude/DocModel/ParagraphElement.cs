using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class ParagraphElement : IDocumentElementContainer
    {
        public ParagraphElement (ParagraphType type, int indentation)
        {
            Contract.Requires(indentation >= 0);

            this.type = type;
            this.indentation = indentation;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public ParagraphType Type
        {
            get { return type; }
        }

        public int Indentation
        {
            get { return indentation; }
        }

        public int ChildrenCount
        {
            get { return Children.Count; }
        }

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
        private readonly ParagraphType type;
        private readonly int indentation;

        public enum ParagraphType
        {
            Regular,
            Bulleted,
            Numbered
        }
    }
}