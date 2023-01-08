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
        private static Player Player => App.Player;

        private static IList<IItem> AllItems => Player.AllItems;

        public CraftPage()
        {
            InitializeComponent();
            _recipe2Preview = Core.Craft.TestAvailableRecipes(Player.Backpack);
            RebuildPicker();
            HealthProgressBar.Progress = Player.Health;
            FoodProgressBar.Progress = Player.Food;
            WaterProgressBar.Progress = Player.Water;
            EnergyProgressBar.Progress = Player.Energy;
            UpdateUI();
        }

        private IList<(IRecipe recipe, IItem preview)> _recipe2Preview;

        private void RebuildPicker()
        {
            ItemsPicker.Items.Clear();
            foreach (var (_, output) in _recipe2Preview)
                ItemsPicker.Items.Add(output.LocalizedName());
        }

        private async void Craft_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= _recipe2Preview.Count) return;
            var (recipe, _) = _recipe2Preview[index];
            var output = recipe.ConsumeAndCraft(Player.Backpack);
            var builder = new AttrModifierBuilder();
            recipe.BuildCraftAttrRequirements(builder);
            builder.PerformModification(Player.Attrs);
            Player.AddItem(output);
            _recipe2Preview = Core.Craft.TestAvailableRecipes(Player.Backpack);
            // Player might die from exhaustion.
            if (Player.IsDead)
            {
                UpdateUI();
                await Navigation.PopModalAsync();
                return;
            }

            if (_recipe2Preview.Count <= 0)
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
            if (index < 0 || index >= _recipe2Preview.Count)
            {
                Craft.IsEnabled = false;
                ItemsPicker.SelectedItem = null;
                ItemDescription.Text = string.Empty;
                AfterCraftArea.IsVisible = false;
            }
            else
            {
                Craft.IsEnabled = Player.CanPerformAnyAction;
                var (recipe, preview) = _recipe2Preview[index];
                ItemDescription.Text = preview.LocalizedDesc();
                var builder = new AttrModifierBuilder();
                recipe.BuildCraftAttrRequirements(builder);
                if (builder.HasAnyEffect)
                {
                    var mock = new DefaultAttributeModel
                    {
                        Health = Player.Health,
                        Food = Player.Food,
                        Water = Player.Water,
                        Energy = Player.Energy,
                    };
                    builder.PerformModification(new AttributeManager(mock));
                    AfterCraftArea.IsVisible = true;
                    HealthProgressBar.ProgressTo(mock.Health, 300, Easing.Linear);
                    FoodProgressBar.ProgressTo(mock.Food, 300, Easing.Linear);
                    WaterProgressBar.ProgressTo(mock.Water, 300, Easing.Linear);
                    EnergyProgressBar.ProgressTo(mock.Energy, 300, Easing.Linear);
                }
                else
                {
                    AfterCraftArea.IsVisible = false;
                }
            }
        }
    }
}