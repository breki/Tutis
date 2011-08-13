using System;
using System.Collections.Generic;

namespace TreasureChest.Policies
{
    public class StandardImplementationFilteringPolicy : GlobalChestPolicyBase, IImplementationFilteringPolicy
    {
        public IList<Type> FindImplementationsOfTypes(
            params Type[] types)
        {
            return Chest.ReflectionExplorer.FindImplementationsOfTypes(types);
        }
    }
}