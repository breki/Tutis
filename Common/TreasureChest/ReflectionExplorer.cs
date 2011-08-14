using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TreasureChest
{
    public class ReflectionExplorer
    {
        public void RegisterAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void RegisterAssemblyOf<T> ()
        {
            assemblies.Add(typeof(T).Assembly);
        }

        public IEnumerable<Type> EnumerateAllNonAbstractClasses()
        {
            if (assemblies.Count == 0)
                throw new InvalidOperationException("No assemblies are registered.");

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in GetAssemblyTypes(assembly))
                {
                    if (type.IsClass && !type.IsAbstract)
                        yield return type;
                }
            }                        
        }

        public IList<Type> FindImplementationsOfTypes(params Type[] types)
        {
            if (assemblies.Count > 0)
                return FindImplementationsOfTypes(assemblies, types);

            Dictionary<Assembly, object> assembliesFromTypes = new Dictionary<Assembly, object>();
            foreach (Type type in types)
                assembliesFromTypes[type.Assembly] = true;

            return FindImplementationsOfTypes(assembliesFromTypes.Keys, types);
        }

        public IList<Type> FindImplementationsOfTypes(
            IEnumerable<Assembly> assemblies, 
            params Type[] types)
        {
            List<Type> implementingTypes = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type assemblyType in GetAssemblyTypes(assembly))
                {
                    if (assemblyType.IsAbstract || assemblyType.IsInterface)
                        continue;

                    bool matchedAllTypes = true;
                    foreach (Type targetType in types)
                    {
                        if (!targetType.IsAssignableFrom(assemblyType))
                        {
                            matchedAllTypes = false;
                            break;
                        }
                    }

                    if (matchedAllTypes)
                        implementingTypes.Add(assemblyType);
                }
            }

            implementingTypes.Sort((a, b) => String.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
            return implementingTypes;
        }

        public IEnumerable<Type> GetAssemblyTypes (Assembly assembly)
        {
            lock (this)
            {
                if (!assembliesTypes.ContainsKey(assembly))
                    assembliesTypes[assembly] = assembly.GetTypes();

                return assembliesTypes[assembly];
            }
        }

        public static IEnumerable<PropertyInfo> GetPublicInstancePropertiesForType(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private HashSet<Assembly> assemblies = new HashSet<Assembly>();
        private Dictionary<Assembly, Type[]> assembliesTypes = new Dictionary<Assembly, Type[]>();
    }
}