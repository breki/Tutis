using System.Collections.Generic;

namespace Parser.Coco
{
    public class LabelDefinition
    {
        public void AddPart (ILabelPart part)
        {
            parts.Add(part);
        }

        public List<ILabelPart> Parts
        {
            get { return parts; }
        }

        private List<ILabelPart> parts = new List<ILabelPart>();
    }
}