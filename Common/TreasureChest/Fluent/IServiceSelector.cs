using System;

namespace TreasureChest.Fluent
{
    public interface IServiceSelector
    {
        IServiceAction<T> Service<T>();
        ServiceMultiImplementationsAction<T> AllImplementationsOf<T>();
        IForEachClass ForEachClass(Func<Type, bool> predicate);
    }
}