using System;

namespace WildernessSurvival.Core
{
    public static class Rand
    {
        private static readonly Random Random = new Random();
        public static bool Bool() => Random.Next(1) == 1;
        public static int Int() => Random.Next();
        public static int Int(int max) => Random.Next(max);
        public static int Int(int min, int max) => Random.Next(min, max);
        public static float Float() => (float)Random.NextDouble();
        public static float Float(float max) => (float)(Random.NextDouble() * max);
        public static float Float(float min, float max) => (float)(Random.NextDouble() * (max - min)) + min;
    }
}