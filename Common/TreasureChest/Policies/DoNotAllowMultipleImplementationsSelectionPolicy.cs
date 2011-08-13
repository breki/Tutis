using System;
using System.Collections.Generic;

namespace TreasureChest.Policies
{
    public class DoNotAllowMultipleImplementationsSelectionPolicy : GlobalChestPolicyBase, IImplementationSelectionPolicy
    {
        public Type SelectImplementation(Type serviceType, IList<Type> candidateImplementations)
        {
            if (candidateImplementations.Count == 0)
                throw TreasureChest.Chest.ChestException(
                    "There are no appropriate implementations for the service {0}",
                    serviceType.FullName);
            if (candidateImplementations.Count > 1)
            {
                throw TreasureChest.Chest.ChestException(
                    "There is more than on implementation for the service {0}, you have to specify the implementation explicitly.",
                    serviceType.FullName);
            }

            return candidateImplementations[0];
        }
    }
}