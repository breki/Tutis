using System.Collections.Generic;

namespace PyDocs.Descriptions
{
    public class ClassDesc : LanguageElementDesc
    {
        public ClassDesc(string name)
            : base(name)
        {
        }

        private List<MethodDesc> methods = new List<MethodDesc>();

        private List<AttributeDesc> attributes = new List<AttributeDesc>();
    }
}