using System;
using System.Collections.Generic;
using WildernessSurvival.Game.Items;
using WildernessSurvival.UI;
using Xamarin.Forms;

namespace WildernessSurvival.Game
{
    public partial class Player
    {
        private static ExploreBehaviour ExploreActions;

        private static readonly Random Random = new Random();

        private void RegisterExplore()
        {
            ExploreActions += ExplorePlain;
            ExploreActions += ExploreForest;
            ExploreActions += ExploreHut;
            ExploreActions += ExploreRiverFront;
        }

        private void DisplayAchievements(List<IItem> achievements)
        {
            if (achievements.Count != 0)
            {
                var result = "";
                foreach (var item in achievements) result += $" {item.LocalizedName()} ";
                DependencyService.Get<IToast>().ShortAlert($"你获得了：{result}。");
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert("你没有收获。");
            }
        }

        /// <summary>
        ///     探索平原
        ///     60%获得浆果(30%x2),60%获得脏水(30%x2)，20%获得木头
        /// </summary>
        private void ExplorePlain()
        {
            const int 浆果概率 = 60, 脏水概率 = 60, 木头概率 = 20, 双倍概率 = 30;
            if (LocationName == "平原")
            {
                var proportion = 10 - _curPositionExploreCount;
                proportion = proportion <= 0 ? 1 : proportion;
                var prop = proportion / 10f;

                var 获得的物品 = new List<IItem>();

                var 浆果 = Random.Next(100);
                if (浆果 < 浆果概率 * prop)
                {
                    获得的物品.Add(new 浆果());
                    if (Random.Next(100) < 双倍概率)
                        获得的物品.Add(new 浆果());
                }

                var 脏水 = Random.Next(100);
                if (脏水 < 脏水概率 * prop)
                {
                    获得的物品.Add(new 脏水());
                    if (Random.Next(100) < 双倍概率)
                        获得的物品.Add(new 脏水());
                }

                var 木头 = Random.Next(100);
                if (木头 < 木头概率) 获得的物品.Add(new 脏水());
                DisplayAchievements(获得的物品);
                AddItems(获得的物品);
            }
        }

        /// <summary>
        ///     探索森林
        ///     30%获得浆果,60%获得坚果(40%*2),20%获得脏水，50%获得木头(20%x2)
        /// </summary>
        private void ExploreForest()
        {
            const int 浆果概率 = 30, 脏水概率 = 20, 木头概率 = 50, 坚果概率 = 60, 坚果双倍概率 = 40, 木头双倍概率 = 20;
            if (LocationName == "森林")
            {
                var proportion = 10 - _curPositionExploreCount;
                proportion = proportion <= 0 ? 1 : proportion;
                var prop = proportion / 10f;

                var 获得的物品 = new List<IItem>();

                var 浆果 = Random.Next(100);
                if (浆果 < 浆果概率 * prop)
                    获得的物品.Add(new 浆果());

                var 脏水 = Random.Next(100);
                if (脏水 < 脏水概率 * prop)
                    获得的物品.Add(new 脏水());

                var 木头 = Random.Next(100);
                if (木头 < 木头概率 * prop)
                {
                    获得的物品.Add(LogItem.One);
                    if (Random.Next(100) < 木头双倍概率)
                        获得的物品.Add(LogItem.One);
                }

                var 坚果 = Random.Next(100);
                if (坚果 < 坚果概率 * prop)
                {
                    获得的物品.Add(new 坚果());
                    if (Random.Next(100) < 坚果双倍概率)
                        获得的物品.Add(new 坚果());
                }


                DisplayAchievements(获得的物品);
                AddItems(获得的物品);
            }
        }

        /// <summary>
        ///     探索河边
        ///     20%获得生鱼,70%获得净水(40%x2)，
        /// </summary>
        private void ExploreRiverFront()
        {
            const int 生鱼概率 = 20, 净水概率 = 70, 净水双倍概率 = 40;
            if (LocationName == "河边")
            {
                var proportion = 10 - _curPositionExploreCount;
                proportion = proportion <= 0 ? 1 : proportion;
                var prop = proportion / 10f;

                var 获得的物品 = new List<IItem>();

                var 生鱼 = Random.Next(100);
                if (生鱼 < 生鱼概率 * prop)
                    获得的物品.Add(new 生鱼());

                var 净水 = Random.Next(100);
                if (净水 < 净水概率 * prop)
                {
                    获得的物品.Add(new 净水());
                    if (Random.Next(100) < 净水双倍概率)
                        获得的物品.Add(new 净水());
                }

                DisplayAchievements(获得的物品);
                AddItems(获得的物品);
            }
        }

        /// <summary>
        ///     探索小屋
        ///     50%获得老旧的斧头，30%获得老旧的鱼竿,20%陷阱，5%获得猎枪
        ///     一定会奖励 瓶装水x2，能量棒x2，木头x2
        ///     猎枪和陷阱一局游戏只能获得一个
        ///     每个工具一局游戏中只能获得一个
        /// </summary>
        private void ExploreHut()
        {
            const int 斧子概率 = 50, 鱼竿概率 = 30, 陷阱概率 = 20, 猎枪概率 = 5;
            if (LocationName == "小屋")
            {
                var prop = _curPositionExploreCount == 0 ? 1 : 0;

                var 获得的物品 = new List<IItem>();

                if (!HasOxe)
                {
                    var 斧子 = Random.Next(100);
                    if (斧子 < 斧子概率) 获得的物品.Add(new 老旧的斧头());
                }

                if (!CanFish)
                {
                    var 鱼竿 = Random.Next(100);
                    if (鱼竿 < 鱼竿概率) 获得的物品.Add(new 老旧的钓鱼竿());
                }


                var 瓶装水 = Random.Next(100);
                if (瓶装水 < 100 * prop)
                {
                    获得的物品.Add(new 瓶装水());
                    获得的物品.Add(new 瓶装水());
                }

                var 能量棒 = Random.Next(100);
                if (能量棒 < 100 * prop)
                {
                    获得的物品.Add(new 能量棒());
                    获得的物品.Add(new 能量棒());
                }

                var 木头 = Random.Next(100);
                if (木头 < 100 * prop)
                {
                    获得的物品.Add(LogItem.One);
                    获得的物品.Add(LogItem.One);
                }

                if (!CanHunt)
                {
                    var s = Random.Next(2);
                    if (s == 0)
                    {
                        var 猎枪 = Random.Next(100);
                        if (猎枪 < 猎枪概率) 获得的物品.Add(new 老旧的猎枪());
                    }
                    else if (s == 1)
                    {
                        var 陷阱 = Random.Next(100);
                        if (陷阱 < 陷阱概率) 获得的物品.Add(new 捕兽陷阱());
                    }
                }

                DisplayAchievements(获得的物品);
                AddItems(获得的物品);
            }
        }

        private delegate void ExploreBehaviour();
    }
}