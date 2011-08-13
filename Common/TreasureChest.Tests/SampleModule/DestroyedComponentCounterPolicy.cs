using TreasureChest.Policies;

namespace TreasureChest.Tests.SampleModule
{
    public class DestroyedComponentCounterPolicy : ChestExtensionBase, IBeforeComponentDestroyedPolicy
    {
        public void BeforeDestroyed(object instance)
        {
            counter++;
        }

        public int Counter
        {
            get { return counter; }
        }

        private int counter;
    }
}