using System;
using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public class ContoursLabels
    {
        public void AddLabel(ContourLabel label)
        {
            labels.Add(label);
        }

        public IList<ContourLabel> Labels
        {
            get { return labels; }
        }

        public void AddLabels(ContoursLabels contoursLabels)
        {
            labels.AddRange(contoursLabels.labels);
        }

        public void AddLabels (LabelsOnAContour labelsOnAContour)
        {
            labels.AddRange (labelsOnAContour.Labels);
        }

        public ContoursLabels CloneDeep()
        {
            ContoursLabels clone = new ContoursLabels();
            foreach (ContourLabel contourLabel in Labels)
                clone.labels.Add(contourLabel.CloneDeep());

            return clone;
        }

        public void RemoveLabel(int labelIndex)
        {
            labels.RemoveAt(labelIndex);
        }

        private List<ContourLabel> labels = new List<ContourLabel>();
    }
}