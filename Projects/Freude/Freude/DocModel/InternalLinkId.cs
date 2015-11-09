using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Freude.DocModel
{
    public class InternalLinkId
    {
        public InternalLinkId(string pageName, IEnumerable<string> namespaceParts = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(pageName));

            this.pageName = pageName;

            if (namespaceParts != null)
                this.namespaceParts = new List<string>(namespaceParts);
            else
                this.namespaceParts = new List<string>();
        }

        public int NamespacePartsCount
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return namespaceParts.Count;
            }
        }

        public IList<string> NamespaceParts
        {
            get { return namespaceParts; }
        }

        public string PageName
        {
            get { return pageName; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (string namespacePart in namespaceParts)
                s.Append(namespacePart).Append(":");

            s.Append(pageName);

            return s.ToString();
        }

        private readonly List<string> namespaceParts;
        private readonly string pageName;
    }
}