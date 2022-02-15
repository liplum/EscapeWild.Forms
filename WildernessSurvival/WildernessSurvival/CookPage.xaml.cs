using System;
using System.Collections.Generic;
using WildernessSurvival.game;
using WildernessSurvival.UI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CookPage : ContentPage {
        public CookPage() {
            InitializeComponent();

            AllRawItems = player.RawItems;
            foreach (ItemBase item in AllRawItems)
                RawItemsPicker.Items.Add(item.ToString());
        }

        private static readonly Player player = (Player)Application.Current.Resources["player"];

        private static IList<IRawItem> AllRawItems;

        private async void Heat_Clicked(object sender, EventArgs e) {
            var index = RawItemsPicker.SelectedIndex;
            if (index != -1) {
                if (player.HasWood) {
                    var rawItem = AllRawItems[index];
                    ItemBase cooked = rawItem.Cook();
                    player.Remove((ItemBase)rawItem);
                    player.ConsumeWood(1);
                    player.AddItem(cooked);
                    DependencyService.Get<IToast>().ShortAlert($"你获得了{cooked}");

                    await Navigation.PopModalAsync();
                } else {
                    await DisplayAlert("提示", "你没有足够的木头！", "退出");
                    await Navigation.PopModalAsync();
                }

            } else {
                await DisplayAlert("提示", "请选择物品！", "好的");
            }
        }

        private void RawItemsPicker_SelectedIndexChanged(object sender, EventArgs e) {
            ItemDescription.Text = AllRawItems[RawItemsPicker.SelectedIndex].RawDescription;
        }
    }
}