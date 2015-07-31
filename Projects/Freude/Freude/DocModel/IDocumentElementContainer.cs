using System.Collections.Generic;

namespace Freude.DocModel
{
    public interface IDocumentElementContainer : IDocumentElement
    {
        IList<IDocumentElement> Children { get; }
    }
}