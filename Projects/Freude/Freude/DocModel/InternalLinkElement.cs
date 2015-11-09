using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class InternalLinkElement : IDocumentElement
    {
        public InternalLinkElement(InternalLinkId linkId, string linkDescription = null)
        {
            Contract.Requires(linkId != null);
            this.linkId = linkId;
            this.linkDescription = linkDescription;
        }

        public InternalLinkId LinkId
        {
            get { return linkId; }
        }

        public string LinkDescription
        {
            get { return linkDescription; }
        }

        private readonly InternalLinkId linkId;
        private readonly string linkDescription;
    }
}