using System.Globalization;
using WildernessSurvival.Localization;
using Xamarin.Forms;

namespace WildernessSurvival
{
    public partial class App : Application
    {
        public App()
        {
            I18N.RegisterLocalization(new LangEn(), isDefault: true);
            I18N.RegisterLocalization(new LangZhCn());
            I18N.SetCulture(CultureInfo.CurrentUICulture);
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}