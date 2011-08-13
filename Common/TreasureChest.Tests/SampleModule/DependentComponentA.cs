namespace TreasureChest.Tests.SampleModule
{
    public class DependentComponentA : IServiceX
    {
        public DependentComponentA(IServiceY serviceY)
        {
            this.serviceY = serviceY;
        }

        public IServiceY ServiceY
        {
            get { return serviceY; }
        }

        private readonly IServiceY serviceY;
    }
}