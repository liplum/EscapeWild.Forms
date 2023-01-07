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
        public static bool EnableFallbackToDefault = false;

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

            if (target.TwoLetterISOLanguageName.Equals(test.TwoLetterISOLanguageName))
            {
                // has the same parent
                return 80;
            }

            // not matched
            return 10;
        }

        public static string Get(string key)
        {
            if (_curLocalization != null &&
                _curLocalization.TranslationKey2Localized.TryGetValue(key, out var localized))
            {
                return localized;
            }

            if (EnableFallbackToDefault && _defaultLocalization != null)
            {
                return _defaultLocalization.TranslationKey2Localized.TryGetValue(key, out localized)
                    ? localized
                    : key;
            }

            return key;
        }

        public static string Tr(this string key) => Get(key);
        public static string Tr(this string key, object arg0) => string.Format(Get(key), arg0);
        public static string Tr(this string key, object arg0, object arg1) => string.Format(Get(key), arg0, arg1);

        public static string Tr(this string key, object arg0, object arg1, object arg2) =>
            string.Format(Get(key), arg0, arg1, arg2);

        public static string Tr(this string key, params object[] args) => string.Format(null, Get(key), args);
    }
}