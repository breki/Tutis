using System;
using System.Collections.Generic;
using Brejc.Geometry;

namespace GisExperiments.ContoursLabeling
{
    public class GridBasedContoursLabelingAlgorithm : IContoursLabelingAlgorithm
    {
        public float GridSize
        {
            get { return gridSize; }
            set { gridSize = value; }
        }

        public ContoursLabels LabelContours(IContoursSet contours, ContoursLabelingParameters parameters)
        {
            ContoursLabels labels = new ContoursLabels();

            LabelContoursThroughGrid(labels, contours, parameters);

            return labels;
        }

        private void LabelContoursThroughGrid(ContoursLabels labels, IContoursSet contours, ContoursLabelingParameters parameters)
        {
            for (double gridY = parameters.ProcessBounds.MinY; gridY < parameters.ProcessBounds.MaxY; gridY += gridSize)
                for (double gridX = parameters.ProcessBounds.MinX; gridX < parameters.ProcessBounds.MaxX; gridX += gridSize)
                    ProcessGridCell(gridX, gridY, labels, contours, parameters);
        }

        private void ProcessGridCell(double gridX, double gridY, ContoursLabels labels, IContoursSet contours, ContoursLabelingParameters parameters)
        {
            List<ContourLabel> labelCandidates = new List<ContourLabel>();

            Bounds2 gridBounds = Bounds2.FromXYWH(gridX, gridY, gridSize, gridSize);
            foreach (IContoursForElevation elevation in contours.EnumerateElevations())
            {
                foreach (IContourLine contourLine in elevation.EnumerateLines())
                {
                    //if (!contourLine.PassesThrough(gridBounds))
                    //    continue;
                }
            }

            throw new NotImplementedException();
        }

        private float gridSize = 200;
    }
}