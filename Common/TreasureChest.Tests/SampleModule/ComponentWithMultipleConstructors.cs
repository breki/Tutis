namespace TreasureChest.Tests.SampleModule
{
    public class ComponentWithMultipleConstructors : IServiceX
    {
        public ComponentWithMultipleConstructors(IServiceY serviceY)
        {
            this.serviceY = serviceY;
        }

        public ComponentWithMultipleConstructors(IServiceY serviceY, IServiceWithSingleImplementation serviceWithSingleImplementation)
        {
            this.serviceY = serviceY;
            this.serviceWithSingleImplementation = serviceWithSingleImplementation;
        }

        private readonly IServiceY serviceY;
        private readonly IServiceWithSingleImplementation serviceWithSingleImplementation;
    }
}