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

        private void Move_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Move();
            Trip.ProgressTo(_player.TripRatio, 300, Easing.Linear);
            UpdateUI();
        }

        private void Explore_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Explore();
            UpdateUI();
        }

        private void Rest_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Rest();
            UpdateUI();
        }

        private void Hunt_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Hunt();
            UpdateUI();
        }

        private void Cut_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Cut();
            UpdateUI();
        }

        private void Fish_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Fish();
            UpdateUI();
        }

        private void Fire_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            _player.Fire();
            UpdateUI();
        }

        private void Cook_Clicked(object sender, EventArgs e)
        {
            if (_player.IsDead) return;
            Navigation.PushModalAsync(new CookPage(), true);
        }

        private void Backpack_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new BackpackPage(), true);
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
            var answer = await DisplayAlert(i("Title"),
                string.Format(i("Content"), _player.TurnCount),
                i("Accept"),
                i("Cancel")
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