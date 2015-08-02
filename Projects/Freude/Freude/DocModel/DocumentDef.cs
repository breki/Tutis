using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class DocumentDef : IDocumentElementContainer
    {
        public IList<IDocumentElement> Children
        {
            get
            {
                Contract.Ensures (Contract.Result<System.Collections.Generic.IList<Freude.DocModel.IDocumentElement>> () != null); 
                return children;
            }
        }

        private readonly List<IDocumentElement> children = new List<IDocumentElement>();
    }
}