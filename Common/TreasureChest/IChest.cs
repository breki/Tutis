using System;
using System.Collections;
using System.Collections.Generic;

namespace TreasureChest
{
    public interface IChest : IDisposable, ILeaseReturning
    {
        Lease<T> Fetch<T>();
        Lease<object> Fetch(Type serviceType);
        Lease<object> Fetch(Type serviceType, IDictionary<string, object> args);
        IEnumerable<T> FetchAll<T>();
        IEnumerable FetchAll(Type serviceType);
    }
}