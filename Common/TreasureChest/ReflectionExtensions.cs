using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TreasureChest
{
    public static class ReflectionExtensions
    {
        [SuppressMessage ("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public static bool IsGenericEnumerable (this Type type, out Type innerType)
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