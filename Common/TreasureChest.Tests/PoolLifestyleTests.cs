using System.Collections.Generic;
using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class PoolLifestyleTests : ChestTestFixtureBase
    {
        [Test]
        public void RequestingPooledComponentAndReleasingItShouldReturnSameInstance()
        {
            Chest.AddPooled<IServiceX, IndependentComponentA>();

            IServiceX instance = null;

            for (int i = 0; i < 5; i++)
            {
                using (Lease<IServiceX> lease = Chest.Fetch<IServiceX>())
                {
                    if (instance != null)
                        Assert.AreSame(instance, lease.Instance);
                    else
                        instance = lease.Instance;
                }
            }
        }

        [Test]
        public void RequestingPooledComponentSeveralTimesWithoutReleasingShouldReturnDifferentInstances ()
        {
            Chest.AddPooled<IServiceX, IndependentComponentA> ();

            List<IServiceX> instances = new List<IServiceX>();

            for (int i = 0; i < 5; i++)
            {
                Lease<IServiceX> lease = Chest.Fetch<IServiceX>();
                instances.Add(lease.Instance);

                for (int j = 0; j < i; j++)
                    Assert.AreNotSame(instances[j], instances[i]);
            }
        }
    }
}