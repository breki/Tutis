using System;
using System.Collections.Generic;
using System.Threading;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class ThreadSingletonLifestyle : RegistrationHandlerBase
    {
        public ThreadSingletonLifestyle(IChestMaster chest) : base(chest)
        {
        }

        public override bool RequiresFetchingValidation
        {
            get { return true; }
        }

        public override bool CanBeFetched(
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            lock (this)
            {
                if (instancesPerThreads.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                    return true;

                if (Registration.UsesCustomCreationMethod)
                    return true;

                return componentCreator.CanBeCreated(Registration.ImplType, context);
            }
        }

        public override void DestroyAllInstances(PolicyCollection chestPolicies)
        {
            lock (this)
            {
                foreach (object instance in instancesPerThreads.Values)
                    DestroyInstance(instance, chestPolicies);

                instancesPerThreads.Clear();
            }
        }

        public override object GetInstance(
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            lock (this)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                if (!instancesPerThreads.ContainsKey(threadId))
                {
                    object instance = GetInstancePrivate(context, componentCreator);
                    instancesPerThreads.Add(threadId, instance);
                }

                return instancesPerThreads[threadId];
            }
        }

        public override bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies)
        {
            Chest.Logger.Log (LogEventType.ReleaseInstance, TreasureChest.Chest.InstanceArgName, instance);
            return false;
        }

        private Dictionary<int, object> instancesPerThreads = new Dictionary<int, object>();
    }
}