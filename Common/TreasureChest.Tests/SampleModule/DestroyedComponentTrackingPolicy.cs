using System.Collections.Generic;
using TreasureChest.Policies;

namespace TreasureChest.Tests.SampleModule
{
    public class DestroyedComponentTrackingPolicy : ChestExtensionBase, IBeforeComponentDestroyedPolicy
    {
        public void BeforeDestroyed(object instance)
        {
            destroyedComponents.Add(instance.GetType().FullName);
        }

        public int Counter
        {
            get { return destroyedComponents.Count; }
        }

        public IList<string> DestroyedComponents
        {
            get { return destroyedComponents; }
        }

        private List<string> destroyedComponents = new List<string>();
    }
}