using System;

namespace WildernessSurvival.Core
{
    public delegate float ValueFixer(float raw);

    public class Hardness
    {
        public ValueFixer AttrCostFix = e => e;
        public ValueFixer AttrBounceFix = e => e;
        public Func<float> JourneyLength = () => 50;
    }

    public static class HardnessTable
    {
        public static Hardness
            Easy = new Hardness
            {
                AttrCostFix = e => e * Rand.Float(0.5f, 0.8f),
                AttrBounceFix = e => e * Rand.Float(1.2f, 1.5f),
                JourneyLength = () => 40 * Rand.Float(0.9f, 1.1f),
            },
            Normal = new Hardness
            {
                AttrCostFix = e => e * Rand.Float(0.8f, 1.2f),
                AttrBounceFix = e => e * Rand.Float(0.8f, 1.2f),
                JourneyLength = () => 55 * Rand.Float(0.8f, 1.2f),
            },
            Hard = new Hardness
            {
                AttrCostFix = e => e * Rand.Float(1.1f, 1.5f),
                AttrBounceFix = e => e * Rand.Float(0.8f, 1.0f),
                JourneyLength = () => 70 * Rand.Float(0.8f, 1.2f),
            };
    }
}