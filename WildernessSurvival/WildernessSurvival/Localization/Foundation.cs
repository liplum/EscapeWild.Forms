using System.Collections.Generic;

namespace WildernessSurvival.Localization
{
    public interface ILocalization
    {
        Dictionary<string, string> TranslationKey2Localized { get; }
    }

    public static class I18N
    {
        public static ILocalization Localization;

        public static string Get(string key)
        {
            if (Localization is null) return key;
            return Localization.TranslationKey2Localized.TryGetValue(key, out var localized) ? localized : key;
        }
    }
}