using System;
using System.Collections.Generic;

namespace TreasureChest
{
    public interface IServicesRegistry
    {
        void AddRegistration(ServiceRegistration registration);
        ServiceRegistration GetFirstRegistrationForService(Type serviceType);
        IEnumerable<ServiceRegistration> EnumerateRegistrations();
        IEnumerable<ServiceRegistration> EnumerateRegistrationsForService(Type serviceType);
        bool IsServiceRegistered(Type serviceType);
    }
}