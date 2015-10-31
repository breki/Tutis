using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class InternalLinkElement : IDocumentElement
    {
        public InternalLinkElement(string address)
        {
            Contract.Requires(address != null);
            this.address = address;
        }

        public string Address
        {
            get
            {
                //Contract.Ensures(Contract.Result<string>() != null);
                return address;
            }
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(address != null);
        }

        private readonly string address;
    }
}