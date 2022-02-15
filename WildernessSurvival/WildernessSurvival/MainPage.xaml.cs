using System;
using System.ComponentModel;
using WildernessSurvival;
using WildernessSurvival.game;
using Xamarin.Forms;

namespace WildernessSurvival_Xamarin {
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
        }

        private static readonly Player player = (Player)Application.Current.Resources["player"];

        /// <summary>
        /// 行动
        /// </summary>
        private void Act_Cliked(object sender, EventArgs e) {
            player.Act();
            Trip.ProgressTo(player.Trip, 300, Easing.Linear);
            CheckDeadOrWin();
        }

        /// <summary>
        /// 探索
        /// </summary>
        private void Explore_Cliked(object sender, EventArgs e) {
            player.Explore();
            CheckDeadOrWin();
        }

        /// <summary>
        /// 休息
        /// </summary>
        private void Rest_Cliked(object sender, EventArgs e) {
            player.Rest();
            CheckDeadOrWin();
        }

        private async void CheckDeadOrWin() {
            if (player.IsDead) {
                var answer = await DisplayAlert("失败", $"你死了！共坚持了{player.TurnCount}个回合。", "重新开始", "返回");
                if (answer) {
                    RestartGame();
                } else {
                    Restart.IsVisible = true;
                }
            } else if (player.IsWin) {
                var answer = await DisplayAlert("恭喜", $"经历了{player.TurnCount}个回合,你成功逃了出来！", "新的一次", "再欣赏一下");
                if (answer) {
                    RestartGame();
                } else {
                    Restart.IsVisible = true;
                }
            }
        }

        private void RestartGame() {
            RestartGame(null, null);
        }

        private void Hunt_Clicked(object sender, EventArgs e) {
            player.Hunt();
            CheckDeadOrWin();
        }

        private void Cut_Clicked(object sender, EventArgs e) {
            player.Cut();
            CheckDeadOrWin();
        }

        private void Fish_Clicked(object sender, EventArgs e) {
            player.Fish();
            CheckDeadOrWin();
        }

        private void Cook_Clicked(object sender, EventArgs e) {
            Navigation.PushModalAsync(new CookPage(), true);
        }

        private void RestartGame(object sender, EventArgs e) {
            player.Reset();
            Restart.IsVisible = false;
            Trip.ProgressTo(player.Trip, 300, Easing.Linear);
        }

        private void Backpack_Clicked(object sender, EventArgs e) {
            Navigation.PushModalAsync(new BackpackPage(), true);
        }

        private void Fire_Clicked(object sender, EventArgs e) {
            player.Fire();
        }
    }
}
