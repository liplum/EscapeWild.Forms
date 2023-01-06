using System.Linq;
using WildernessSurvival.game.Items;
using WildernessSurvival.UI;
using Xamarin.Forms;

namespace WildernessSurvival.game
{
    public partial class Player
    {
        /// <summary>
        ///     意外受伤 10%
        /// </summary>
        private bool Injured(int damage)
        {
            const int 受伤概率 = 10;
            var 失去的生命 = -damage;
            var 受伤 = Random.Next(100);
            if (受伤 < 受伤概率)
            {
                Modify(失去的生命, AttrType.Hp);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     行动：消耗 食物1，行动距离1
        /// </summary>
        public void Move()
        {
            if (IsDead) return;
            if (Injured(2))
                DependencyService.Get<IToast>().ShortAlert("你在路上遇到了野兽，你被抓伤了！");
            else
                DependencyService.Get<IToast>().ShortAlert("你前进了一段路。");
            Modify(-1, AttrType.Food);
            Modify(-1, AttrType.Energy);
            AddTrip();
            Location = _curRoute.NextPlace;
            HasFire = false;
            ++TurnCount;
        }

        /// <summary>
        ///     探索：消耗 食物1和体力1
        /// </summary>
        public void Explore()
        {
            if (IsDead) return;
            if (Injured(3))
                DependencyService.Get<IToast>().ShortAlert("你在探索的时候遇到了野兽，你被咬伤了！");
            Modify(-1, AttrType.Water);
            Modify(-1, AttrType.Energy);
            ExploreActions();
            ++_curPositionExploreCount;
            ++TurnCount;
        }

        /// <summary>
        ///     休息：消耗 食物1，水分1，回复生命2，能量4
        /// </summary>
        public void Rest()
        {
            if (IsDead) return;
            Modify(-1, AttrType.Food);
            Modify(-1, AttrType.Water);
            Modify(2, AttrType.Hp);
            Modify(4, AttrType.Energy);
            DependencyService.Get<IToast>().ShortAlert("你休息了一会，感觉充满了力量！");
            ++TurnCount;
        }

        /// <summary>
        ///     生火：消耗 木头x1
        /// </summary>
        public void Fire()
        {
            if (IsDead) return;
            if (HasWood)
            {
                ConsumeWood(1);
                HasFire = true;
                DependencyService.Get<IToast>().ShortAlert("你生起了火。");
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert("你没有木头，无法生火。");
            }
        }

        /// <summary>
        ///     打猎：
        /// </summary>
        public void Hunt()
        {
            if (IsDead) return;
            if (CanHunt)
            {
                Modify(-1, AttrType.Food);
                Modify(-1, AttrType.Water);
                Modify(-3, AttrType.Energy);

                if (Injured(4))
                {
                    DependencyService.Get<IToast>().ShortAlert("你的打猎的过程中遇上了野兽，经过一番搏斗后你逃脱了。");
                }
                else
                {
                    var hunting = HuntingTools.First();
                    var level = ((IHuntingToolItem)hunting).HuntingToolLevel;
                    var rate = 0;
                    var doubleRate = 0;

                    switch (level)
                    {
                        case ToolLevel.Low:
                            rate = 40;
                            doubleRate = 10;
                            break;
                        case ToolLevel.Normal:
                            rate = 55;
                            doubleRate = 20;
                            break;
                        case ToolLevel.High:
                            rate = 70;
                            doubleRate = 30;
                            break;
                        case ToolLevel.Max:
                            rate = 100;
                            doubleRate = 50;
                            break;
                    }

                    var r = Random.Next(100);
                    if (r < rate)
                    {
                        AddItem(new 熟兔肉());
                        if (Random.Next(100) < doubleRate)
                            AddItem(new 熟兔肉());
                        DependencyService.Get<IToast>().ShortAlert("你满载而归，获得了大量的兔肉！");
                        return;
                    }

                    DependencyService.Get<IToast>().ShortAlert("眼前的猎物就这么溜走了，你感到很丧气。");
                }

                ++TurnCount;
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert("你没狩猎的工具，不能打猎。");
            }
        }

        /// <summary>
        ///     砍树：100% x1 ,消耗饱腹值2，能量值2
        ///     如果当前的位置木头多，能多获得一根,并50%x2
        /// </summary>
        public void Cut()
        {
            if (IsDead) return;
            if (HasOxe)
            {
                Modify(-2, AttrType.Food);
                Modify(-2, AttrType.Energy);
                AddItem(new 木头());
                var count = 1;
                var rate = Random.Next(100);
                if (rate < 50)
                {
                    AddItem(new 木头());
                    ++count;
                }

                if (_location.HasLog)
                {
                    AddItem(new 木头());
                    ++count;
                    if (Random.Next(100) < 50)
                    {
                        AddItem(new 木头());
                        ++count;
                    }
                }

                DependencyService.Get<IToast>().ShortAlert($"你获得了{count}根木头。");
                ++TurnCount;
            }
            else
            {
                DependencyService.Get<IToast>().ShortAlert("你没斧子，不能砍树");
            }
        }

        /// <summary>
        ///     钓鱼：消耗 饱腹值1和饮水值1
        ///     仅在可以钓鱼的地方钓鱼
        ///     80%x1,20%x2
        /// </summary>
        public void Fish()
        {
            if (IsDead) return;
            if (CanFish)
                if (_location.CanFish)
                {
                    Modify(-1, AttrType.Food);
                    Modify(-1, AttrType.Water);
                    var r = Random.Next(100);
                    if (r < 80)
                    {
                        AddItem(new 生鱼());
                        if (Random.Next(100) < 20)
                            AddItem(new 生鱼());
                        DependencyService.Get<IToast>().ShortAlert("经过漫长地等待，你终于钓上了大鱼。");
                        return;
                    }

                    DependencyService.Get<IToast>().ShortAlert("很可惜，你没有钓上鱼。");
                    ++TurnCount;
                }

            DependencyService.Get<IToast>().ShortAlert("这个地方不适合钓鱼。");
        }
    }
}