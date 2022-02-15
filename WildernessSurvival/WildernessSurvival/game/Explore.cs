using System;
using System.Collections.Generic;
using WildernessSurvival.game.Items;
using WildernessSurvival.UI;
using Xamarin.Forms;

namespace WildernessSurvival.game
{
    public partial class Player
    {
        private delegate void ExploreBehaviour();

        private static ExploreBehaviour ExploreActions;

        private readonly static Random Random = new Random();

        private void RegisterExplore()
        {
            ExploreActions += ExplorePlain;
            ExploreActions += ExploreForest;
            ExploreActions += ExploreHut;
            ExploreActions += ExploreRiverFront;
        }

        private void DisplayAchievements(List<ItemBase> Achievements)
        {
            if (Achievements.Count != 0)
            {
                string result = "";
                foreach (ItemBase item in Achievements)
                {
                    result += $" {item} ";
                }
                DependencyService.Get<IToast>().ShortAlert($"你获得了：{result}。");
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert("你没有收获。");
            }
        }

        /// <summary>
        /// 探索平原
        /// 60%获得浆果(30%x2),60%获得脏水(30%x2)，20%获得木头
        /// </summary>
        private void ExplorePlain()
        {
            const int 浆果概率 = 60, 脏水概率 = 60, 木头概率 = 20, 双倍概率 = 30;
            if (POS == "平原")
            {
                int proportion = 10 - CurPositionExploreCount;
                proportion = proportion <= 0 ? 1 : proportion;
                float prop = proportion / 10f;

                List<ItemBase> 获得的物品 = new List<ItemBase>();

                int 浆果 = Random.Next(100);
                if (浆果 < 浆果概率 * prop)
                {
                    获得的物品.Add(new 浆果());
                    if (Random.Next(100) < 双倍概率)
                        获得的物品.Add(new 浆果());
                }
                int 脏水 = Random.Next(100);
                if (脏水 < 脏水概率 * prop)
                {
                    获得的物品.Add(new 脏水());
                    if (Random.Next(100) < 双倍概率)
                        获得的物品.Add(new 脏水());
                }
                int 木头 = Random.Next(100);
                if (木头 < 木头概率)
                {
                    获得的物品.Add(new 脏水());
                }
                DisplayAchievements(获得的物品);
                AddItems(获得的物品);
            }
        }

        /// <summary>
        /// 探索森林
        /// 30%获得浆果,60%获得坚果(40%*2),20%获得脏水，50%获得木头(20%x2)
        /// </summary>
        private void ExploreForest()
        {
            const int 浆果概率 = 30, 脏水概率 = 20, 木头概率 = 50, 坚果概率 = 60, 坚果双倍概率 = 40, 木头双倍概率 = 20;
            if (POS == "森林")
            {
                int proportion = 10 - CurPositionExploreCount;
                proportion = proportion <= 0 ? 1 : proportion;
                float prop = proportion / 10f;

                List<ItemBase> 获得的物品 = new List<ItemBase>();

                int 浆果 = Random.Next(100);
                if (浆果 < 浆果概率 * prop)
                    获得的物品.Add(new 浆果());

                int 脏水 = Random.Next(100);
                if (脏水 < 脏水概率 * prop)
                    获得的物品.Add(new 脏水());

                int 木头 = Random.Next(100);
                if (木头 < 木头概率 * prop)
                {
                    获得的物品.Add(new 木头());
                    if (Random.Next(100) < 木头双倍概率)
                        获得的物品.Add(new 木头());
                }

                int 坚果 = Random.Next(100);
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
        /// 探索河边
        /// 20%获得生鱼,70%获得净水(40%x2)，
        /// </summary>
        private void ExploreRiverFront()
        {
            const int 生鱼概率 = 20, 净水概率 = 70, 净水双倍概率 = 40;
            if (POS == "河边")
            {
                int proportion = 10 - CurPositionExploreCount;
                proportion = proportion <= 0 ? 1 : proportion;
                float prop = proportion / 10f;

                List<ItemBase> 获得的物品 = new List<ItemBase>();

                int 生鱼 = Random.Next(100);
                if (生鱼 < 生鱼概率 * prop)
                    获得的物品.Add(new 生鱼());

                int 净水 = Random.Next(100);
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
        /// 探索小屋
        /// 50%获得老旧的斧头，30%获得老旧的鱼竿,20%陷阱，5%获得猎枪
        /// 一定会奖励 瓶装水x2，能量棒x2，木头x2
        /// 猎枪和陷阱一局游戏只能获得一个
        /// 每个工具一局游戏中只能获得一个
        /// </summary>
        private void ExploreHut()
        {
            const int 斧子概率 = 50, 鱼竿概率 = 30,陷阱概率 = 20, 猎枪概率 = 5;
            if (POS == "小屋")
            {
                int prop = CurPositionExploreCount == 0 ? 1 : 0;

                List<ItemBase> 获得的物品 = new List<ItemBase>();

                if (!HasOxe)
                {
                    int 斧子 = Random.Next(100);
                    if (斧子 < 斧子概率)
                    {
                        获得的物品.Add(new 老旧的斧头());
                    }
                }

                if (!CanFish)
                {
                    int 鱼竿 = Random.Next(100);
                    if (鱼竿 < 鱼竿概率)
                    {
                        获得的物品.Add(new 老旧的钓鱼竿());
                    }
                }


                int 瓶装水 = Random.Next(100);
                if (瓶装水 < 100 * prop)
                {
                    获得的物品.Add(new 瓶装水());
                    获得的物品.Add(new 瓶装水());
                }

                int 能量棒 = Random.Next(100);
                if (能量棒 < 100 * prop)
                {
                    获得的物品.Add(new 能量棒());
                    获得的物品.Add(new 能量棒());
                }

                int 木头 = Random.Next(100);
                if (木头 < 100 * prop)
                {
                    获得的物品.Add(new 木头());
                    获得的物品.Add(new 木头());
                }

                if (!CanHunt)
                {
                    int s = Random.Next(2);
                    if (s == 0)
                    {
                        int 猎枪 = Random.Next(100);
                        if (猎枪 < 猎枪概率)
                        {
                            获得的物品.Add(new 老旧的猎枪());
                        }
                    }
                    else if(s == 1)
                    {
                        int 陷阱 = Random.Next(100);
                        if (陷阱 < 陷阱概率)
                        {
                            获得的物品.Add(new 捕兽陷阱());
                        }
                    }
                }
                DisplayAchievements(获得的物品);
                AddItems(获得的物品);
            }
        }
    }
}
