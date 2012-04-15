using System.Globalization;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class SingletonLifestyle : RegistrationHandlerBase
    {
        public SingletonLifestyle (IChestMaster chest)
            : base (chest)
        {
        }

        public SingletonLifestyle(IChestMaster chest, object instance) : base(chest)
        {
            this.instance = instance;
        }

        public override bool RequiresFetchingValidation
        {
            get { return !IsInstantiated && !Registration.UsesCustomCreationMethod; }
        }

        public bool IsInstantiated
        {
            get { return instance != null; }
        }

        public override void DestroyAllInstances(PolicyCollection chestPolicies)
        {
            if (IsInstantiated)
            {
                DestroyInstance(instance, chestPolicies);
                instance = null;
            }
        }

        public override bool CanBeFetched(
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            if (!IsInstantiated && !Registration.UsesCustomCreationMethod)
                return componentCreator.CanBeCreated(Registration.ImplType, context);

            return true;
        }

        public override object GetInstance(
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            lock (this)
            {
                if (!IsInstantiated)
                {
                    if (instantiationInProgress)
                    {
                        string message = string.Format (
                            CultureInfo.InvariantCulture,
                            "Singleton service implementation {0} instantiation is already in progress, looks like you have a dependency cycle somewhere.",
                            Registration.ImplType);
                        throw new ChestException (message);
                    }

                    instantiationInProgress = true;
                    instance = GetInstancePrivate (context, componentCreator);
                }

                return instance;
            }
        }

        public void MarkAsInstantiated (object instance)
        {
            instantiationInProgress = false;
            this.instance = instance;
        }

        public override bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies)
        {
            Chest.Logger.Log (LogEventType.ReleaseInstance, TreasureChest.Chest.InstanceArgName, instance);
            return false;
        }

        public void SetInstance(object instance)
        {
            this.instance = instance;
        }

        private object instance;
        private bool instantiationInProgress;
    }
}