using System;
using System.Collections.Generic;
using System.Linq;

namespace TreasureChest.Policies
{
    public class DefaultAutowiringConstructorArgumentTypesPolicy : GlobalChestPolicyBase, 
        IAutowiringConstructorArgumentTypesPolicy
    {
        public bool ShouldArgumentTypeBeAutowired(Type argumentType)
        {
            if (argumentType.IsValueType)
                return false;
            if (argumentType == typeof(string))
                return false;
            if (argumentType.Namespace == "System")
                return false;

            Type innerEnumerableType;
            if (argumentType.IsGenericEnumerable(out innerEnumerableType))
                return ShouldArgumentTypeBeAutowired(innerEnumerableType);

            return true;
        }
    }
}