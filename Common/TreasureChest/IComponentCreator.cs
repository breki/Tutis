using System;

namespace TreasureChest
{
    public interface IComponentCreator
    {
        bool CanBeCreated(Type type, ResolvingContext context);
        
        object CreateInstance(
            ServiceRegistration registration,
            ResolvingContext context);
    }
}