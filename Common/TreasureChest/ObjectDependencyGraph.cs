using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class ObjectDependencyGraph
    {
        public ObjectDependencyGraph(PolicyCollection chestPolicies)
        {
            this.chestPolicies = chestPolicies;
        }

        public int Count
        {
            get
            {
                lock (this)
                    return objectsMap.Count;
            }
        }

        public void AddInstanceToMap(
            object instance, 
            IRegistrationHandler registrationHandler,
            IEnumerable<object> dependencies)
        {
            lock (this)
            {
                objectsMap.Add(instance, registrationHandler);

                AddIsNecessaryForRelation(instance, null);
                if (dependencies != null)
                {
                    isDependentOn.Add(instance, new List<object>(dependencies));
                    foreach (object dependency in dependencies)
                        AddIsNecessaryForRelation(dependency, instance);
                }
                else
                    isDependentOn.Add(instance, null);

                foreach (IAfterComponentCreatedPolicy policy in chestPolicies.FindAllPoliciesOf<IAfterComponentCreatedPolicy> ())
                    policy.AfterCreated (instance, registrationHandler);
            }
        }

        public IRegistrationHandler GetRegistrationHandlerForInstance (object instance)
        {
            lock (this)
                return objectsMap[instance];
        }

        public void RegisterDependency(object dependencyInstance, object dependentInstance)
        {
            lock (this)
            {
                if (!objectsMap.ContainsKey(dependentInstance))
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Dependency instance of type {0} is not registered in the object dependency graph so the dependency from {1} cannot be registered.",
                        dependencyInstance.GetType().FullName,
                        dependentInstance.GetType().FullName);
                    throw new ChestException(message);     
                }

                if (!objectsMap.ContainsKey (dependentInstance))
                {
                    string message = string.Format (
                        CultureInfo.InvariantCulture,
                        "Dependent instance of type {0} is not registered in the object dependency graph so the dependency to {1} cannot be registered.",
                        dependentInstance.GetType ().FullName,
                        dependencyInstance.GetType ().FullName);
                    throw new ChestException (message);
                }

                if (isDependentOn[dependentInstance] == null)
                    isDependentOn[dependentInstance] = new List<object>();

                isDependentOn[dependentInstance].Add(dependencyInstance);

                AddIsNecessaryForRelation (dependencyInstance, dependentInstance);
            }
        }

        public void RemoveInstance (object instance, bool destroyIt)
        {
            lock (this)
            {
                List<object> objectsDependentOn = isNecessaryFor[instance];
                if (objectsDependentOn != null && objectsDependentOn.Count > 0)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot remove an instance of {0} because an instance of {1} depends on it.",
                        instance.GetType().FullName,
                        objectsDependentOn[0].GetType().FullName);
                    throw new ChestException(message);
                }

                IRegistrationHandler handler = objectsMap[instance];
                objectsMap.Remove(instance);

                List<object> dependencies = isDependentOn[instance];
                if (dependencies != null)
                {
                    foreach (object dependency in dependencies)
                    {
                        RemoveIsNecessaryForRelation(dependency, instance);
                        if (GetRegistrationHandlerForInstance(dependency)
                            .MarkInstanceAsReleased(dependency, chestPolicies))
                            RemoveInstance(dependency, destroyIt);
                    }
                }

                isNecessaryFor.Remove(instance);
                isDependentOn.Remove(instance);
                handler.DestroyInstance(instance, chestPolicies);
            }
        }

        [SuppressMessage ("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "servicesRegistry")]
        public void DestroyAllInstances (IServicesRegistry servicesRegistry)
        {
            lock (this)
            {
                List<object> objectsToDestroy = new List<object>(objectsMap.Keys);

                while (objectsToDestroy.Count > 0)
                {
                    int i = 0;
                    bool destroyedAnythingThisRound = false;

                    while (i < objectsToDestroy.Count)
                    {
                        object instanceToDestroy = objectsToDestroy[i];

                        if (!isNecessaryFor.ContainsKey(instanceToDestroy))
                        {
                            objectsToDestroy.RemoveAt(i);
                            destroyedAnythingThisRound = true;
                            continue;
                        }

                        List<object> dependencies = isNecessaryFor[instanceToDestroy];
                        if (dependencies == null || dependencies.Count == 0)
                        {
                            RemoveInstance(instanceToDestroy, true);
                            objectsToDestroy.RemoveAt(i);
                            destroyedAnythingThisRound = true;
                        }
                        else
                            i++;
                    }

                    if (false == destroyedAnythingThisRound)
                    {
                        throw new ChestException("Looks like there is a cycle in the dependency graph.");
                    }
                }

                if (objectsMap.Count != 0 || isDependentOn.Count != 0 || isNecessaryFor.Count != 0)
                    throw new InvalidOperationException("BUG");
            }
        }

        private void AddIsNecessaryForRelation(object isNecessary, object instance)
        {
            if (!isNecessaryFor.ContainsKey(isNecessary))
                isNecessaryFor.Add(isNecessary, new List<object>());

            if (instance != null)
                isNecessaryFor[isNecessary].Add(instance);
        }

        private void RemoveIsNecessaryForRelation(object isNecessary, object instance)
        {
            isNecessaryFor[isNecessary].Remove(instance);
        }

        private readonly PolicyCollection chestPolicies;
        private Dictionary<object, IRegistrationHandler> objectsMap = new Dictionary<object, IRegistrationHandler>();
        private Dictionary<object, List<object>> isDependentOn = new Dictionary<object, List<object>>();
        private Dictionary<object, List<object>> isNecessaryFor = new Dictionary<object, List<object>>();
    }
}