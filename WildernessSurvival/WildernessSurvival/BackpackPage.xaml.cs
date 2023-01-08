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
    public partial class BackpackPage : ContentPage
    {
        private static Player Player => App.Player;

        private static IList<IItem> AllItems => Player.AllItems;

        public BackpackPage()
        {
            InitializeComponent();
            Use.IsEnabled = Player.CanPerformAnyAction && ItemsPicker.SelectedIndex >= 0;
            RebuildPicker();
            HealthProgressBar.Progress = Player.Health;
            FoodProgressBar.Progress = Player.Food;
            WaterProgressBar.Progress = Player.Water;
            EnergyProgressBar.Progress = Player.Energy;
            UpdateUI();
        }

        private void RebuildPicker()
        {
            ItemsPicker.Items.Clear();
            foreach (var item in AllItems)
                ItemsPicker.Items.Add(item.LocalizedName());
        }

        private async void Use_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0 || index >= AllItems.Count) return;
            var item = AllItems[index];
            if (!(item is IUsableItem i) || !i.CanUse(Player)) return;
            await Player.UseItem(i);
            var afterUsed = i.AfterUsed();
            Player.RemoveItem(item);
            if (afterUsed != null)
                Player.AddItem(afterUsed);
            // Player might die from poison.
            if (Player.IsDead)
            {
                UpdateUI();
                await Navigation.PopModalAsync();
                return;
            }

            if (AllItems.Count <= 0)
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

        private void UpdatePreview(bool display)
        {
            UnknownPreview.IsVisible = !display;
            AfterUsePreview.IsVisible = display;
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            var index = ItemsPicker.SelectedIndex;

            void Clear()
            {
                AfterUseArea.IsVisible = false;
                Use.IsEnabled = false;
                AfterUseLabel.Text = $"Backpack.After{UseType.Use}".Tr();
            }

            if (index < 0 || index >= AllItems.Count)
            {
                Clear();
                Use.Text = $"Backpack.{UseType.Use}".Tr();
                ItemDescription.Text = string.Empty;
                ItemsPicker.SelectedItem = null;
            }
            else
            {
                var selected = AllItems[index];
                ItemDescription.Text = selected.LocalizedDesc();
                if (selected is IUsableItem item && item.CanUse(Player))
                {
                    UpdatePreview(item.DisplayPreview);
                    Use.IsEnabled = Player.CanPerformAnyAction;
                    Use.Text = $"Backpack.{item.UseType}".Tr();
                    AfterUseLabel.Text = $"Backpack.After{item.UseType}".Tr();
                    var builder = new AttrModifierBuilder();
                    item.BuildAttrModification(builder);
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
                        AfterUseArea.IsVisible = true;
                        HealthProgressBar.ProgressTo(mock.Health, 300, Easing.Linear);
                        FoodProgressBar.ProgressTo(mock.Food, 300, Easing.Linear);
                        WaterProgressBar.ProgressTo(mock.Water, 300, Easing.Linear);
                        EnergyProgressBar.ProgressTo(mock.Energy, 300, Easing.Linear);
                    }
                    else
                    {
                        AfterUseArea.IsVisible = false;
                    }
                }
                else
                {
                    Clear();
                    Use.Text = "Backpack.CannotUse".Tr();
                }
            }
        }
    }
}