using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftPage : ContentPage
    {
        private readonly Player _player;

        public CraftPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["Player"];
            _recipe2Output = Core.Craft.TestAvailableRecipes(_player.Backpack);
            RebuildPicker();
            UpdateUI();
        }

        private IList<(IRecipe recipe, IItem output)> _recipe2Output;

        private void RebuildPicker()
        {
            ItemsPicker.Items.Clear();
            foreach (var (_, output) in _recipe2Output)
                ItemsPicker.Items.Add(output.LocalizedName());
        }

        private async void Craft_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _recipe2Output.Count) return;
            var (recipe, output) = _recipe2Output[index];
            recipe.ConsumeAndCraft(_player.Backpack);
            _player.AddItem(output);
            _recipe2Output = Core.Craft.TestAvailableRecipes(_player.Backpack);
            if (_recipe2Output.Count <= 0)
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
            if (index < 0 || index >= _recipe2Output.Count)
            {
                Craft.IsEnabled = false;
                ItemsPicker.SelectedItem = null;
                ItemDescription.Text = string.Empty;
            }
            else
            {
                Craft.IsEnabled = _player.CanPerformAnyAction;
                var (_, output) = _recipe2Output[index];
                ItemDescription.Text = output.LocalizedDesc();
            }
        }
    }
}