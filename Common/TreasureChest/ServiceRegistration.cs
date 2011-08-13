using System;
using System.Collections.Generic;
using System.Globalization;
using TreasureChest.Policies;
using TreasureChest.Policies.ServicePolicies;

namespace TreasureChest
{
    public class ServiceRegistration : PolicyCollection
    {
        public ServiceRegistration()
        {
        }

        public ServiceRegistration(Type serviceType, Type implType, IRegistrationHandler registrationHandler)
        {
            if (!serviceType.IsAssignableFrom(implType))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Type {0} is not an implementation of service type {1}.",
                    implType.FullName,
                    serviceType.FullName);
                throw new ArgumentException(message);
            }

            serviceTypes.Add(serviceType);
            this.implType = implType;
            this.registrationHandler = registrationHandler;
            registrationHandler.Initialize(this);
        }

        public ServiceRegistration(
            IEnumerable<Type> serviceTypes, 
            Type implType, 
            IRegistrationHandler registrationHandler)
        {
            this.serviceTypes = new HashSet<Type>(serviceTypes);
            this.implType = implType;
            this.registrationHandler = registrationHandler;
            registrationHandler.Initialize(this);
        }

        public IDictionary<string, object> Args
        {
            get { return args; }
            set { args = value; }
        }

        public Type FirstServiceType
        {
            get
            {
                foreach (Type serviceType in ServiceTypes)
                    return serviceType;

                throw new InvalidOperationException("BUG");
            }
        }

        public bool UsesCustomCreationMethod
        {
            get { return HasPoliciesOf<IComponentCreationMethod>(); }
        }

        public HashSet<Type> ServiceTypes
        {
            get { return serviceTypes; }
            set { serviceTypes = value; }
        }

        public Type ImplType
        {
            get { return implType; }
            set { implType = value; }
        }

        public IRegistrationHandler RegistrationHandler
        {
            get { return registrationHandler; }
            set { registrationHandler = value; }
        }

        public int ServiceTypesCount    
        {
            get { return serviceTypes.Count; }
        }

        public bool CoversService(Type serviceType)
        {
            return serviceTypes.Contains(serviceType);
        }

        public ServiceRegistration CreateFromPrototype(
            Type serviceType, 
            Type implType, 
            IRegistrationHandler registrationHandler)
        {
            ServiceRegistration clone = new ServiceRegistration (
                serviceType, 
                implType, 
                registrationHandler);
            clone.args = args;
            clone.CopyPoliciesFrom(this);
            return clone;
        }

        public ServiceRegistration CreateFromPrototype(
            IEnumerable<Type> serviceTypes,
            Type implType,
            IRegistrationHandler registrationHandler)
        {
            ServiceRegistration clone = new ServiceRegistration(
                serviceTypes,
                implType,
                registrationHandler);
            clone.args = args;
            clone.CopyPoliciesFrom(this);
            return clone;
        }

        public object CreateInstanceUsingCustomCreationMethod(IChest chest)
        {
            IComponentCreationMethod creationMethod = FindPolicyOf<IComponentCreationMethod>();
            return creationMethod.Create();
        }

        private IDictionary<string, object> args = new Dictionary<string, object>();
        private Type implType;
        private IRegistrationHandler registrationHandler;
        private HashSet<Type> serviceTypes = new HashSet<Type>();
    }
}