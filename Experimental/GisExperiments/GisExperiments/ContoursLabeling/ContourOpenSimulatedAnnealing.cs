using System;
using Brejc.Geometry;

namespace GisExperiments.ContoursLabeling
{
    public class ContourOpenSimulatedAnnealing : SimulatedAnnealing
    {
        public ContourOpenSimulatedAnnealing(
            ContoursLabelingParameters labelingParameters,
            double initialTemperature, 
            double coolingFraction, 
            double k, 
            int coolingSteps, 
            decimal stepsPerTemperature) 
            : base(initialTemperature, coolingFraction, k, coolingSteps, stepsPerTemperature)
        {
            this.labelingParameters = labelingParameters;
        }

        public LabelsOnAContour Solution { get; set; }

        public void InitializeProblem(IContourLine contourLine, float labelLength)
        {
            this.contourLine = contourLine;
            this.labelLength = labelLength;
            Rnd = new Random(labelingParameters.RandomSeed);
        }

        protected override double GenerateRandomTransition()
        {
            transition = Solution.CloneDeep();

            double randomDecision = Rnd.NextDouble();
            if (randomDecision < 0.1 && transition.LabelsCount > 1)
                RemoveLabel();
            else
                MoveLabel();

            return CalculatePlacementValue(transition);
        }

        private void RemoveLabel()
        {
            int labelToRemove = Rnd.Next(transition.LabelsCount);
            transition.RemoveLabel(labelToRemove);
        }

        private void MoveLabel()
        {
            int labelToMove = Rnd.Next(transition.LabelsCount);
            double movement = (Rnd.NextDouble() - 0.5) * 100;

            // check that the label isn't too close to another one after the move
            // (or too close to the ends of the contour)
            ContourLabel label = transition.Labels[labelToMove];
            float newLabelPosition = (float)(label.LinePosition + movement);

            if (movement < 0)
            {
                // label is moving to the left
                if (labelToMove > 0)
                {
                    // find a label that's to the left
                    ContourLabel neighbourLabel = transition.Labels[labelToMove - 1];
                    float distanceBetweenLabels = newLabelPosition - neighbourLabel.LinePosition;
                    if (distanceBetweenLabels < labelingParameters.MinimumSameLabelDistance)
                    {
                        // the label has gone too far to the left, move it to the
                        // absolute minimum distance
                        newLabelPosition = neighbourLabel.LinePosition + distanceBetweenLabels;
                    }
                }
                else
                {
                    // make sure the label is not too far to the start of the contour
                    newLabelPosition = Math.Max(newLabelPosition, labelingParameters.MinimumSameLabelDistance/2);
                }
            }
            else
            {
                // label is moving to the right
                if (labelToMove < transition.LabelsCount-1)
                {
                    // find a label that's to the right
                    ContourLabel neighbourLabel = transition.Labels[labelToMove + 1];
                    float distanceBetweenLabels = neighbourLabel.LinePosition - newLabelPosition;
                    if (distanceBetweenLabels < labelingParameters.MinimumSameLabelDistance)
                    {
                        // the label has gone too far to the left, move it to the
                        // absolute minimum distance
                        newLabelPosition = neighbourLabel.LinePosition - distanceBetweenLabels;
                    }
                }
                else
                {
                    // make sure the label is not too far to the end of the contour
                    newLabelPosition = Math.Max (
                        newLabelPosition, 
                        contourLine.Length - labelingParameters.MinimumSameLabelDistance / 2);
                }
            }

            // mark the label movement
            label.LinePosition = newLabelPosition;
        }

