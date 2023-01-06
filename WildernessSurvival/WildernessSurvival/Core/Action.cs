using WildernessSurvival.Game;
using WildernessSurvival.UI;
using Xamarin.Forms;

namespace WildernessSurvival.Core
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
        /// Move:
        /// Cost: Food[1]
        /// Go to next place
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
        /// Explore
        /// Cost: Food[1], Energy[1]
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
        /// Rest
        /// Cost: Food[1], Water[1]
        /// Restore: Health[2], Energy[4]
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
        /// Fire
        /// Cost: Log x1
        /// </summary>
        public void Fire()
        {
            if (IsDead) return;
            if (!HasWood) return;
            ConsumeWood(1);
            HasFire = true;
            DependencyService.Get<IToast>().ShortAlert("你生起了火。");
        }

        /// <summary>
        /// Hunt
        /// Prerequisites: Has any hunting tool
        /// </summary>
        public void Hunt()
        {
            if (IsDead) return;
            if (!HasHuntingTool) return;
            Modify(-1, AttrType.Food);
            Modify(-1, AttrType.Water);
            Modify(-3, AttrType.Energy);

            if (Injured(4))
            {
                DependencyService.Get<IToast>().ShortAlert("你的打猎的过程中遇上了野兽，经过一番搏斗后你逃脱了。");
            }
            else
            {
                var huntingTool = GetBestHuntingTool();
                var rate = 0;
                var doubleRate = 0;

                switch (huntingTool.Level)
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
                    AddItem(new RawRabbit());
                    if (Random.Next(100) < doubleRate)
                        AddItem(new RawRabbit());
                    DependencyService.Get<IToast>().ShortAlert("你满载而归，获得了大量的兔肉！");
                    return;
                }

                DependencyService.Get<IToast>().ShortAlert("眼前的猎物就这么溜走了，你感到很丧气。");
            }

            ++TurnCount;
        }

        /// <summary>
        /// Cut Down Tree
        /// Cost: Food[2], Energy[2]
        /// Gain: Log x1 + x1(50%)
        /// </summary>
        public void Cut()
        {
            if (IsDead) return;
            if (!HasOxe) return;
            Modify(-2, AttrType.Food);
            Modify(-2, AttrType.Energy);
            AddItem(LogItem.One);
            var count = 1;
            var rate = Random.Next(100);
            if (rate < 50)
            {
                AddItem(LogItem.One);
                ++count;
            }

            if (Location.HasLog)
            {
                AddItem(LogItem.One);
                ++count;
                if (Random.Next(100) < 50)
                {
                    AddItem(LogItem.One);
                    ++count;
                }
            }

            DependencyService.Get<IToast>().ShortAlert($"你获得了{count}根木头。");
            ++TurnCount;
        }

        /// <summary>
        /// Go Fishing
        /// Cost: Food[1], Water[1]
        /// Gain: Raw Fish x1(80%) + x1(20%)
        /// Prerequisites: Current location allows fishing.
        /// </summary>
        public void Fish()
        {
            if (IsDead) return;
            if (!CanFish || !Location.CanFish) return;
            Modify(-1, AttrType.Food);
            Modify(-1, AttrType.Water);
            var r = Random.Next(100);
            if (r < 80)
            {
                AddItem(new RawFish());
                if (Random.Next(100) < 20)
                    AddItem(new RawFish());
                DependencyService.Get<IToast>().ShortAlert("经过漫长地等待，你终于钓上了大鱼。");
                return;
            }

            DependencyService.Get<IToast>().ShortAlert("很可惜，你没有钓上鱼。");
            ++TurnCount;
        }
    }
}