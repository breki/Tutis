using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TreasureChest.Policies
{
    public class PropertyInjectionPolicy : ChestExtensionBase, IAfterComponentCreatedPolicy
    {
        [SuppressMessage ("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails")]
        public void AfterCreated (object instance, IRegistrationHandler registrationHandler)
        {
            IAutowiringConstructorArgumentTypesPolicy autowiringPolicy = Chest.ChestPolicies.FindPolicyOf<IAutowiringConstructorArgumentTypesPolicy>();

            foreach (PropertyInfo property in ReflectionExplorer.GetPublicInstancePropertiesForType(
                registrationHandler.Registration.ImplType))
            {
                // only read-write properties can be injected
                if (!(property.CanRead && property.CanWrite))
                    continue;

                // indexer properties are ignored
                if (property.GetIndexParameters().Length > 0)
                    continue;

                if (!autowiringPolicy.ShouldArgumentTypeBeAutowired(property.PropertyType))
                    continue;

                object existingPropertyValue;
                try
                {
                    existingPropertyValue = property.GetValue(instance, null);
                }
                catch (NotImplementedException)
                {
                    // this is to prevent Mono problems
                    continue;
                }
                catch (TargetException ex)
                {
                    throw ex;
                }

                if (existingPropertyValue == null)
                {
                    if (Chest.HasService(property.PropertyType))
                    {
                        ServiceRegistration propertyTypeRegistration = Chest.ServicesRegistry
                            .GetFirstRegistrationForService(property.PropertyType);
                        if (!(propertyTypeRegistration.RegistrationHandler is SingletonLifestyle))
                            continue;

                        object propertyValue = Chest.Fetch(property.PropertyType).Instance;
                        property.SetValue(instance, propertyValue, null);
                    }
                }
            }            
        }
    }
}