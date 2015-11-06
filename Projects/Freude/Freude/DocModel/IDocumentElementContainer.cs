using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    [ContractClass(typeof(IDocumentElementContainerContract))]
    public interface IDocumentElementContainer : IDocumentElement
    {
        int ChildrenCount { get; }
        IList<IDocumentElement> Children { get; }
        void AddChild(IDocumentElement child);
    }

    [GeneratedCode("CC", "1")]
    [ContractClassFor(typeof(IDocumentElementContainer))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IDocumentElementContainerContract : IDocumentElementContainer
    {
        public int ChildrenCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == ((IDocumentElementContainer)this).Children.Count);
                throw new InvalidOperationException();
            }
        }

        IList<IDocumentElement> IDocumentElementContainer.Children
        {
            get
            {
                Contract.Ensures (Contract.Result<IList<IDocumentElement>> () != null);
                throw new NotImplementedException ();
            }
        }

        void IDocumentElementContainer.AddChild(IDocumentElement child)
        {
            Contract.Requires(child != null);
            throw new NotImplementedException();
        }
    }
}