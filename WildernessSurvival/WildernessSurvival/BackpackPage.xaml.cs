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
        private readonly Player _player;

        private IList<IItem> AllItems => _player.AllItems;

        public BackpackPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["Player"];
            Use.IsEnabled = _player.CanPerformAnyAction && ItemsPicker.SelectedIndex >= 0;
            RebuildPicker();
            HealthProgressBar.Progress = _player.Health;
            FoodProgressBar.Progress = _player.Food;
            WaterProgressBar.Progress = _player.Water;
            EnergyProgressBar.Progress = _player.Energy;
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
            if (!(item is IUsableItem i) || !i.CanUse(_player)) return;
            await _player.UseItem(i);
            _player.RemoveItem(item);
            // Player might die from poison.
            if (_player.IsDead)
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
                if (selected is IUsableItem item && item.CanUse(_player))
                {
                    UpdatePreview(item.DisplayPreview);
                    Use.IsEnabled = _player.CanPerformAnyAction;
                    Use.Text = $"Backpack.{item.UseType}".Tr();
                    AfterUseLabel.Text = $"Backpack.After{item.UseType}".Tr();
                    var builder = new AttrModifierBuilder();
                    item.BuildAttrModification(builder);
                    if (builder.HasAnyEffect)
                    {
                        var mock = new DefaultAttributeModel
                        {
                            Health = _player.Health,
                            Food = _player.Food,
                            Water = _player.Water,
                            Energy = _player.Energy,
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