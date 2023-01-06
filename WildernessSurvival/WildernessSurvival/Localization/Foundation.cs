using System.Collections.Generic;
using System.Globalization;

namespace WildernessSurvival.Localization
{
    public interface ILocalization
    {
        CultureInfo BoundCulture { get; }
        Dictionary<string, string> TranslationKey2Localized { get; }
    }

    public static class I18N
    {
        private static ILocalization _curLocalization;
        private static ILocalization _defaultLocalization;

        private static readonly Dictionary<CultureInfo, ILocalization> Culture2Localization =
            new Dictionary<CultureInfo, ILocalization>();

        public static void RegisterLocalization(ILocalization localization, bool isDefault = false)
        {
            Culture2Localization[localization.BoundCulture] = localization;
            if (isDefault)
            {
                _defaultLocalization = localization;
            }
        }

        public static void SetCulture(CultureInfo culture)
        {
            ILocalization matched = null;
            var maxScore = 0;
            foreach (var p in Culture2Localization)
            {
                var score = MatchScore(target: culture, test: p.Key);
                if (score > maxScore)
                {
                    maxScore = score;
                    matched = p.Value;
                }
            }

            _curLocalization = matched ?? _defaultLocalization;
        }

        private static int MatchScore(CultureInfo target, CultureInfo test)
        {
            if (target.Name.Equals(test.Name))
            {
                // the same culture
                return 100;
            }

            if (target.Parent.Name.Equals(test.Parent.Name))
            {
                // has the same parent
                return 90;
            }

            // not matched
            return 0;
        }

        public static string Get(string key)
        {
            if (_curLocalization is null) return key;
            return _curLocalization.TranslationKey2Localized.TryGetValue(key, out var localized) ? localized : key;
        }
    }
}