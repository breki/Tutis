using System.Collections.Generic;

namespace PyDocs.Descriptions
{
    public class PackageDesc : LanguageElementDesc
    {
        public PackageDesc(string name)
            : base(name)
        {
        }

        public IList<ModuleDesc> Modules
        {
            get { return modules; }
        }

        private List<ModuleDesc> modules = new List<ModuleDesc>();
    }
}