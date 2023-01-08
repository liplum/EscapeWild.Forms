using System;
using System.ComponentModel;
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
        private static Player Player => App.Player;

        public MainPage()
        {
            InitializeComponent();
            UpdateUI(updateProgressBarInSequence: true);
        }

        private async void Move_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await Player.PerformAction(ActionType.Move);
            UpdateUI();
        }

        private async void Explore_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await Player.PerformAction(ActionType.Explore);
            UpdateUI();
        }

        private async void Rest_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await Player.PerformAction(ActionType.Rest);
            UpdateUI();
        }

        private async void Hunt_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await Player.PerformAction(ActionType.Hunt);
            UpdateUI();
        }

        private async void CutDownTree_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await Player.PerformAction(ActionType.CutDownTree);
            UpdateUI();
        }

        private async void Fish_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await Player.PerformAction(ActionType.Fish);
            UpdateUI();
        }

        private async void Fire_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await this.ShowModalSheetAndAwaitPop(new FirePage());
            UpdateUI();
        }

        private async void Cook_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
            await this.ShowModalSheetAndAwaitPop(new CookPage());
            UpdateUI();
        }

        private async void Craft_Clicked(object sender, EventArgs e)
        {
            if (Player.IsDead) return;
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
            RouteLabel.Text = Player.CurRoute.LocalizedName();
            LocationName.Text = Player.Location.LocalizedName();
            // Initialized by location actions
            var actions = Player.Location.AvailableActions;
            Move.IsEnabled = actions.Contains(ActionType.Move);
            Explore.IsEnabled = actions.Contains(ActionType.Explore);
            Rest.IsEnabled = actions.Contains(ActionType.Rest);
            Fire.IsEnabled = actions.Contains(ActionType.Fire);
            Hunt.IsEnabled = actions.Contains(ActionType.Hunt);
            CutDownTree.IsEnabled = actions.Contains(ActionType.CutDownTree);
            Fish.IsEnabled = actions.Contains(ActionType.Fish);
            // Initialized by player states
            Cook.IsEnabled = Player.HasFire;
            Craft.IsEnabled = true;
            // Modified by player states
            Fire.IsEnabled &= Player.HasFire;
            CutDownTree.IsEnabled &= Player.HasToolOf(ToolType.Oxe);
            Hunt.IsEnabled &= Player.HasToolOf(ToolType.Hunting);
            Fish.IsEnabled &= Player.HasToolOf(ToolType.Fishing);
            // Modified by win or failure
            Move.IsEnabled &= Player.CanPerformAnyAction;
            Hunt.IsEnabled &= Player.CanPerformAnyAction;
            CutDownTree.IsEnabled &= Player.CanPerformAnyAction;
            Fish.IsEnabled &= Player.CanPerformAnyAction;
            Explore.IsEnabled &= Player.CanPerformAnyAction;
            Rest.IsEnabled &= Player.CanPerformAnyAction;
            Fire.IsEnabled &= Player.CanPerformAnyAction;
            Cook.IsEnabled &= Player.CanPerformAnyAction;
            Craft.IsEnabled &= Player.CanPerformAnyAction;
            // Modified by energy
            Move.IsEnabled &= Player.HasEnergy;
            Hunt.IsEnabled &= Player.HasEnergy;
            CutDownTree.IsEnabled &= Player.HasEnergy;
            Fish.IsEnabled &= Player.HasEnergy;
            Explore.IsEnabled &= Player.HasEnergy;
            // Update Restart button
            if (updateProgressBarInSequence)
            {
                await TripProgressBar.ProgressTo(Player.JourneyProgress, 300, Easing.Linear);
                await HealthProgressBar.ProgressTo(Player.Health, 300, Easing.Linear);
                await FoodProgressBar.ProgressTo(Player.Food, 300, Easing.Linear);
                await WaterProgressBar.ProgressTo(Player.Water, 300, Easing.Linear);
                await EnergyProgressBar.ProgressTo(Player.Energy, 300, Easing.Linear);
            }
            else
            {
                TripProgressBar.ProgressTo(Player.JourneyProgress, 300, Easing.Linear);
                HealthProgressBar.ProgressTo(Player.Health, 300, Easing.Linear);
                FoodProgressBar.ProgressTo(Player.Food, 300, Easing.Linear);
                WaterProgressBar.ProgressTo(Player.Water, 300, Easing.Linear);
                EnergyProgressBar.ProgressTo(Player.Energy, 300, Easing.Linear);
            }

            await CheckDeadOrWin();
        }

        private async Task CheckDeadOrWin()
        {
            // Don't show the tip again if player can hit the restart button.
            if (Restart.IsVisible) return;
            Restart.IsVisible = !Player.CanPerformAnyAction;
            if (Player.IsDead)
            {
                await ShowDialog("Failed");
            }
            else if (Player.IsWon)
            {
                await ShowDialog("Win");
            }
        }

        private async Task ShowDialog(string state)
        {
            string i(string key) => I18N.Get($"Dialog.{state}.{key}");
            if (await DisplayAlert(
                    title: i("Title"),
                    message: string.Format(i("Content"), Player.ActionNumber),
                    accept: i("Accept"),
                    cancel: i("Cancel")
                ))
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {
            Player.Reset();
            Restart.IsVisible = false;
            UpdateUI();
        }

        private void Restart_OnClicked(object sender, EventArgs e)
        {
            RestartGame();
        }
    }
}