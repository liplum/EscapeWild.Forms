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
    public partial class FirePage : ContentPage
    {
        private readonly Player _player;

        public FirePage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["Player"];
            _fuels = _player.GetFuelItems().ToList();
            RebuildPicker();
            UpdateUI();
        }

        private IList<IFuelItem> _fuels;

        private void RebuildPicker()
        {
            ItemsPicker.Items.Clear();
            foreach (var fuel in _fuels)
                ItemsPicker.Items.Add(fuel.LocalizedName());
        }

        private async void Throw_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _fuels.Count) return;
            var fuel = _fuels[index];
            _player.FireFuel += fuel.Fuel;
            _player.RemoveItem(fuel);
            _fuels = _player.GetFuelItems().ToList();
            if (_fuels.Count <= 0)
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
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _fuels.Count)
            {
                Throw.IsEnabled = false;
                ItemsPicker.SelectedItem = null;
                ItemDescription.Text = string.Empty;
            }
            else
            {
                Throw.IsEnabled = _player.CanPerformAnyAction;
                var fuel = _fuels[index];
                ItemDescription.Text = fuel.LocalizedDesc();
            }
        }
    }
}