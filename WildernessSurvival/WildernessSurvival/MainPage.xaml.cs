using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;
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
            _player = (Player)Application.Current.Resources["player"];
            UpdateUI();
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

        private async void Cut_Clicked(object sender, EventArgs e)
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
            await _player.PerformAction(ActionType.Fire);
            UpdateUI();
        }

        private async void Cook_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            await Navigation.PushModalAsync(new CookPage(), true);
        }

        private async void Backpack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new BackpackPage(), true);
        }


        // ReSharper disable once InconsistentNaming
        private async void UpdateUI()
        {
            Fire.IsEnabled = _player.HasWood && !_player.HasFire;
            Cut.IsEnabled = _player.HasOxe;
            Hunt.IsEnabled = _player.HasHuntingTool;
            Fish.IsEnabled = _player.CanFish & _player.Location.CanFish;
            Cook.IsEnabled = _player.HasFire;
            await CheckDeadOrWin();
            Move.IsEnabled &= _player.CanPerformAnyAction;
            Hunt.IsEnabled &= _player.CanPerformAnyAction;
            Cut.IsEnabled &= _player.CanPerformAnyAction;
            Fish.IsEnabled &= _player.CanPerformAnyAction;
            Explore.IsEnabled &= _player.CanPerformAnyAction;
            Cut.IsEnabled &= _player.CanPerformAnyAction;
            Rest.IsEnabled &= _player.CanPerformAnyAction;
            Fire.IsEnabled &= _player.CanPerformAnyAction;
            Cook.IsEnabled &= _player.CanPerformAnyAction;
            await Trip.ProgressTo(_player.TripRatio, 300, Easing.Linear);
        }

        private async Task CheckDeadOrWin()
        {
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
            var answer = await DisplayAlert(
                title: i("Title"),
                message: string.Format(i("Content"), _player.TurnCount),
                accept: i("Accept"),
                cancel: i("Cancel")
            );
            if (answer)
            {
                RestartGame();
            }
            else
            {
                Restart.IsVisible = true;
            }
        }

        private void RestartGame()
        {
            _player.Reset();
            Restart.IsVisible = false;
            Trip.ProgressTo(_player.TripRatio, 300, Easing.Linear);
            Move.IsEnabled = true;
            Explore.IsEnabled = true;
            Rest.IsEnabled = true;
            UpdateUI();
        }

        private void Restart_OnClicked(object sender, EventArgs e)
        {
            RestartGame();
        }
    }
}