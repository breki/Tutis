using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TreasureChest.Policies;

namespace TreasureChest.Fluent
{
    public class ServiceAction<TService> : ServiceActionBase<TService>
    {
        public ServiceAction(
            IChestMaster chest,
            PolicyCollection chestPolicies,
            IServicesRegistry servicesRegistry)
            : base(chest, chestPolicies, servicesRegistry)
        {
        }

        public override IChestFilling Done
        {
            get
            {
                Type serviceType = typeof(TService);
                serviceTypes.Insert(0, serviceType);

                if (!PrototypeRegistration.UsesCustomCreationMethod)
                {
                    if (PrototypeRegistration.ImplType != null)
                        AddRegistration(serviceTypes, PrototypeRegistration.ImplType);
                    else
                    {
                        IImplementationFilteringPolicy filteringPolicy
                            = ChestPolicies.FindPolicyOf<IImplementationFilteringPolicy>();
                        IList<Type> implementations = filteringPolicy.FindImplementationsOfTypes(serviceTypes.ToArray());
                        IImplementationSelectionPolicy selectionPolicy
                            = ChestPolicies.FindPolicyOf<IImplementationSelectionPolicy>();
                        Type implementationType = selectionPolicy.SelectImplementation(serviceType, implementations);

                        AddRegistration(serviceTypes, implementationType);
                    }
                }
                else
                    AddRegistrationWithCustomCreationMethod(serviceTypes);

                return Chest;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public override IServiceAction<TService> AlsoRegisterFor<TService2> ()
        {
            serviceTypes.Add(typeof(TService2));
            return this;
        }

        private List<Type> serviceTypes = new List<Type>();
    }
}