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

        private List<ContourLabel> labels = new List<ContourLabel>();
    }
}