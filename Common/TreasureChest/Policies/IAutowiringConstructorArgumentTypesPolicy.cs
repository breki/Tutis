using System;

namespace TreasureChest.Policies
{
    public interface IAutowiringConstructorArgumentTypesPolicy : ISingleInstancePolicy
    {
        bool ShouldArgumentTypeBeAutowired(Type argumentType);
    }
}