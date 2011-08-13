using System;

namespace TreasureChest.Policies
{
    public interface IConstructionPolicy : IGlobalChestPolicy, ISingleInstancePolicy
    {
        bool CanBeCreated(Type type, ResolvingContext context);
        object CreateInstance(
            ServiceRegistration registration,
            ResolvingContext context);
    }
}