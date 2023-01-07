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
        private readonly Player _player;

        public CookPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["Player"];
            RebuildPicker();
        }

        private IList<(ICookableItem raw, IItem cooked)> _raw2Cooked;

        private void RebuildPicker()
        {
            _raw2Cooked = (from cookable in _player.GetCookableItems() select (cookable, cookable.Cook())).ToList();
            ItemsPicker.Items.Clear();
            foreach (var (_, cooked) in _raw2Cooked)
                ItemsPicker.Items.Add(cooked.LocalizedName());
        }

        private async void Cook_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _raw2Cooked.Count) return;
            if (!_player.HasWood) return;
            var (raw, cooked) = _raw2Cooked[index];
            _player.RemoveItem(raw);
            _player.ConsumeWood(1);
            _player.AddItem(cooked);
            if (!_player.GetCookableItems().Any())
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

        private void RawItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
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
                Cook.IsEnabled = _player.CanPerformAnyAction && _player.HasWood;
                var (raw,cooked) = _raw2Cooked[index];
                Cook.Text = _player.HasWood ? _i18n(raw.CookType.ToString()) : _i18n("NoWood");

                ItemDescription.Text =
                    string.Format(_i18n($"After{raw.CookType}"), cooked.LocalizedName());
            }
        }

        private static string _i18n(string key)
        {
            return I18N.Get($"Cook.{key}");
        }
    }
}