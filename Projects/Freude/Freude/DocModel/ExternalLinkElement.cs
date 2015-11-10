using System;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class ExternalLinkElement : IDocumentElement
    {
        public ExternalLinkElement(Uri url, string linkDescription = null)
        {
            Contract.Requires(url != null);
            this.url = url;
            this.linkDescription = linkDescription;
        }

        public Uri Url
        {
            get { return url; }
        }

        public string LinkDescription
        {
            get { return linkDescription; }
        }

        private readonly Uri url;
        private readonly string linkDescription;
    }
}