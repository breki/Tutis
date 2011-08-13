using System;
using System.Globalization;

namespace TreasureChest.Policies
{
    public class OverlappingRegistrationsConflictValidationPolicy : GlobalChestPolicyBase, 
                                                                    IRegistrationsConflictValidationPolicy
    {
        public void Validate(ServiceRegistration registration1, ServiceRegistration registration2)
        {
            bool sameImplementation = registration1.ImplType == registration2.ImplType;

            if (!sameImplementation)
                return;

            foreach (Type serviceType1 in registration1.ServiceTypes)
            {
                if (registration2.CoversService(serviceType1))
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Service {0} + implementation {1} has already been registered.",
                        registration1.ImplType.FullName,
                        serviceType1.FullName);
                    throw TreasureChest.Chest.ChestException(message);
                }
            }
        }
    }
}