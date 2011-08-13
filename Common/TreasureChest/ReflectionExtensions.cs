using System;
using System.Collections.Generic;

namespace TreasureChest
{
    public static class ReflectionExtensions
    {
        public static bool IsGenericEnumerable(this Type type, out Type innerType)
        {
            var genArgs = type.GetGenericArguments();
            if (genArgs.Length == 1 &&
                typeof(IEnumerable<>).MakeGenericType(genArgs).IsAssignableFrom(type))
            {
                innerType = genArgs[0];
                return true;
            }

            innerType = null;
            return type.BaseType != null && type.BaseType.IsGenericEnumerable(out innerType);
        }
    }
}