using System;
using System.Collections.Generic;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class TransientLifestyle : RegistrationHandlerBase
    {
        public override bool RequiresFetchingValidation
        {
            get { return !Registration.UsesCustomCreationMethod; }
        }

        public override bool CanBeFetched(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            if (Registration.UsesCustomCreationMethod)
                return true;

            return componentCreator.CanBeCreated(Registration.ImplType, context);
        }

        public override void DestroyAllInstances(PolicyCollection chestPolicies)
        {
            foreach (object instance in instances)
                DestroyInstance(instance, chestPolicies);

            instances.Clear();
        }

        public override object GetInstance(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            object instance = GetInstancePrivate(chest, context, componentCreator);
            instances.Add(instance);
            return instance;
        }

        public override bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies)
        {
            instances.Remove(instance);
            return true;
        }

        private HashSet<object> instances = new HashSet<object>();
    }
}