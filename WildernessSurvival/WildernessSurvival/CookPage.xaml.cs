﻿using System;
using System.Collections.Generic;
using WildernessSurvival.Game;
using WildernessSurvival.UI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CookPage : ContentPage
    {
        private static readonly Player player = (Player)Application.Current.Resources["player"];

        private static IList<IRawItem> AllRawItems;

        public CookPage()
        {
            InitializeComponent();

            AllRawItems = player.RawItems;
            foreach (IItem item in AllRawItems)
                RawItemsPicker.Items.Add(item.LocalizedName());
        }

        private async void Heat_Clicked(object sender, EventArgs e)
        {
            var index = RawItemsPicker.SelectedIndex;
            if (index != -1)
            {
                if (player.HasWood)
                {
                    var rawItem = AllRawItems[index];
                    IItem cooked = rawItem.Cook();
                    player.Remove((IItem)rawItem);
                    player.ConsumeWood(1);
                    player.AddItem(cooked);
                    DependencyService.Get<IToast>().ShortAlert($"你获得了{cooked.LocalizedName()}");

                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("提示", "你没有足够的木头！", "退出");
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await DisplayAlert("提示", "请选择物品！", "好的");
            }
        }

        private void RawItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemDescription.Text = AllRawItems[RawItemsPicker.SelectedIndex].RawDescription;
        }
    }
}