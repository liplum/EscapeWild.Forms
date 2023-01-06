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

        private readonly IList<IItem> _allItems;

        public BackpackPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["player"];
            _allItems = _player.AllItems;
            Use.IsEnabled = _player.CanPerformAnyAction && ItemsPicker.SelectedIndex > 0;
            foreach (var item in _allItems)
                ItemsPicker.Items.Add(item.LocalizedName());
            HealthProgressBar.Progress = _player.Health;
            FoodProgressBar.Progress = _player.Food;
            WaterProgressBar.Progress = _player.Water;
            EnergyProgressBar.Progress = _player.Energy;
        }

        private async void Use_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index < 0) return;
            var item = _allItems[index];
            if (!(item is IUsableItem i)) return;
            _player.UseItem(i);
            _player.RemoveItem(item);
            Use.IsEnabled = false;
            await Task.Delay(500);
            await Navigation.PopModalAsync();
        }

        private void ItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = _allItems[ItemsPicker.SelectedIndex];
            ItemDescription.Text = selected.LocalizedDesc();

            if (selected is IUsableItem item)
            {
                Use.IsEnabled = _player.CanPerformAnyAction;
                Use.Text = $"Backpack.{item.UseType}".Tr();
                AfterUsedLabel.Text = $"Backpack.After{item.UseType}".Tr();
                var builder = new UseEffectBuilder();
                item.BuildUseEffect(builder);
                if (builder.HasAnyEffect)
                {
                    var mock = new MockPlayerAcceptUseEffect
                    {
                        Health = _player.Health,
                        Food = _player.Food,
                        Water = _player.Water,
                        Energy = _player.Energy,
                    };
                    builder.PerformUseEffects(mock);
                    AfterUsedArea.IsVisible = true;
                    HealthProgressBar.ProgressTo(mock.Health, 300, Easing.Linear);
                    FoodProgressBar.ProgressTo(mock.Food, 300, Easing.Linear);
                    WaterProgressBar.ProgressTo(mock.Water, 300, Easing.Linear);
                    EnergyProgressBar.ProgressTo(mock.Energy, 300, Easing.Linear);
                }
                else
                {
                    AfterUsedArea.IsVisible = false;
                }
            }
            else
            {
                AfterUsedArea.IsVisible = false;
                Use.IsEnabled = false;
                Use.Text = "Backpack.CannotUse".Tr();
                AfterUsedLabel.Text = $"Backpack.After{UseType.Use}".Tr();
            }
        }
    }
}