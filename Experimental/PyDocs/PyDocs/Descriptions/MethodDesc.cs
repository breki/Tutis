using System.Collections.Generic;

namespace PyDocs.Descriptions
{
    public class MethodDesc : LanguageElementDesc
    {
        public MethodDesc(string name)
            : base(name)
        {
        }

        private List<ParameterDesc> parameters = new List<ParameterDesc>();
    }
}