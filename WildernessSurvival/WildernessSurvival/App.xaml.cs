using System.Globalization;
using WildernessSurvival.Game;
using WildernessSurvival.Localization;
using Xamarin.Forms;

namespace WildernessSurvival
{
    public partial class App : Application
    {
        static App()
        {
            I18N.EnableFallbackToDefault = true;
            I18N.RegisterLocalization(new LangEn(), isDefault: true);
            I18N.RegisterLocalization(new LangZhCn());
            // Add localization for another language here.
            // I18N.RegisterLocalization(new LangAnother());
            Recipes.RegisterAll();
        }

        public App()
        {
            I18N.SetCulture(CultureInfo.CurrentUICulture);
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: false, debugLogEnable: false);
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