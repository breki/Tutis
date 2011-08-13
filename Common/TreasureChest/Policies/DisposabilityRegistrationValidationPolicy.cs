using System;

namespace TreasureChest.Policies
{
    public class DisposabilityRegistrationValidationPolicy : GlobalChestPolicyBase, IRegistrationValidationPolicy
    {
        public void Validate(ServiceRegistration registration)
        {
            if (registration.ServiceTypesCount == 1)
            {
                Type firstServiceType = registration.FirstServiceType;

                if (disposableType.IsAssignableFrom(registration.ImplType)
                    && !disposableType.IsAssignableFrom(firstServiceType))
                    TreasureChest.Chest.ChestException(
                        "The implementation {0} is disposable, but the service {1} is not.",
                        registration.ImplType.FullName,
                        firstServiceType.FullName);
            }
        }

        private static Type disposableType = typeof(IDisposable);
    }
}