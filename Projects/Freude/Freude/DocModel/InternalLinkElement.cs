using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class InternalLinkElement : IDocumentElement
    {
        public InternalLinkElement(string linkName, string linkDescription = null)
        {
            Contract.Requires(linkName != null);
            this.linkName = linkName;
            this.linkDescription = linkDescription;
        }

        public string LinkName
        {
            get { return linkName; }
        }

        public string LinkDescription
        {
            get { return linkDescription; }
        }

        private readonly string linkName;
        private readonly string linkDescription;
    }
}