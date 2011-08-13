namespace TreasureChest.Tests.SampleModule
{
    public class ComponentWithNonWiredConstructorParameters
    {
        public ComponentWithNonWiredConstructorParameters(
            string fileName, 
            IServiceWithSingleImplementation dependency)
        {
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
        }

        private readonly string fileName;
    }
}