using System;

namespace WildernessSurvival.Utils
{
    public static class Extension
    {
        public static T CoerceIn<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }
    }
}