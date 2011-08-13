using System.Collections.Generic;

namespace TreasureChest.Tests.SampleModule
{
    public class ComponentWithEnumerableConstructorArg
    {
        public ComponentWithEnumerableConstructorArg(IEnumerable<IServiceX> services)
        {
            foreach (IServiceX service in services)
            {
                this.services.Add(service);
            }
        }

        public IList<IServiceX> Services
        {
            get { return services; }
        }

        private List<IServiceX> services = new List<IServiceX>();
    }
}