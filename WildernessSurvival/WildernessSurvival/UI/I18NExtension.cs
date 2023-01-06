using System;
using WildernessSurvival.Localization;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival.UI
{
    public class I18NExtension : IMarkupExtension<string>
    {
        public string Key { get; set; } = "";

        string IMarkupExtension<string>.ProvideValue(IServiceProvider serviceProvider)
        {
            return I18N.Get(Key);
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return ((IMarkupExtension<string>)this).ProvideValue(serviceProvider);
        }
    }
}