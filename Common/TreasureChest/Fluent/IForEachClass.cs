using System;

namespace TreasureChest.Fluent
{
    public interface IForEachClass
    {
        IChestFilling Do(Action<IChestMaster, Type> registrationAction);
    }
}