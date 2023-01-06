using System.Collections.Generic;

namespace WildernessSurvival.Localization
{
    public interface ILocalization
    {
        Dictionary<string, string> TranslationKey2Localized { get; }
    }
}