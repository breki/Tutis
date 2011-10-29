using System;

namespace GisExperiments.ContoursLabeling
{
    public class Igor1ContoursLabelingAlgorithm : IContoursLabelingAlgorithm
    {
        public ContoursLabels LabelContours(IContoursSet contours, ContoursLabelingParameters parameters)
        {
            ContoursLabels labels = new ContoursLabels ();

            LabelContours (labels, contours, parameters);

            return labels;
        }

        private void LabelContours (ContoursLabels labels, IContoursSet contours, ContoursLabelingParameters parameters)
        {
            foreach (IContoursForElevation elevation in contours.EnumerateElevations ())
                foreach (IContourLine contourLine in elevation.EnumerateLines ())
                    LabelContourLine(labels, contourLine, parameters);
        }

        private void LabelContourLine(ContoursLabels labels, IContourLine contourLine, ContoursLabelingParameters parameters)
        {
            if (contourLine.Length < parameters.MinimumContourLength)
                return;

            if (contourLine.IsClosed)
                LabelClosedContourLine (labels, contourLine, parameters);
            else
                LabelOpenContourLine(labels, contourLine, parameters);
        }

        private void LabelOpenContourLine(ContoursLabels labels, IContourLine contourLine, ContoursLabelingParameters parameters)
        {
            throw new NotImplementedException();
        }

        private void LabelClosedContourLine(ContoursLabels labels, IContourLine contourLine, ContoursLabelingParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}