        protected override double InitializeSolution()
        {
            // we first spread out labels so that they are not too close and yet not too far (using a factor)
            Solution = new LabelsOnAContour();

            double diff = (labelingParameters.LabelCoverageRange*2 - labelingParameters.MinimumSameLabelDistance);
            float initialLabelSeparationDistance = (float)((diff*0.85 + labelingParameters.MinimumSameLabelDistance) / 2);

            float linePosition = 0;
            bool firstJump = true;
            while (true)
            {
                if (linePosition + initialLabelSeparationDistance >= contourLine.Length)
                    break;

                contourLine.PolylineAnalysis.MoveBy (initialLabelSeparationDistance);
                linePosition += initialLabelSeparationDistance;

                if (firstJump)
                {
                    firstJump = false;
                    initialLabelSeparationDistance *= 2;
                }

                ContourLabel label = new ContourLabel (linePosition, contourLine.PolylineAnalysis.CurrentPoint);
                Solution.AddLabel(label);
            }

            maximumNumberOfLabels = Solution.Labels.Count;
            // min number of labels needed to cover everything
            minimumNumberOfLabelsNeeded = (int)Math.Ceiling (contourLine.Length / (labelingParameters.LabelCoverageRange * 2));

            return CalculatePlacementValue(Solution);
        }

        protected override void UseTransitionAsSolution()
        {
            Solution = transition;
        }

        private double CalculatePlacementValue(LabelsOnAContour placement)
        {
            double coverage = CalculateLabelsCoverage(placement);
            double labelsUsage = CalculateLabelsUsage(placement);
            double averageNicety = CalculateNicetyOfLabels(placement);

            return coverage + labelsUsage * 2 + averageNicety;
        }

        private double CalculateLabelsCoverage(LabelsOnAContour placement)
        {
            IntervalSet intervals = new IntervalSet(contourLine.Length);

            foreach (ContourLabel label in placement.Labels)
            {
                float leftCoverage = Math.Max(label.LinePosition - labelingParameters.LabelCoverageRange, 0);
                float rightCoverage = Math.Min (label.LinePosition + labelingParameters.LabelCoverageRange, contourLine.Length);
                intervals.AddInterval(leftCoverage, rightCoverage);
            }

            double totalLengthOfIntervals = intervals.TotalIntervalsLength;
            return 1 - totalLengthOfIntervals/contourLine.Length;
        }

        private double CalculateLabelsUsage(LabelsOnAContour placement)
        {
            // score:
            // 0 = the actual number is the minimal number (or less)
            // 1 = the actual number is the starting number of contours
            if (minimumNumberOfLabelsNeeded == maximumNumberOfLabels)
                return 0;

            double value = ((double)placement.Labels.Count - minimumNumberOfLabelsNeeded)/
                   (maximumNumberOfLabels - minimumNumberOfLabelsNeeded);
            if (value > 1)
                throw new InvalidOperationException("BUG");
            return value;
        }

        private double CalculateNicetyOfLabels (LabelsOnAContour placement)
        {
            double currentUgliestLabelValue = 0;

            foreach (ContourLabel label in placement.Labels)
            {
                double labelValue = CalculateNicetyOfLabel(label);
                currentUgliestLabelValue = Math.Max(currentUgliestLabelValue, labelValue);
            }

            return currentUgliestLabelValue;
        }

        private double CalculateNicetyOfLabel(ContourLabel label)
        {
            float angle = contourLine.PolylineAnalysis.GetAngleForPosition(label.LinePosition);
            // 0 = text is totally horizontal
            // 1 = text is totally vertical

            if (angle > 180)
                angle = 260 - angle;
            double angleFactor = angle/180;

            // TODO: measure straightness
            contourLine.PolylineAnalysis.MoveTo(label.LinePosition - labelLength / 2);
            Point2<float> startSegmentPoint = contourLine.PolylineAnalysis.CurrentPoint;
            contourLine.PolylineAnalysis.MoveBy(labelLength);
            Point2<float> endSegmentPoint = contourLine.PolylineAnalysis.CurrentPoint;
            double directLength = Math.Sqrt(
                (startSegmentPoint.X - endSegmentPoint.X) * (startSegmentPoint.X - endSegmentPoint.X) 
                + (startSegmentPoint.Y - endSegmentPoint.Y) * (startSegmentPoint.Y - endSegmentPoint.Y));

            // 0 = directLength == labelLength
            double straightness = Math.Min((labelLength - directLength) / directLength, 1);

            return straightness*5 + angleFactor;
        }

        private IContourLine contourLine;
        private readonly ContoursLabelingParameters labelingParameters;
        private float labelLength;
        private int maximumNumberOfLabels;
        private int minimumNumberOfLabelsNeeded;
        private LabelsOnAContour transition;
    }
}