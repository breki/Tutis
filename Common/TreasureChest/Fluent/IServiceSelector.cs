using System;
using System.Diagnostics.CodeAnalysis;

namespace TreasureChest.Fluent
{
    public interface IServiceSelector
    {
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IServiceAction<T> Service<T> ();
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ServiceMultiImplementationsAction<T> AllImplementationsOf<T> ();
        IForEachClass ForEachClass(Func<Type, bool> predicate);
    }
}