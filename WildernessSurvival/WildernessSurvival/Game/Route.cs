using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;

namespace WildernessSurvival.Game
{
    public class Route : IRoute<Place>
    {
        public string Name { get; }
        private const int ChangedRate = 30;
        private static readonly Random Random = new Random();
        private readonly List<Place> _allPlace;

        public Route(string name, params Place[] places)
        {
            Name = name;
            _allPlace = places.ToList();
            foreach (var place in places)
            {
                place.Route = this;
            }

            _curPlace = _allPlace[0];
        }

        public Place InitialPlace => _allPlace[0];

        private Place _curPlace;

        public async Task<Place> GoNextPlace(Player player)
        {
            var needChange = Rand.Int(100);
            if (_curPlace.IsSpecial || needChange < ChangedRate)
            {
                var cur = _curPlace;
                var range = 100 - _curPlace.AppearRate;
                var next = Rand.Int(range);
                var OtherPlace = (from p in _allPlace where p != cur select p).ToList();

                for (int i = 0, sum = 0; i <= OtherPlace.Count; ++i)
                {
                    var p = OtherPlace[i];
                    sum += p.AppearRate;
                    if (next <= sum)
                    {
                        await _curPlace.OnLeave(player);
                        _curPlace = p;
                        await p.OnEnter(player);
                        return p;
                    }
                }
            }

            return _curPlace;
        }
    }

    public abstract class Place : IPlace
    {
        public abstract string Name { get; }

        public abstract bool HasLog { get; }
        public abstract bool CanFish { get; }

        public abstract int AppearRate { get; }

        public abstract int HuntingRate { get; }

        public abstract bool IsSpecial { get; }

        public IRoute<IPlace> Route { get; set; }
        private Route Owner => Route as Route;
        protected int ExploreCount;

        public virtual async Task OnLeave(Player player)
        {
            ExploreCount = 0;
        }


        public virtual async Task OnEnter(Player player)
        {
            ExploreCount = 0;
        }

        public async Task PerformAction(Player player, ActionType action)
        {
            if (action == ActionType.Move)
            {
                await PerformMove(player);
            }
            else if (action == ActionType.Explore)
            {
                await PerformExplore(player);
            }
            else if (action == ActionType.Rest)
            {
                await PerformRest(player);
            }
            else if (action == ActionType.Fire)
            {
                await PerformFire(player);
            }
            else if (action == ActionType.Hunt)
            {
                await PerformHunt(player);
            }
            else if (action == ActionType.CutDownTree)
            {
                await PerformCutDownTree(player);
            }
            else if (action == ActionType.Fish)
            {
                await PerformFish(player);
            }
        }

        /// <summary>
        /// Move:
        /// Cost: Food[1]
        /// Go to next place
        /// </summary>
        protected virtual async Task PerformMove(Player player)
        {
            player.Modify(-1, AttrType.Food);
            player.Modify(-1, AttrType.Energy);
            player.AdvanceTrip();
            player.Location = await Owner.GoNextPlace(player);
            player.HasFire = false;
        }

        /// <summary>
        /// Rest
        /// Cost: Food[1], Water[1]
        /// Restore: Health[2], Energy[4]
        /// </summary>
        protected virtual async Task PerformRest(Player player)
        {
            player.Modify(-1, AttrType.Food);
            player.Modify(-1, AttrType.Water);
            player.Modify(2, AttrType.Hp);
            player.Modify(4, AttrType.Energy);

            await App.Current.MainPage.DisplayAlert(
                title: ActionType.Rest.LocalizedName(),
                message: $"{Route.Name}.Common.Rest".Tr(),
                cancel: "OK".Tr()
            );
        }

        protected virtual async Task PerformFish(Player player)
        {
        }

        protected virtual async Task PerformHunt(Player player)
        {
        }

        protected virtual async Task PerformCutDownTree(Player player)
        {
        }

        protected abstract Task PerformExplore(Player player);

        /// <summary>
        /// Fire
        /// Cost: Log x1
        /// </summary>
        protected virtual async Task PerformFire(Player player)
        {
            if (!player.HasWood) return;
            player.ConsumeWood(1);
            player.HasFire = true;
            await App.Current.MainPage.DisplayAlert(
                title: ActionType.Fire.LocalizedName(),
                message: $"{Route.Name}.Common.Fire".Tr(),
                cancel: "OK".Tr()
            );
        }

        public ISet<ActionType> AvailableActions
        {
            get
            {
                var actions = new HashSet<ActionType>
                {
                    ActionType.Move,
                    ActionType.Explore,
                    ActionType.Rest,
                    ActionType.Fire,
                    ActionType.Hunt,
                };
                if (HasLog) actions.Add(ActionType.CutDownTree);
                if (CanFish) actions.Add(ActionType.Fish);
                return actions;
            }
        }
    }

    public class PlainPlace : Place
    {
        public PlainPlace(string name, int appearRate, int huntingRate)
        {
            Name = name;
            AppearRate = appearRate;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override bool HasLog => false;
        public override bool CanFish => false;
        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }


        /// <summary>
        /// Berry x1(60%) + x1(30%)
        /// Dirty Water x1(60%) + x1(30%)
        /// Log x1(20%)
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-1, AttrType.Water);
            player.Modify(-1, AttrType.Energy);
            const int 浆果概率 = 60, 脏水概率 = 60, 木头概率 = 20, 双倍概率 = 30;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var 获得的物品 = new List<IItem>();

            var 浆果 = Rand.Int(100);
            if (浆果 < 浆果概率 * prop)
            {
                获得的物品.Add(new Berry());
                if (Rand.Int(100) < 双倍概率)
                    获得的物品.Add(new Berry());
            }

