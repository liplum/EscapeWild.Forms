using System;
using System.Collections.Generic;
using WildernessSurvival.Core;

namespace WildernessSurvival.Utils
{
    public static class Extension
    {
        public static T CoerceIn<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rand.Int(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}