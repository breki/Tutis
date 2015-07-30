using System.Collections.Generic;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class DocumentDef : IDocumentElementContainer
    {
        public IList<IDocumentElement> Children
        {
            get { return children; }
        }

        private List<IDocumentElement> children = new List<IDocumentElement>();
    }
}