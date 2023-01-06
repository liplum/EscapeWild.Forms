﻿using System;
using System.Collections.Generic;
using WildernessSurvival.Core;
using WildernessSurvival.UI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WildernessSurvival
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CookPage : ContentPage
    {
        private readonly Player _player;

        private readonly IList<IRawItem> _allRawItems;

        public CookPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["player"];
            _allRawItems = _player.RawItems;
            foreach (var item in _allRawItems)
                RawItemsPicker.Items.Add(item.LocalizedName());
        }

        private async void Heat_Clicked(object sender, EventArgs e)
        {
            var index = RawItemsPicker.SelectedIndex;
            if (index < 0) return;
            if (!_player.HasWood) return;
            var rawItem = _allRawItems[index];
            IItem cooked = rawItem.Cook();
            _player.RemoveItem(rawItem);
            _player.ConsumeWood(1);
            _player.AddItem(cooked);
            DependencyService.Get<IToast>().ShortAlert($"你获得了{cooked.LocalizedName()}");
            UpdateUI();
            await Navigation.PopModalAsync();
        }

        private void RawItemsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = _allRawItems[RawItemsPicker.SelectedIndex];
            ItemDescription.Text = selected.RawDescription;
            UpdateUI();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            Cook.IsEnabled = _player.HasWood;
        }
    }
}