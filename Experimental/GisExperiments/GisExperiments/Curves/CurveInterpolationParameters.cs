namespace GisExperiments.Curves
{
    public class CurveInterpolationParameters
    {
        public CurveInterpolationParameters(float tension, float smoothness)
        {
            Tension = tension;
            Smoothness = smoothness;
        }

        public float Tension { get; set; }
        public float Smoothness { get; set; }
    }
}