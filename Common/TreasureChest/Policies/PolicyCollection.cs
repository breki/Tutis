using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace TreasureChest.Policies
{
    [SuppressMessage ("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public interface IPolicyCollection
    {
        void AddPolicy(IPolicy policy);

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void AddPolicy<T> () where T : IPolicy, new ();
        IEnumerable<IPolicy> EnumerateAllPolicies();

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T FindPolicyOf<T> () where T : class, ISingleInstancePolicy;

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IEnumerable FindAllPoliciesOf<T> () where T : IMultipleInstancePolicy;

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        bool HasPoliciesOf<T> () where T : IPolicy;
        bool HasPoliciesOf(Type policyType);
    }

    [SuppressMessage ("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class PolicyCollection : IPolicyCollection
    {
        public virtual void AddPolicy(IPolicy policy)
        {
            lock (this)
            {
                Type policyType = policy.GetType();
                if (policyType is ISingleInstancePolicy)
                {
                    if (HasPoliciesOf(policyType))
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            "There is already a policy of type {0}. You can only define one such policy.",
                            policyType.Name);
                        throw new InvalidOperationException(message);
                    }
                }

                policies.Add(policy);

                allPoliciesOf.Clear();
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void AddPolicy<T> () where T : IPolicy, new ()
        {
            AddPolicy(new T());
        }

        public IEnumerable<IPolicy> EnumerateAllPolicies()
        {
            return policies;
        }

        public void CopyPoliciesFrom (IPolicyCollection policyCollection)
        {
            lock (this)
            {
                foreach (IPolicy policy in policyCollection.EnumerateAllPolicies())
                    policies.Add(policy);
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T FindPolicyOf<T> () where T : class, ISingleInstancePolicy
        {
            lock (this)
            {
                T policy = (T)policies.Find(x => x is T);
                if (policy == null)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "There is no registered policy of type {0}.",
                        typeof(T).FullName);
                    throw new KeyNotFoundException(message);
                }

                return policy;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IEnumerable FindAllPoliciesOf<T> () where T : IMultipleInstancePolicy
        {
            lock (this)
            {
                Type type = typeof(T);
                if (allPoliciesOf.ContainsKey(type))
                    return allPoliciesOf[type];

                ArrayList matchingPolicies = new ArrayList();
                foreach (IPolicy policy in policies)
                {
                    if (policy is T)
                        matchingPolicies.Add(policy);
                }

                allPoliciesOf[type] = matchingPolicies;
                return matchingPolicies;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool HasPoliciesOf<T> () where T : IPolicy
        {
            lock (this)
                return policies.Find(x => x is T) != null;
        }

        public bool HasPoliciesOf(Type policyType)
        {
            lock (this)
                return policies.Find(x => policyType.IsAssignableFrom(x.GetType())) != null;
        }

        private List<IPolicy> policies = new List<IPolicy>();
        private Dictionary<Type, ArrayList> allPoliciesOf = new Dictionary<Type, ArrayList>();
    }
}