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

        private readonly IList<IRawItem> _allRawItems;

        public CookPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["player"];
            _allRawItems = _player.RawItems;
            UpdateUI();
            foreach (var item in _allRawItems)
                RawItemsPicker.Items.Add(item.LocalizedName());
        }

        private async void Heat_Clicked(object sender, EventArgs e)
        {
            var index = RawItemsPicker.SelectedIndex;
            if (index < 0) return;
            if (!_player.HasWood) return;
            var rawItem = _allRawItems[index];
            IItem cooked = rawItem.Cook();
            _player.RemoveItem(rawItem);
            _player.ConsumeWood(1);
            _player.AddItem(cooked);
            UpdateUI();
            Cook.IsEnabled = false;
            await Task.Delay(500);
            await Navigation.PopModalAsync();
        }

        private void RawItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = _allRawItems[RawItemsPicker.SelectedIndex];
            ItemDescription.Text = selected.RawDescription;
            UpdateUI();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            Cook.IsEnabled = _player.CanPerformAnyAction && _player.HasWood && RawItemsPicker.SelectedIndex > 0;
            Cook.Text = _i18n(_player.HasWood ? "Cook" : "NoWood");
        }

        private static string _i18n(string key)
        {
            return I18N.Get($"Cook.{key}");
        }
    }
}