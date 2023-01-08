using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirePage : ContentPage
    {
        private static Player Player => App.Player;

        private static IList<IItem> AllItems => Player.AllItems;

        public FirePage()
        {
            InitializeComponent();
            _fuels = Player.GetFuelItems().ToList();
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
            Player.FireFuel += fuel.Fuel;
            Player.RemoveItem(fuel);
            _fuels = Player.GetFuelItems().ToList();
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
            FireFuelProgress.ProgressTo(Player.FireFuelProgress, 300, Easing.Linear);
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _fuels.Count)
            {
                Throw.IsEnabled = false;
                ItemsPicker.SelectedItem = null;
                ItemDescription.Text = string.Empty;
            }
            else
            {
                Throw.IsEnabled = Player.CanPerformAnyAction;
                var fuel = _fuels[index];
                ItemDescription.Text = fuel.LocalizedDesc();
            }
        }
    }
}