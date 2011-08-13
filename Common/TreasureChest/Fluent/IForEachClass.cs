using System;
using System.Diagnostics.CodeAnalysis;

namespace TreasureChest.Fluent
{
    public interface IForEachClass
    {
        [SuppressMessage ("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Do")]
        IChestFilling Do (Action<IChestMaster, Type> registrationAction);
    }
}