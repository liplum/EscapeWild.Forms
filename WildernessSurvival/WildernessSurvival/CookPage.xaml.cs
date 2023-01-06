using System;
using System.Collections.Generic;
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

        private IList<IRawItem> AllRawItems => _player.RawItems;

        public CookPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["player"];
            RebuildPicker();
        }

        private void RebuildPicker()
        {
            ItemsPicker.Items.Clear();
            foreach (var item in AllRawItems)
                ItemsPicker.Items.Add(item.LocalizedName());
        }

        private async void Cook_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            var allRawItems = AllRawItems;
            if (index < 0 || index >= allRawItems.Count) return;
            if (!_player.HasWood) return;
            var rawItem = allRawItems[index];
            IItem cooked = rawItem.Cook();
            _player.RemoveItem(rawItem);
            _player.ConsumeWood(1);
            _player.AddItem(cooked);
            if (AllRawItems.Count <= 0)
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
            var allRawItems = AllRawItems;
            if (index < 0 || index >= allRawItems.Count)
            {
                Cook.Text = _i18n("Cook");
                Cook.IsEnabled = false;
                ItemDescription.Text = string.Empty;
            }
            else
            {
                Cook.IsEnabled = _player.CanPerformAnyAction && _player.HasWood;
                var selected = allRawItems[index];
                Cook.Text = _player.HasWood ? _i18n(selected.CookType.ToString()) : _i18n("NoWood");

                ItemDescription.Text =
                    string.Format(_i18n($"After{selected.CookType}"), selected.Cook().LocalizedName());
            }
        }

        private static string _i18n(string key)
        {
            return I18N.Get($"Cook.{key}");
        }
    }
}