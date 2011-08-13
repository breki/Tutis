using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class ServicesRegistry : IServicesRegistry
    {
        public ServicesRegistry(PolicyCollection chestPolicies)
        {
            this.chestPolicies = chestPolicies;
        }

        // todo slow
        public void AddRegistration(ServiceRegistration registration)
        {
            lock (this)
            {
                // todo slow
                foreach (IRegistrationValidationPolicy policy in
                    chestPolicies.FindAllPoliciesOf<IRegistrationValidationPolicy>())
                    policy.Validate(registration);

                foreach (ServiceRegistration existingRegistration in EnumerateRegistrations())
                {
                    foreach (IRegistrationsConflictValidationPolicy policy
                        in chestPolicies.FindAllPoliciesOf<IRegistrationsConflictValidationPolicy>())
                        policy.Validate(registration, existingRegistration);
                }

                foreach (Type serviceType in registration.ServiceTypes)
                {
                    if (!registrations.ContainsKey(serviceType))
                        registrations.Add(serviceType, new List<ServiceRegistration>());

                    registrations[serviceType].Add(registration);
                }
            }
        }

        //[Obsolete]
        //public void DestroyAllInstances()
        //{
        //    foreach (List<ServiceRegistration> list in registrations.Values)
        //        foreach (ServiceRegistration registration in list)
        //            registration.RegistrationHandler.DestroyAllInstances(chestPolicies);
        //}

        public ServiceRegistration GetFirstRegistrationForService(Type serviceType)
        {
            return registrations[serviceType][0];
        }

        public IEnumerable<ServiceRegistration> EnumerateRegistrations()
        {
            lock (this)
            {
                foreach (KeyValuePair<Type, List<ServiceRegistration>> pair in registrations)
                {
                    foreach (ServiceRegistration chestRegistration in pair.Value)
                    {
                        yield return chestRegistration;
                    }
                }
            }
        }

        public IEnumerable<ServiceRegistration> EnumerateRegistrationsForService(Type serviceType)
        {
            return registrations[serviceType];
        }

        public bool IsServiceRegistered(Type serviceType)
        {
            return registrations.ContainsKey(serviceType)
                   && registrations[serviceType].Count > 0;
        }

        [SuppressMessage ("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Dictionary<Type, List<ServiceRegistration>> Registrations
        {
            get { return registrations; }
        }

        private readonly PolicyCollection chestPolicies;
        [SuppressMessage ("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        private Dictionary<Type, List<ServiceRegistration>> registrations = new Dictionary<Type, List<ServiceRegistration>> ();
    }
}