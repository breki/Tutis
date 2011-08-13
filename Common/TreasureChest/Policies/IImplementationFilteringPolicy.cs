using System;
using System.Collections.Generic;

namespace TreasureChest.Policies
{
    public interface IImplementationFilteringPolicy : ISingleInstancePolicy
    {
        IList<Type> FindImplementationsOfTypes(params Type[] types);
    }
}