using System;
using System.Collections.Generic;
using WildernessSurvival.game;
using WildernessSurvival.UI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackpackPage : ContentPage
    {
        private static readonly Player player = (Player)Application.Current.Resources["player"];

        private static IList<ItemBase> AllItems;

        public BackpackPage()
        {
            InitializeComponent();
            AllItems = player.AllItems;
            foreach (var item in AllItems)
                ItemsPicker.Items.Add(item.ToString());
        }

        private async void Use_Clicked(object sender, EventArgs e)
        {
            var index = ItemsPicker.SelectedIndex;
            if (index != -1)
            {
                var item = AllItems[index];
                if (player.Use(item))
                {
                    DependencyService.Get<IToast>().ShortAlert($"你成功使用了{item}");
                    player.Remove(item);
                    await Navigation.PopModalAsync();
                }
                else
                {
                    DependencyService.Get<IToast>().ShortAlert("该物品无法直接使用");
                }
            }
            else
            {
                await DisplayAlert("提示", "请选择物品！", "好的");
            }
        }

        private void ItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemDescription.Text = AllItems[ItemsPicker.SelectedIndex].Description;
        }
    }
}