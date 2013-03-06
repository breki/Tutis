using System.Collections.Generic;

namespace Elf
{
    public class FieldsContext
    {
        public FieldsContext(IEnumerable<string> fields)
        {
            this.fields = new List<string>(fields);
        }

        public IList<string> Fields
        {
            get { return fields; }
        }

        private List<string> fields;
    }
}