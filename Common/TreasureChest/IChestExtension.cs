using System;
using TreasureChest.Policies;

namespace TreasureChest
{
    public interface IChestExtension : IGlobalChestPolicy, IDisposable
    {
    }
}