using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public class LabelsOnAContour
    {
        public int LabelsCount { get { return labels.Count; } }
        public IList<ContourLabel> Labels { get { return labels.Values; } }

        public void AddLabel (ContourLabel label)
        {
            labels.Add(label.LinePosition, label);
        }

        public LabelsOnAContour CloneDeep()
        {
            LabelsOnAContour clone = new LabelsOnAContour ();
            foreach (ContourLabel contourLabel in Labels)
                clone.AddLabel(contourLabel.CloneDeep ());

            return clone;
        }

        public void RemoveLabel(int labelIndex)
        {
            labels.RemoveAt(labelIndex);
        }

        private SortedList<double, ContourLabel> labels = new SortedList<double, ContourLabel>();
    }
}