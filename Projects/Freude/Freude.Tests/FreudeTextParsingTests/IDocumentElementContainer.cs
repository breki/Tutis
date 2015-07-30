using System.Collections.Generic;

namespace Freude.Tests.FreudeTextParsingTests
{
    public interface IDocumentElementContainer : IDocumentElement
    {
        IList<IDocumentElement> Children { get; }
    }
}