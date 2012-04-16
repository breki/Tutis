using System.Collections.Generic;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class TransientLifestyle : RegistrationHandlerBase
    {
        public TransientLifestyle(IChestMaster chest) : base(chest)
        {
        }

        public override bool RequiresFetchingValidation
        {
            get { return !Registration.UsesCustomCreationMethod; }
        }

        public override bool CanBeFetched(
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            if (Registration.UsesCustomCreationMethod)
                return true;

            return componentCreator.CanBeCreated(Registration.ImplType, context);
        }

        public override void DestroyAllInstances(PolicyCollection chestPolicies)
        {
            lock (this)
            {
                foreach (object instance in instances)
                    DestroyInstance(instance, chestPolicies);

                instances.Clear();
            }
        }

        public override object GetInstance(
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            lock (this)
            {
                object instance = GetInstancePrivate(context, componentCreator);
                instances.Add(instance);
                return instance;
            }
        }

        public override bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies)
        {
            lock (this)
            {
                instances.Remove(instance);
                Chest.Logger.Log (LogEventType.ReleaseInstance, TreasureChest.Chest.InstanceArgName, instance);

                return true;
            }
        }

        private HashSet<object> instances = new HashSet<object>();
    }
}