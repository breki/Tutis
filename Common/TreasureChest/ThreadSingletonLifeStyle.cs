using System;
using System.Collections.Generic;
using System.Threading;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class ThreadSingletonLifestyle : RegistrationHandlerBase
    {
        public override bool RequiresFetchingValidation
        {
            get { return true; }
        }

        public override bool CanBeFetched(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            if (instancesPerThreads.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                return true;

            if (Registration.UsesCustomCreationMethod)
                return true;

            return componentCreator.CanBeCreated(Registration.ImplType, context);
        }

        public override void DestroyAllInstances(PolicyCollection chestPolicies)
        {
            foreach (object instance in instancesPerThreads.Values)
                DestroyInstance(instance, chestPolicies);

            instancesPerThreads.Clear();
        }

        public override object GetInstance(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            if (!instancesPerThreads.ContainsKey(threadId))
            {
                object instance = GetInstancePrivate(chest, context, componentCreator);
                instancesPerThreads.Add(threadId, instance);
            }

            return instancesPerThreads[threadId];
        }

        public override bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies)
        {
            return false;
        }

        private Dictionary<int, object> instancesPerThreads = new Dictionary<int, object>();
    }
}