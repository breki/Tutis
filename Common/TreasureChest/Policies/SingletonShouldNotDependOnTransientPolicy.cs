using System;

namespace TreasureChest.Policies
{
    public class SingletonShouldNotDependOnTransientPolicy : IRegistrationValidationPolicy
    {
        public void Validate(ServiceRegistration registration)
        {
            throw new NotImplementedException();
        }

        public void AssignLogger(ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}