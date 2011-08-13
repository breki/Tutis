using System;
using System.Collections.Generic;
using System.Linq;

namespace TreasureChest.Fluent
{
    public class ForEachClass : IForEachClass
    {
        public ForEachClass(IChestMaster chest, Func<Type, bool> predicate)
        {
            this.chest = chest;
            this.predicate = predicate;
        }

        public IChestFilling Do(Action<IChestMaster, Type> registrationAction)
        {
            IEnumerable<Type> matchingClasses = chest.ReflectionExplorer.EnumerateAllNonAbstractClasses().AsQueryable()
                .Where(predicate);

            foreach (Type type in matchingClasses)
                registrationAction(chest, type);

            return chest;
        }

        private readonly IChestMaster chest;
        private readonly Func<Type, bool> predicate;
    }
}