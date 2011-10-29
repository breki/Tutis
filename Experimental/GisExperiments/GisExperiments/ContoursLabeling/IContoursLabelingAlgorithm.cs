namespace GisExperiments.ContoursLabeling
{
    public interface IContoursLabelingAlgorithm
    {
        ContoursLabels LabelContours(IContoursSet contours, ContoursLabelingParameters parameters);
    }
}