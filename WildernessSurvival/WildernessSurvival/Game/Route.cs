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
        private readonly List<Place> _allPlace;

        /// <summary>
        /// [0,1]
        /// </summary>
        private float Hardness { get; }

        public Route(string name, float hardness, params Place[] places)
        {
            Name = name;
            Hardness = hardness;
            _allPlace = places.ToList();
            foreach (var place in places)
            {
                place.Route = this;
            }

            _curPlace = _allPlace[0];
        }

        public Place InitialPlace => _allPlace[0];

        private Place _curPlace;

        public float HardnessFix(float raw)
        {
            float fixedValue;
            if (raw < 0) fixedValue = raw - raw * Hardness;
            else fixedValue = raw * (1f - Hardness);
            return fixedValue * (1f + Rand.Float(-0.01f, 0.01f));
        }

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

        public abstract int AppearRate { get; }

        public abstract int HuntingRate { get; }

        public abstract bool IsSpecial { get; }

        public IRoute<IPlace> Route { get; set; }
        private Route Owner => Route as Route;

        public float HardnessFix(float raw) => Owner.HardnessFix(raw);
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
        /// Cost: Food[0.05], Energy[0.08]
        /// Go to next place
        /// </summary>
        protected virtual async Task PerformMove(Player player)
        {
            player.Modify(-0.05f, AttrType.Food, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            player.AdvanceTrip();
            player.Location = await Owner.GoNextPlace(player);
            player.HasFire = false;
        }

        /// <summary>
        /// Rest
        /// Cost: Food[0.03], Water[0.03]
        /// Restore: Health[0.1], Energy[0.25]
        /// </summary>
        protected virtual async Task PerformRest(Player player)
        {
            player.Modify(-0.03f, AttrType.Food, HardnessFix);
            player.Modify(-0.03f, AttrType.Water, HardnessFix);
            player.Modify(0.1f, AttrType.Health, HardnessFix);
            player.Modify(0.25f, AttrType.Energy, HardnessFix);

            await App.Current.MainPage.DisplayAlert(
                title: ActionType.Rest.LocalizedName(),
                message: $"{Route.Name}.Common.Rest".Tr(),
                cancel: "OK".Tr()
            );
        }

        protected virtual async Task PerformFish(Player player)
        {
        }

        /// <summary>
        /// Hunt
        /// Cost: Food[0.05], Water[0.05], Energy[0.15]
        /// Prerequisites: Has any hunting tool
        /// </summary>
        protected virtual async Task PerformHunt(Player player)
        {
            player.Modify(-0.05f, AttrType.Food, HardnessFix);
            player.Modify(-0.05f, AttrType.Water, HardnessFix);
            player.Modify(-0.15f, AttrType.Energy, HardnessFix);


            var huntingTool = player.GetBestHuntingTool();
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

            var gained = new List<IItem>();
            var r = Rand.Int(100);
            if (r < rate)
            {
                gained.Add(new RawRabbit());
                if (Rand.Int(100) < doubleRate)
                    gained.Add(new RawRabbit());
            }

            player.AddItems(gained);
            await player.DisplayGainedItems(gained);
        }

        /// <summary>
        /// Cost: Food[0.02], Energy[0.3]
        /// Gain: Log x1 + x1(50%)
        /// </summary>
        protected virtual async Task PerformCutDownTree(Player player)
        {
            player.Modify(-0.02f, AttrType.Food, HardnessFix);
            player.Modify(-0.3f, AttrType.Energy, HardnessFix);
            var gained = new List<IItem>();
            gained.Add(LogItem.One);
            var rate = Rand.Int(100);
            if (rate < 50)
            {
                gained.Add(LogItem.One);
            }

            player.AddItems(gained);
            await player.DisplayGainedItems(gained);
        }

        protected abstract Task PerformExplore(Player player);

        /// <summary>
        /// Fire
        /// Cost: Water[0.01], Energy[0.05]
        /// Cost: Log x1
        /// </summary>
        protected virtual async Task PerformFire(Player player)
        {
            if (!player.HasWood) return;
            player.Modify(-0.01f, AttrType.Food, HardnessFix);
            player.Modify(-0.05f, AttrType.Energy, HardnessFix);
            player.ConsumeWood(1);
            player.HasFire = true;
            await App.Current.MainPage.DisplayAlert(
                title: ActionType.Fire.LocalizedName(),
                message: $"{Route.Name}.Common.Fire".Tr(),
                cancel: "OK".Tr()
            );
        }

        public virtual ISet<ActionType> AvailableActions
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
        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }


        /// <summary>
        /// Cost: Water[0.04], Energy[0.08]
        /// Berry x1(60%) + x1(30%)
        /// Dirty Water x1(60%) + x1(30%)
        /// Log x1(20%)
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-0.04f, AttrType.Water, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            const int BerryRate = 60, DirtyWaterRate = 60, LogRate = 20, DoubleRate = 30;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var gained = new List<IItem>();

            if (Rand.Int(100) < BerryRate * prop)
            {
                gained.Add(new Berry());
                if (Rand.Int(100) < DoubleRate)
                    gained.Add(new Berry());
            }

            if (Rand.Int(100) < DirtyWaterRate * prop)
            {
                gained.Add(new DirtyWater());
                if (Rand.Int(100) < DoubleRate)
                    gained.Add(new DirtyWater());
            }

            if (Rand.Int(100) < LogRate)
            {
                gained.Add(new DirtyWater());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
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

        public override ISet<ActionType> AvailableActions
        {
            get
            {
                var actions = base.AvailableActions;
                actions.Add(ActionType.Fish);
                return actions;
            }
        }

        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }

        /// <summary>
        /// Cost: Food[0.08], Water[0.08]
        /// Gain: Raw Fish x1(80%) + x1(20%)
        /// Prerequisites: Current location allows fishing.
        /// </summary>
        protected override async Task PerformFish(Player player)
        {
            player.Modify(-0.08f, AttrType.Food, HardnessFix);
            player.Modify(-0.08f, AttrType.Water, HardnessFix);
            var r = Rand.Int(100);
            var gained = new List<IItem>();
            if (r < 80)
            {
                var fishA = new RawFish();
                gained.Add(fishA);
                if (Rand.Int(100) < 20)
                {
                    var fishB = new RawFish();
                    gained.Add(fishB);
                }
            }

            player.AddItems(gained);
            await player.DisplayGainedItems(gained);
        }

        /// <summary>
        /// Cost: Water[0.04], Energy[0.08]
        /// Raw Fish x1(20%)
        /// Clean Water x1(70%) + x1(40%) 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-0.04f, AttrType.Water, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            const int RawFishRate = 20, CleanWaterRate = 70, doubleRate = 40;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var gained = new List<IItem>();

            var 生鱼 = Rand.Int(100);
            if (生鱼 < RawFishRate * prop)
                gained.Add(new RawFish());

            var 净水 = Rand.Int(100);
            if (净水 < CleanWaterRate * prop)
            {
                gained.Add(new CleanWater());
                if (Rand.Int(100) < doubleRate)
                    gained.Add(new CleanWater());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
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
        public override int HuntingRate { get; }
        public override bool IsSpecial => true;
        public override int AppearRate { get; }

        public override ISet<ActionType> AvailableActions
        {
            get
            {
                var actions = base.AvailableActions;
                actions.Add(ActionType.CutDownTree);
                return actions;
            }
        }

        /// <summary>
        /// Cost: Water[0.04], Energy[0.08]
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
            player.Modify(-0.04f, AttrType.Water, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            const int oxeRate = 50, FishRodRate = 30, TrapRate = 20, GunRate = 5;
            var exploreFix = ExploreCount == 0 ? 1 : 0;

            var gained = new List<IItem>();

            if (!player.HasOxe)
            {
                if (Rand.Int(100) < oxeRate) gained.Add(new OldOxe());
            }

            if (!player.HasFishingTool)
            {
                if (Rand.Int(100) < FishRodRate) gained.Add(new OldFishRod());
            }


            if (Rand.Int(100) < 100 * exploreFix)
            {
                gained.Add(new BottledWater());
                gained.Add(new BottledWater());
                gained.Add(new EnergyBar());
                gained.Add(new EnergyBar());
                gained.Add(LogItem.One);
                gained.Add(LogItem.One);
            }


            if (!player.HasHuntingTool)
            {
                var s = Rand.Int(2);
                if (s == 0)
                {
                    if (Rand.Int(100) < GunRate) gained.Add(new OldShotgun());
                }
                else if (s == 1)
                {
                    if (Rand.Int(100) < TrapRate) gained.Add(new Trap());
                }
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
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
        public override int HuntingRate { get; }
        public override bool IsSpecial => false;
        public override int AppearRate { get; }

        public override ISet<ActionType> AvailableActions
        {
            get
            {
                var actions = base.AvailableActions;
                actions.Add(ActionType.CutDownTree);
                return actions;
            }
        }

        /// <summary>
        /// Cost: Water[0.04], Energy[0.08]
        /// Raw Fish x1(20%)
        /// Clean Water x1(70%) + x1(40%) 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-0.04f, AttrType.Water, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            const int BerryRate = 30,
                DirtyWaterRate = 20,
                LogRate = 50,
                NutsRate = 60,
                NutsDoubleRate = 40,
                LogDoubleRate = 20;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var gained = new List<IItem>();

            if (Rand.Int(100) < BerryRate * prop)
                gained.Add(new Berry());

            if (Rand.Int(100) < DirtyWaterRate * prop)
                gained.Add(new DirtyWater());

            if (Rand.Int(100) < LogRate * prop)
            {
                gained.Add(LogItem.One);
                if (Rand.Int(100) < LogDoubleRate)
                    gained.Add(LogItem.One);
            }

            if (Rand.Int(100) < NutsRate * prop)
            {
                gained.Add(new Nuts());
                if (Rand.Int(100) < NutsDoubleRate)
                    gained.Add(new Nuts());
            }


            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }
}