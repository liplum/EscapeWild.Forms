using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;
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
            Use.IsEnabled = _player.CanPerformAnyAction && selected is IUsableItem;
        }
    }
}