            var 脏水 = Rand.Int(100);
            if (脏水 < 脏水概率 * prop)
            {
                获得的物品.Add(new DirtyWater());
                if (Rand.Int(100) < 双倍概率)
                    获得的物品.Add(new DirtyWater());
            }

            var 木头 = Rand.Int(100);
            if (木头 < 木头概率) 获得的物品.Add(new DirtyWater());
            player.AddItems(获得的物品);
            ExploreCount++;
            await player.DisplayAchievements(获得的物品);
        }
    }

    public class RiversidePlace : Place
    {
        public RiversidePlace(string name, int appearRate, int huntingRate)
        {
            Name = name;
            AppearRate = appearRate;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override bool HasLog => false;
        public override bool CanFish => true;
        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }

        /// <summary>
        /// Raw Fish x1(20%)
        /// Clean Water x1(70%) + x1(40%) 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-1, AttrType.Water);
            player.Modify(-1, AttrType.Energy);
            const int 生鱼概率 = 20, 净水概率 = 70, 净水双倍概率 = 40;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var 获得的物品 = new List<IItem>();

            var 生鱼 = Rand.Int(100);
            if (生鱼 < 生鱼概率 * prop)
                获得的物品.Add(new RawFish());

            var 净水 = Rand.Int(100);
            if (净水 < 净水概率 * prop)
            {
                获得的物品.Add(new CleanWater());
                if (Rand.Int(100) < 净水双倍概率)
                    获得的物品.Add(new CleanWater());
            }

            player.AddItems(获得的物品);
            ExploreCount++;
            await player.DisplayAchievements(获得的物品);
        }
    }

    public class HutPlace : Place
    {
        public HutPlace(string name, int appearRate, int huntingRate)
        {
            Name = name;
            AppearRate = appearRate;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override bool HasLog => false;
        public override bool CanFish => true;
        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }

        /// <summary>
        /// Bottled Water x2
        /// Energy Bar x2
        /// Log x2
        /// Old Oxe x1(50%)
        /// Old Fish Rod x1(30%)
        /// Trap x1(20%)
        /// Old Shotgun x1(5%)
        /// In one game, player can only get either Trap x1 or Old Shotgun x1 in Hut.
        /// In one game, player can only get one of each tool. 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-1, AttrType.Water);
            player.Modify(-1, AttrType.Energy);
            const int 斧子概率 = 50, 鱼竿概率 = 30, 陷阱概率 = 20, 猎枪概率 = 5;
            var prop = ExploreCount == 0 ? 1 : 0;

            var 获得的物品 = new List<IItem>();

            if (!player.HasOxe)
            {
                var 斧子 = Rand.Int(100);
                if (斧子 < 斧子概率) 获得的物品.Add(new OldOxe());
            }

            if (!CanFish)
            {
                var 鱼竿 = Rand.Int(100);
                if (鱼竿 < 鱼竿概率) 获得的物品.Add(new OldFishRod());
            }


            var 瓶装水 = Rand.Int(100);
            if (瓶装水 < 100 * prop)
            {
                获得的物品.Add(new BottledWater());
                获得的物品.Add(new BottledWater());
            }

            var 能量棒 = Rand.Int(100);
            if (能量棒 < 100 * prop)
            {
                获得的物品.Add(new EnergyBar());
                获得的物品.Add(new EnergyBar());
            }

            var 木头 = Rand.Int(100);
            if (木头 < 100 * prop)
            {
                获得的物品.Add(LogItem.One);
                获得的物品.Add(LogItem.One);
            }

            if (!player.HasHuntingTool)
            {
                var s = Rand.Int(2);
                if (s == 0)
                {
                    var 猎枪 = Rand.Int(100);
                    if (猎枪 < 猎枪概率) 获得的物品.Add(new OldShotgun());
                }
                else if (s == 1)
                {
                    var 陷阱 = Rand.Int(100);
                    if (陷阱 < 陷阱概率) 获得的物品.Add(new Trap());
                }
            }

            player.AddItems(获得的物品);
            ExploreCount++;
            await player.DisplayAchievements(获得的物品);
        }
    }

    public class ForestPlace : Place
    {
        public ForestPlace(string name, int appearRate, int huntingRate)
        {
            Name = name;
            AppearRate = appearRate;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override bool HasLog => false;
        public override bool CanFish => true;
        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }

        /// <summary>
        /// Raw Fish x1(20%)
        /// Clean Water x1(70%) + x1(40%) 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-1, AttrType.Water);
            player.Modify(-1, AttrType.Energy);
            const int 浆果概率 = 30, 脏水概率 = 20, 木头概率 = 50, 坚果概率 = 60, 坚果双倍概率 = 40, 木头双倍概率 = 20;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var 获得的物品 = new List<IItem>();

            var 浆果 = Rand.Int(100);
            if (浆果 < 浆果概率 * prop)
                获得的物品.Add(new Berry());

            var 脏水 = Rand.Int(100);
            if (脏水 < 脏水概率 * prop)
                获得的物品.Add(new DirtyWater());

            var 木头 = Rand.Int(100);
            if (木头 < 木头概率 * prop)
            {
                获得的物品.Add(LogItem.One);
                if (Rand.Int(100) < 木头双倍概率)
                    获得的物品.Add(LogItem.One);
            }

            var 坚果 = Rand.Int(100);
            if (坚果 < 坚果概率 * prop)
            {
                获得的物品.Add(new Nuts());
                if (Rand.Int(100) < 坚果双倍概率)
                    获得的物品.Add(new Nuts());
            }


            player.AddItems(获得的物品);
            ExploreCount++;
            await player.DisplayAchievements(获得的物品);
        }
    }
}