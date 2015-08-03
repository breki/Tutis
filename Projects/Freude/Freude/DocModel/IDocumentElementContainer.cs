using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    [ContractClass(typeof(IDocumentElementContainerContract))]
    public interface IDocumentElementContainer : IDocumentElement
    {
        IList<IDocumentElement> Children { get; }
    }

    [ContractClassFor(typeof(IDocumentElementContainer))]
    internal abstract class IDocumentElementContainerContract : IDocumentElementContainer
    {
        IList<IDocumentElement> IDocumentElementContainer.Children
        {
            get
            {
                Contract.Ensures (Contract.Result<System.Collections.Generic.IList<Freude.DocModel.IDocumentElement>> () != null);
                throw new System.NotImplementedException ();
            }
        }
    }
}