namespace GisExperiments.Proj4
{
    public class SrsParameter
    {
        public SrsParameter(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }

        public double? NumericValue
        {
            get { return numericValue; }
            set { numericValue = value; }
        }

        private string name;
        private string stringValue;
        private double? numericValue;
    }
}