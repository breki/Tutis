using System.Collections.Generic;

namespace Freude.DocModel
{
    public class DocumentDef : IDocumentElementContainer
    {
        public IList<IDocumentElement> Children
        {
            get { return children; }
        }

        private readonly List<IDocumentElement> children = new List<IDocumentElement>();
    }
}