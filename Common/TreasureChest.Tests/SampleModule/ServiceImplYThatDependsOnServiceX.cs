namespace TreasureChest.Tests.SampleModule
{
    public class ServiceImplYThatDependsOnServiceX : IServiceY
    {
        public ServiceImplYThatDependsOnServiceX(IServiceX serviceX)
        {
        }
    }
}