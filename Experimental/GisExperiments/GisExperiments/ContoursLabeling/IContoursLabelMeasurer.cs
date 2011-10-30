namespace GisExperiments.ContoursLabeling
{
    public interface IContoursLabelMeasurer
    {
        float CalculateLabelWidth(double elevation, ContoursLabelingParameters parameters);
    }
}