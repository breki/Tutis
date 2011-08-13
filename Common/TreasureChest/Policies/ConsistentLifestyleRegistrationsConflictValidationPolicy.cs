using System;

namespace TreasureChest.Policies
{
    public class ConsistentLifestyleRegistrationsConflictValidationPolicy : GlobalChestPolicyBase, IRegistrationsConflictValidationPolicy
    {
        public void Validate(ServiceRegistration registration1, ServiceRegistration registration2)
        {
            foreach (Type serviceType in registration1.ServiceTypes)
            {
                if (registration2.CoversService(serviceType))
                {
                    if (registration1.RegistrationHandler.GetType() != registration2.RegistrationHandler.GetType())
                        throw TreasureChest.Chest.ChestException(
                            "You can not register components with different lifestyles for the same service (service={0}, impl1={1}, impl2={2}).",
                            serviceType.FullName,
                            registration2.ImplType.FullName,
                            registration1.ImplType.FullName);
                }
            }
        }
    }
}