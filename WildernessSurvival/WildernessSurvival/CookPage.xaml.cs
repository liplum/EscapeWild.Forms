using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CookPage : ContentPage
    {
        private static Player Player => App.Player;

        private static IList<IItem> AllItems => Player.AllItems;

        public CookPage()
        {
            InitializeComponent();
            _raw2Cooked = (from cookable in Player.GetCookableItems() select (cookable, cookable.Cook())).ToList();
            RebuildPicker();
            UpdateUI();
        }

        private IList<(ICookableItem raw, IItem cooked)> _raw2Cooked;

        private void RebuildPicker()
        {
            ItemsPicker.Items.Clear();
            foreach (var (raw, _) in _raw2Cooked)
                ItemsPicker.Items.Add(raw.LocalizedName());
        }

        private async void Cook_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _raw2Cooked.Count) return;
            var (raw, cooked) = _raw2Cooked[index];
            if (Player.FireFuel < raw.FlueCost) return;
            Player.FireFuel -= raw.FlueCost;
            Player.RemoveItem(raw);
            Player.AddItem(cooked);
            _raw2Cooked = (from cookable in Player.GetCookableItems() select (cookable, cookable.Cook())).ToList();
            if (_raw2Cooked.Count <= 0)
            {
                UpdateUI();
                await Task.Delay(500);
                await Navigation.PopModalAsync();
                return;
            }

            RebuildPicker();
            if (ItemsPicker.Items.Count > 0)
            {
                // Go to the next item automatically
                ItemsPicker.SelectedIndex = index % ItemsPicker.Items.Count;
            }

            UpdateUI();
        }

        private void ItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            FireFuelProgress.ProgressTo(Player.FireFuelProgress, 300, Easing.Linear);
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _raw2Cooked.Count)
            {
                Cook.Text = _i18n("Cook");
                Cook.IsEnabled = false;
                ItemsPicker.SelectedItem = null;
                ItemDescription.Text = string.Empty;
            }
            else
            {
                Cook.IsEnabled = Player.CanPerformAnyAction;
                var (raw, cooked) = _raw2Cooked[index];
                var hasEnoughFuel = Player.FireFuel >= raw.FlueCost;
                Cook.IsEnabled &= hasEnoughFuel;
                Cook.Text = hasEnoughFuel ? _i18n(raw.CookType.ToString()) : _i18n("LowFuel");
                ItemDescription.Text = string.Format(_i18n($"After{raw.CookType}"), cooked.LocalizedName());
            }
        }

        private static string _i18n(string key)
        {
            return I18N.Get($"Cook.{key}");
        }
    }
}