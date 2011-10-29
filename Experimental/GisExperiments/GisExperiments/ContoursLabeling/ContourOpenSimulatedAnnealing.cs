using System;

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

        public IContourLine ContourLine { get; set; }
        public ContoursLabels Solution { get; set; }

        protected override double GenerateRandomTransition()
        {
            throw new NotImplementedException();
        }

        protected override double InitializeSolution()
        {
            // we first spread out labels so that they are not too close and yet not too far (using a factor)
            Solution = new ContoursLabels();

            double diff = (labelingParameters.LabelCoverageRange*2 - labelingParameters.MinimumSameLabelDistance);
            float initialLabelSeparationDistance = (float)((diff*0.85 + labelingParameters.MinimumSameLabelDistance) / 2);

            float linePosition = 0;
            bool firstJump = true;
            while (true)
            {
                if (linePosition + initialLabelSeparationDistance >= ContourLine.Length)
                    break;

                ContourLine.PolylineAnalysis.MoveBy (initialLabelSeparationDistance);
                linePosition += initialLabelSeparationDistance;

                if (firstJump)
                {
                    firstJump = false;
                    initialLabelSeparationDistance *= 2;
                }

                ContourLabel label = new ContourLabel (linePosition, ContourLine.PolylineAnalysis.CurrentPoint);
                Solution.AddLabel(label);
            }

            maximumNumberOfLabels = Solution.Labels.Count;
            // min number of labels needed to cover everything
            minimumNumberOfLabelsNeeded = (int)Math.Ceiling (ContourLine.Length / (labelingParameters.LabelCoverageRange * 2));

            return CalculatePlacementValue(Solution);
        }

        protected override void UseTransitionAsSolution()
        {
            Solution = transition;
        }

        private double CalculatePlacementValue(ContoursLabels placement)
        {
            double coverage = CalculateLabelsCoverage(placement);
            double labelsUsage = CalculateLabelsUsage(placement);
            double averageNicety = CalculateNicetyOfLabels(placement);

            return coverage + labelsUsage * 2 + averageNicety;
        }

        private double CalculateLabelsCoverage(ContoursLabels placement)
        {
            IntervalSet intervals = new IntervalSet(ContourLine.Length);

            foreach (ContourLabel label in placement.Labels)
            {
                float leftCoverage = Math.Max(label.LinePosition - labelingParameters.LabelCoverageRange, 0);
                float rightCoverage = Math.Min (label.LinePosition + labelingParameters.LabelCoverageRange, ContourLine.Length);
                intervals.AddInterval(leftCoverage, rightCoverage);
            }

            double totalLengthOfIntervals = intervals.TotalIntervalsLength;
            return 1 - totalLengthOfIntervals/ContourLine.Length;
        }

        private double CalculateLabelsUsage(ContoursLabels placement)
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

        private double CalculateNicetyOfLabels (ContoursLabels placement)
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
            float angle = ContourLine.PolylineAnalysis.GetAngleForPosition(label.LinePosition);
            // 0 = text is totally horizontal
            // 1 = text is totally vertical

            if (angle > 180)
                angle = 260 - angle;
            double angleFactor = angle/180;

            // TODO: measure straightness
            throw new NotImplementedException();
        }

        private readonly ContoursLabelingParameters labelingParameters;
        private int maximumNumberOfLabels;
        private int minimumNumberOfLabelsNeeded;
        private ContoursLabels transition;
    }

    // quality of the solution:
    // how much of the contour line is covered by labels (%, some value indicating how far each label "covers" the line)
    // the number of labels (less is better if it satisfies the coverage)
    // the average nicety of each label
    // label nicety:
    // text angle
    // curvature
    // hard constraints:
    // there is an absolute minimum distance allowed for two labels which cannot be crossed
}