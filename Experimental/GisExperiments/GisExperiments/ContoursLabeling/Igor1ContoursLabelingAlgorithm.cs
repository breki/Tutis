using System;

namespace GisExperiments.ContoursLabeling
{
    public class Igor1ContoursLabelingAlgorithm : IContoursLabelingAlgorithm
    {
        public ContoursLabels LabelContours(IContoursSet contours, ContoursLabelingParameters parameters)
        {
            openAnnealing = new ContourOpenSimulatedAnnealing(parameters, 100, 0.85, 5, 100, 100);

            ContoursLabels labels = new ContoursLabels();

            LabelContours (labels, contours, parameters);

            return labels;
        }

        private void LabelContours (
            ContoursLabels labels, 
            IContoursSet contours, 
            ContoursLabelingParameters parameters)
        {
            foreach (IContoursForElevation elevation in contours.EnumerateElevations())
            {
                float labelLength = parameters.ContoursLabelMeasurer.CalculateLabelWidth(
                    elevation.Elevation, parameters);

                foreach (IContourLine contourLine in elevation.EnumerateLines())
                    LabelContourLine(labels, contourLine, parameters, labelLength);
            }
        }

        private void LabelContourLine(
            ContoursLabels labels, 
            IContourLine contourLine, 
            ContoursLabelingParameters parameters,
            float labelLength)
        {
            if (contourLine.Length < parameters.MinimumContourLength
                || contourLine.Length < labelLength)
                return;

            if (contourLine.IsClosed)
                LabelClosedContourLine (labels, contourLine, labelLength);
            else
                LabelOpenContourLine(labels, contourLine, labelLength);
        }

        private void LabelOpenContourLine(ContoursLabels labels, IContourLine contourLine, float labelLength)
        {
            openAnnealing.InitializeProblem(contourLine, labelLength);
            openAnnealing.Run();

            labels.AddLabels(openAnnealing.Solution);
        }

        private void LabelClosedContourLine(ContoursLabels labels, IContourLine contourLine, float labelLength)
        {
        }

        private ContourOpenSimulatedAnnealing openAnnealing;
    }
}