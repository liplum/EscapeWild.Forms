using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;
using WildernessSurvival.UI;
using Xamarin.Forms;

namespace WildernessSurvival
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private static Player _player;

        public MainPage()
        {
            InitializeComponent();
            _player = (Player)Application.Current.Resources["Player"];
            UpdateUI(updateProgressBarInSequence: true);
        }

        private async void Move_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await _player.PerformAction(ActionType.Move);
            UpdateUI();
        }

        private async void Explore_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await _player.PerformAction(ActionType.Explore);
            UpdateUI();
        }

        private async void Rest_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await _player.PerformAction(ActionType.Rest);
            UpdateUI();
        }

        private async void Hunt_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await _player.PerformAction(ActionType.Hunt);
            UpdateUI();
        }

        private async void CutDownTree_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await _player.PerformAction(ActionType.CutDownTree);
            UpdateUI();
        }

        private async void Fish_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await _player.PerformAction(ActionType.Fish);
            UpdateUI();
        }

        private async void Fire_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await this.ShowModalSheetAndAwaitPop(new FirePage());
            UpdateUI();
        }

        private async void Cook_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await this.ShowModalSheetAndAwaitPop(new CookPage());
            UpdateUI();
        }

        private async void Craft_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await this.ShowModalSheetAndAwaitPop(new CraftPage());
            UpdateUI();
        }

        private async void Backpack_Clicked(object sender, EventArgs e)
        {
            await this.ShowModalSheetAndAwaitPop(new BackpackPage());
            UpdateUI();
        }


        // ReSharper disable once InconsistentNaming
        private async void UpdateUI(bool updateProgressBarInSequence = false)
        {
            RouteLabel.Text = _player.CurRoute.LocalizedName();
            // Initialized by location actions
            var actions = _player.Location.AvailableActions;
            Move.IsEnabled = actions.Contains(ActionType.Move);
            Explore.IsEnabled = actions.Contains(ActionType.Explore);
            Rest.IsEnabled = actions.Contains(ActionType.Rest);
            Fire.IsEnabled = actions.Contains(ActionType.Fire);
            Hunt.IsEnabled = actions.Contains(ActionType.Hunt);
            CutDownTree.IsEnabled = actions.Contains(ActionType.CutDownTree);
            Fish.IsEnabled = actions.Contains(ActionType.Fish);
            // Initialized by player states
            Cook.IsEnabled = _player.HasFire;
            Craft.IsEnabled = true;
            // Modified by player states
            Fire.IsEnabled &= _player.HasFire;
            CutDownTree.IsEnabled &= _player.HasToolOf(ToolType.Oxe);
            Hunt.IsEnabled &= _player.HasToolOf(ToolType.Hunting);
            Fish.IsEnabled &= _player.HasToolOf(ToolType.Fishing);
            // Modified by win or failure
            Move.IsEnabled &= _player.CanPerformAnyAction;
            Hunt.IsEnabled &= _player.CanPerformAnyAction;
            CutDownTree.IsEnabled &= _player.CanPerformAnyAction;
            Fish.IsEnabled &= _player.CanPerformAnyAction;
            Explore.IsEnabled &= _player.CanPerformAnyAction;
            Rest.IsEnabled &= _player.CanPerformAnyAction;
            Fire.IsEnabled &= _player.CanPerformAnyAction;
            Cook.IsEnabled &= _player.CanPerformAnyAction;
            Craft.IsEnabled &= _player.CanPerformAnyAction;
            // Update Restart button
            if (updateProgressBarInSequence)
            {
                await TripProgressBar.ProgressTo(_player.TripProgress, 300, Easing.Linear);
                await HealthProgressBar.ProgressTo(_player.Health, 300, Easing.Linear);
                await FoodProgressBar.ProgressTo(_player.Food, 300, Easing.Linear);
                await WaterProgressBar.ProgressTo(_player.Water, 300, Easing.Linear);
                await EnergyProgressBar.ProgressTo(_player.Energy, 300, Easing.Linear);
            }
            else
            {
                TripProgressBar.ProgressTo(_player.TripProgress, 300, Easing.Linear);
                HealthProgressBar.ProgressTo(_player.Health, 300, Easing.Linear);
                FoodProgressBar.ProgressTo(_player.Food, 300, Easing.Linear);
                WaterProgressBar.ProgressTo(_player.Water, 300, Easing.Linear);
                EnergyProgressBar.ProgressTo(_player.Energy, 300, Easing.Linear);
            }

            await CheckDeadOrWin();
        }

        private async Task CheckDeadOrWin()
        {
            // Don't show the tip again if player can hit the restart button.
            if (Restart.IsVisible) return;
            Restart.IsVisible = !_player.CanPerformAnyAction;
            if (_player.IsDead)
            {
                await ShowDialog("Failed");
            }
            else if (_player.IsWon)
            {
                await ShowDialog("Win");
            }
        }

        private async Task ShowDialog(string state)
        {
            string i(string key) => I18N.Get($"Dialog.{state}.{key}");
            if (await DisplayAlert(
                    title: i("Title"),
                    message: string.Format(i("Content"), _player.ActionNumber),
                    accept: i("Accept"),
                    cancel: i("Cancel")
                ))
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {
            _player.Reset();
            Restart.IsVisible = false;
            UpdateUI();
        }

        private void Restart_OnClicked(object sender, EventArgs e)
        {
            RestartGame();
        }
    }
}