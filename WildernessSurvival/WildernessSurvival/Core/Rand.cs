using System;

namespace WildernessSurvival.Core
{
    public static class Rand
    {
        private static readonly Random Random = new Random();

        public static int Int(int maxValue) => Random.Next(maxValue);
    }
}