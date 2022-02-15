using System;
using System.Linq;
using WildernessSurvival.game.Items;
using WildernessSurvival.UI;
using Xamarin.Forms;
using static WildernessSurvival.game.Hunting;

namespace WildernessSurvival.game {
    public partial class Player {
        /// <summary>
        /// 意外受伤 10%
        /// </summary>
        private bool Injured(int Damge) {
            const int 受伤概率 = 10;
            var 失去的生命 = -Damge;
            var 受伤 = Random.Next(100);
            if (受伤 < 受伤概率) {
                Modify(失去的生命, ValueType.HP);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 行动：消耗 食物1，行动距离1
        /// </summary>
        public void Act() {
            if (!IsDead) {
                if (Injured(Damge: 2))
                    DependencyService.Get<IToast>().ShortAlert("你在路上遇到了野兽，你被抓伤了！");
                else
                    DependencyService.Get<IToast>().ShortAlert("你前进了一段路。");
                Modify(-1, ValueType.FOOD);
                Modify(-1, ValueType.ENERGY);
                AddTrip();
                SetPostion(CurRoute.NextPlace);
                HasFire = false;
                ++TurnCount;
            }
        }

        /// <summary>
        /// 探索：消耗 食物1和体力1
        /// </summary>
        public void Explore() {
            if (!IsDead) {
                if (Injured(Damge: 3))
                    DependencyService.Get<IToast>().ShortAlert("你在探索的时候遇到了野兽，你被咬伤了！");
                Modify(-1, ValueType.WATER);
                Modify(-1, ValueType.ENERGY);
                ExploreActions();
                ++CurPositionExploreCount;
                ++TurnCount;
            }
        }

        /// <summary>
        /// 休息：消耗 食物1，水分1，回复生命2，能量4
        /// </summary>
        public void Rest() {
            if (!IsDead) {
                Modify(-1, ValueType.FOOD);
                Modify(-1, ValueType.WATER);
                Modify(2, ValueType.HP);
                Modify(4, ValueType.ENERGY);
                DependencyService.Get<IToast>().ShortAlert("你休息了一会，感觉充满了力量！");
                ++TurnCount;
            }
        }

        /// <summary>
        /// 生火：消耗 木头x1
        /// </summary>
        public void Fire() {
            if (!IsDead) {
                if (HasWood) {
                    ConsumeWood(1);
                    HasFire = true;
                    DependencyService.Get<IToast>().ShortAlert("你生起了火。");
                } else {
                    DependencyService.Get<IToast>().ShortAlert("你没有木头，无法生火。");
                }
            }
        }

        /// <summary>
        /// 打猎：
        /// </summary>
        public void Hunt() {
            if (!IsDead) {
                if (CanHunt) {
                    Modify(-1, ValueType.FOOD);
                    Modify(-1, ValueType.WATER);
                    Modify(-3, ValueType.ENERGY);

                    if (Injured(Damge: 4)) {
                        DependencyService.Get<IToast>().ShortAlert("你的打猎的过程中遇上了野兽，经过一番搏斗后你逃脱了。");
                    } else {

                        var hunting = HuntingTools.First();
                        var level = ((Hunting)hunting).HuntingLevel;
                        var rate = 0;
                        var doubleRate = 0;

                        switch (level) {
                            case Level.LOW:
                                rate = 40;
                                doubleRate = 10;
                                break;
                            case Level.NOMAL:
                                rate = 55;
                                doubleRate = 20;
                                break;
                            case Level.HIGH:
                                rate = 70;
                                doubleRate = 30;
                                break;
                            case Level.MAX:
                                rate = 100;
                                doubleRate = 50;
                                break;
                            default:
                                break;
                        }
                        var r = Random.Next(100);
                        if (r < rate) {
                            AddItem(new 熟兔肉());
                            if (Random.Next(100) < doubleRate)
                                AddItem(new 熟兔肉());
                            DependencyService.Get<IToast>().ShortAlert("你满载而归，获得了大量的兔肉！");
                            return;
                        }
                        DependencyService.Get<IToast>().ShortAlert("眼前的猎物就这么溜走了，你感到很丧气。");
                    }
                    ++TurnCount;
                } else {
                    DependencyService.Get<IToast>().ShortAlert("你没狩猎的工具，不能打猎。");
                }
            }

        }

        /// <summary>
        /// 砍树：100% x1 ,消耗饱腹值2，能量值2
        /// 如果当前的位置木头多，能多获得一根,并50%x2
        /// </summary>
        public void Cut() {
            if (!IsDead) {
                if (HasOxe) {
                    Modify(-2, Player.ValueType.FOOD);
                    Modify(-2, Player.ValueType.ENERGY);
                    AddItem(new 木头());
                    var count = 1;
                    var rate = Random.Next(100);
                    if (rate < 50) {
                        AddItem(new 木头());
                        ++count;
                    }
                    if (Position.HasALotOfLog) {
                        AddItem(new 木头());
                        ++count;
                        if (Random.Next(100) < 50) {
                            AddItem(new 木头());
                            ++count;
                        }

                    }
                    DependencyService.Get<IToast>().ShortAlert($"你获得了{count}根木头。");
                    ++TurnCount;
                } else {
                    DependencyService.Get<IToast>().ShortAlert("你没斧子，不能砍树");
                }
            }
        }

        /// <summary>
        /// 钓鱼：消耗 饱腹值1和饮水值1
        /// 仅在可以钓鱼的地方钓鱼
        /// 80%x1,20%x2
        /// </summary>
        public void Fish() {
            if (!IsDead) {
                if (CanFish) {
                    if (Position.CanFish) {
                        Modify(-1, ValueType.FOOD);
                        Modify(-1, ValueType.WATER);
                        var r = Random.Next(100);
                        if (r < 80) {
                            AddItem(new 生鱼());
                            if (Random.Next(100) < 20)
                                AddItem(new 生鱼());
                            DependencyService.Get<IToast>().ShortAlert("经过漫长地等待，你终于钓上了大鱼。");
                            return;
                        }
                        DependencyService.Get<IToast>().ShortAlert("很可惜，你没有钓上鱼。");
                        ++TurnCount;
                    }
                }
                DependencyService.Get<IToast>().ShortAlert("这个地方不适合钓鱼。");
            }

        }
    }
}
