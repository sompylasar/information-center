using System;
using System.Collections;
using System.Collections.Generic;

namespace LogicUtils
{

    public static class EnumerableExtensions
    {

        public static IEnumerable<Type> GetConstituentTypes(this IEnumerable target)
        {
            List<Type> result = new List<Type>();
            foreach (var item in target)
            {
                var type = item.GetType();
                if (!result.Contains(type))
                    result.Add(type);
            }
            return result.ToArray();
        }

    }

}