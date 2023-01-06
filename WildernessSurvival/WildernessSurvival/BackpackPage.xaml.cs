using System;
using System.Collections.Generic;
using WildernessSurvival.Core;
using WildernessSurvival.UI;
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
            Use.IsEnabled = _player.CanPerformAnyAction;
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
            DependencyService.Get<IToast>().ShortAlert($"你成功使用了{item.LocalizedName()}");
            _player.RemoveItem(item);
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