using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TreasureChest
{
    public interface IChest : IDisposable, ILeaseReturning
    {
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        Lease<T> Fetch<T> ();
        Lease<object> Fetch(Type serviceType);
        Lease<object> Fetch(Type serviceType, IDictionary<string, object> args);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IEnumerable<T> FetchAll<T> ();
        IEnumerable FetchAll(Type serviceType);
    }
}