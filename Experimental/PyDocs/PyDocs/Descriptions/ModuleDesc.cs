using System.Collections.Generic;

namespace PyDocs.Descriptions
{
    public class ModuleDesc : LanguageElementDesc
    {
        public ModuleDesc(string name)
            : base(name)
        {
        }

        private List<ClassDesc> classes = new List<ClassDesc>();
    }
}