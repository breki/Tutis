using System.Collections.Generic;
using System.Linq;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class PoolLifestyle : RegistrationHandlerBase
    {
        public PoolLifestyle (IChestMaster chest)
            : base (chest)
        {
        }

        public override bool RequiresFetchingValidation
        {
            get { return !Registration.UsesCustomCreationMethod; }
        }

        public override bool CanBeFetched (
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            if (Registration.UsesCustomCreationMethod)
                return true;

            return componentCreator.CanBeCreated (Registration.ImplType, context);
        }

        public override void DestroyAllInstances (PolicyCollection chestPolicies)
        {
            lock (this)
            {
                foreach (PooledObject pooledObject in pool)
                    DestroyInstance(pooledObject.Instance, chestPolicies);

                pool.Clear();
            }
        }

        public override object GetInstance (
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            lock (this)
            {
                PooledObject freeInstance = pool.FirstOrDefault(x => x.IsNotInUse);

                if (freeInstance == null)
                {
                    object instance = GetInstancePrivate(context, componentCreator);
                    PooledObject newPooledObject = new PooledObject(instance);
                    pool.Add(newPooledObject);
                    freeInstance = newPooledObject;
                }

                freeInstance.Reserve();

                return freeInstance.Instance;
            }
        }

        public override bool MarkInstanceAsReleased (object instance, PolicyCollection chestPolicies)
        {
            lock (this)
            {
                PooledObject pooledObject = pool.First(x => ReferenceEquals(x.Instance, instance));
                pooledObject.Release();
                Chest.Logger.Log(LogEventType.ReleaseInstance, TreasureChest.Chest.InstanceArgName, instance);

                return false;
            }
        }

        private HashSet<PooledObject> pool = new HashSet<PooledObject> ();
    }
}