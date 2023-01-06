using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WildernessSurvival.Core;
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
            if (_player.IsDead) return;
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
            Backpack.IsEnabled &= _player.IsAlive;
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
                var answer = await DisplayAlert("失败", $"你死了！共坚持了{_player.TurnCount}个回合。", "重新开始", "返回");
                if (answer)
                {
                    RestartGame();
                }
                else
                {
                    Restart.IsVisible = true;
                }
            }
            else if (_player.IsWon)
            {
                var answer = await DisplayAlert("恭喜", $"经历了{_player.TurnCount}个回合,你成功逃了出来！", "新的一次", "再欣赏一下");
                if (answer)
                {
                    RestartGame();
                }
                else
                {
                    Restart.IsVisible = true;
                }
            }
        }


        private void RestartGame()
        {
            _player.Reset();
            Restart.IsVisible = false;
            Trip.ProgressTo(_player.TripRatio, 300, Easing.Linear);
        }

        private void Restart_OnClicked(object sender, EventArgs e)
        {
            RestartGame();
        }
    }
}