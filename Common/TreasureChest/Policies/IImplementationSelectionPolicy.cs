using System;
using System.Collections.Generic;

namespace TreasureChest.Policies
{
    public interface IImplementationSelectionPolicy : ISingleInstancePolicy
    {
        Type SelectImplementation(Type serviceType, IList<Type> candidateImplementations);
    }
}