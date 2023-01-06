using System;
using Xamarin.Forms;

namespace WildernessSurvival.UI
{
    public static class Curves
    {
        public static readonly Easing FastLinearToSlowEaseIn = new Cubic(0.18, 1.0, 0.04, 1.0).toEasing();
    }

    public static class EasingHelper
    {
        public static Easing Reversed(this Easing raw) => new Easing(x => 1.0 - raw.Ease(x));
    }

    public class Cubic
    {
        private readonly double _a;

        private readonly double _b;

        private readonly double _c;

        private readonly double _d;

        private const double CubicErrorBound = 0.001;

        public Cubic(double a, double b, double c, double d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        public Easing toEasing() => new Easing(TransformInternal);

        private static double _evaluateCubic(double a, double b, double m)
        {
            return 3 * a * (1 - m) * (1 - m) * m +
                   3 * b * (1 - m) * m * m +
                   m * m * m;
        }

        private double TransformInternal(double t)
        {
            var start = 0.0;
            var end = 1.0;
            while (true)
            {
                var midpoint = (start + end) / 2;
                var estimate = _evaluateCubic(_a, _c, midpoint);
                if (Math.Abs(t - estimate) < CubicErrorBound)
                {
                    return _evaluateCubic(_b, _d, midpoint);
                }

                if (estimate < t)
                {
                    start = midpoint;
                }
                else
                {
                    end = midpoint;
                }
            }
        }
    }
}