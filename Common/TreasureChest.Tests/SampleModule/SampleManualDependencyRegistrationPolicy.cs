using TreasureChest.Policies;

namespace TreasureChest.Tests.SampleModule
{
    public class SampleManualDependencyRegistrationPolicy : GlobalChestPolicyBase, IAfterComponentCreatedPolicy
    {
        public void AfterCreated(object instance, IRegistrationHandler registrationHandler)
        {
            if (instance is IndependentComponentB)
                Chest.RegisterDependency(Dependency, instance);
        }

        private IndependentComponentA Dependency
        {
            get
            {
                if (dependency == null)
                    dependency = Chest.Fetch<IndependentComponentA>();

                return dependency;
            }
        }

        private IndependentComponentA dependency;
    }
}