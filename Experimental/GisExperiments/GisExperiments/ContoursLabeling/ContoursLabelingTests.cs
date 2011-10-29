using MbUnit.Framework;

namespace GisExperiments.ContoursLabeling
{
    public class ContoursLabelingTests
    {
        [Test]
        public void Test()
        {
            // input: contours
            // input: labeling settings
            // output: a list of objects:
                // line segment + elevation + placement for each character

            IContoursLabelingAlgorithm algo = null;
            ContoursLabelingParameters par = new ContoursLabelingParameters();
            par.ElevationInterval = 50;

            IContoursSet contours = null;
            algo.LabelContours(contours, par);
        }
    }
}