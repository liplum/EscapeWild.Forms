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
        public class Entry
        {
            public Place Place;

            /// <summary>
            /// The appear rate of all places.
            /// Larger proportion will be more likely to appear.
            /// </summary>
            public int Proportion;

            /// <summary>
            /// [0f,1f]
            /// Larger inertia will try to keep player stay the same location.
            /// If it's zero, player will leave the place instantly. 
            /// </summary>
            public float Inertia;

            private int _maxStayCount;

            public int MaxStayCount
            {
                get => _maxStayCount;
                set => _maxStayCount = Math.Max(value, 1);
            }

            public override string ToString() => Place.ToString();
        }

        private class EntryImpl
        {
            public Entry Meta;
            public float AppearRate;
            public int AppearCount;
            public int Proportion => Meta.Proportion;
            public float Inertia => Meta.Inertia;
            public int MaxStayCount => Meta.MaxStayCount;
            public Place Place => Meta.Place;
            public override string ToString() => Meta.Place.ToString();
        }

        public string Name { get; }
        private readonly List<EntryImpl> _entries;

        /// <summary>
        /// [0,1]
        /// </summary>
        private float Hardness { get; }

        public Route(string name, float hardness, params Entry[] entries)
        {
            Name = name;
            Hardness = hardness;
            _entries = new List<EntryImpl>();
            var sum = 0f;
            foreach (var entry in entries)
            {
                entry.Place.Route = this;
                _entries.Add(new EntryImpl
                {
                    Meta = entry,
                });
                sum += entry.Proportion;
            }

            foreach (var entry in _entries)
            {
                entry.AppearRate = entry.Proportion / sum;
            }

            _cur = _entries[0];
        }

        public Place InitialPlace => _entries[0].Place;

        private EntryImpl _cur;

        public float HardnessFix(float raw)
        {
            float fixedValue;
            if (raw < 0) fixedValue = raw - raw * Hardness;
            else fixedValue = raw * (1f - Hardness);
            return fixedValue * (1f + Rand.Float(-0.01f, 0.01f));
        }

        public async Task<Place> GoNextPlace(Player player)
        {
            // Stay the same place if player is "attracted".
            var isStay = Rand.Float() < _cur.Inertia * (1f - (float)_cur.AppearCount / _cur.MaxStayCount);
            if (isStay) return _cur.Place;
            var cur = _cur;
            var otherPlaces = (from p in _entries where p != cur select p).ToList();
            var changeHit = Rand.Float();
            var sum = 0f;
            foreach (var test in otherPlaces)
            {
                sum += test.AppearRate;
                if (changeHit <= sum)
                {
                    // Hit the place
                    await _cur.Place.OnLeave(player);
                    _cur = test;
                    await test.Place.OnEnter(player);
                    _cur.AppearCount++;
                    return test.Place;
                }
            }

            return _cur.Place;
        }
    }

    public abstract class Place : IPlace
    {
        public abstract string Name { get; }

        public abstract int HuntingRate { get; }

        public IRoute<IPlace> Route { get; set; }
        protected Route Owner => Route as Route;

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
        /// Cost: Food[0.05], Water[0.05], Energy[0.135]
        /// Go to next place
        /// </summary>
        protected virtual async Task PerformMove(Player player)
        {
            player.Modify(-0.05f, AttrType.Food, HardnessFix);
            player.Modify(-0.05f, AttrType.Water, HardnessFix);
            player.Modify(-0.135f, AttrType.Energy, HardnessFix);
            player.AdvanceTrip(HardnessFix(Player.MoveStep));
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

            await ShowRestDialog();
        }

        protected async Task ShowRestDialog(string restType = "Common")
        {
            await App.Current.MainPage.DisplayAlert(
                title: ActionType.Rest.LocalizedName(),
                message: $"{Route.Name}.{restType}.Rest".Tr(),
                cancel: "OK".Tr()
            );
        }

        protected virtual async Task PerformFish(Player player)
        {
        }

        /// <summary>
        /// Hunt
        /// Cost: Food[0.08], Water[0.08], Energy[0.15]
        /// Prerequisites: Has any hunting tool
        /// </summary>
        protected virtual async Task PerformHunt(Player player)
        {
            player.Modify(-0.08f, AttrType.Food, HardnessFix);
            player.Modify(-0.08f, AttrType.Water, HardnessFix);
            player.Modify(-0.15f, AttrType.Energy, HardnessFix);


            var huntingTool = player.GetBestToolOf(ToolType.Hunting);
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
                gained.Add(new RawRabbit
                {
                    FoodRestore = RawRabbit.DefaultFoodRestore * Rand.Float(1f, 1.2f),
                    WaterRestore = RawRabbit.DefaultWaterRestore * Rand.Float(1f, 1.2f),
                });
                if (Rand.Int(100) < doubleRate)
                {
                    gained.Add(new RawRabbit
                    {
                        FoodRestore = RawRabbit.DefaultFoodRestore * Rand.Float(1f, 1.5f),
                        WaterRestore = RawRabbit.DefaultWaterRestore * Rand.Float(1f, 1.5f),
                    });
                }
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
            gained.Add(new Log
            {
                Fuel = Log.DefaultFuel * Rand.Float(0.55f, 0.75f),
            });
            if (Rand.Int(100) < 50)
            {
                gained.Add(new Log
                {
                    Fuel = Log.DefaultFuel * Rand.Float(0.55f, 0.75f),
                });
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
        public PlainPlace(string name, int huntingRate)
        {
            Name = name;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override int HuntingRate { get; }

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
        public RiversidePlace(string name, int huntingRate)
        {
            Name = name;
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
        /// Cost: Food[0.02], Water[0.04], Energy[0.08]
        /// Raw Fish x1(20%)
        /// Clean Water x1(70%) + x1(40%) 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-0.02f, AttrType.Food, HardnessFix);
            player.Modify(-0.04f, AttrType.Water, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            const int RawFishRate = 20, CleanWaterRate = 70, DoubleRate = 40;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var gained = new List<IItem>();

            if (Rand.Int(100) < RawFishRate * prop)
                gained.Add(new RawFish());

            if (Rand.Int(100) < CleanWaterRate * prop)
            {
                gained.Add(new CleanWater());
                if (Rand.Int(100) < DoubleRate)
                    gained.Add(new CleanWater());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }

    public class HutPlace : Place
    {
        public HutPlace(string name, int huntingRate)
        {
            Name = name;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override int HuntingRate { get; }

        public override ISet<ActionType> AvailableActions
        {
            get
            {
                var actions = base.AvailableActions;
                actions.Add(ActionType.CutDownTree);
                return actions;
            }
        }

        public override async Task OnEnter(Player player)
        {
        }

        public override async Task OnLeave(Player player)
        {
        }

        /// <summary>
        /// Rest
        /// Cost: Food[0.03], Water[0.03]
        /// Restore: Health[0.15], Energy[0.4]
        /// </summary>
        protected override async Task PerformRest(Player player)
        {
            player.Modify(-0.03f, AttrType.Food, HardnessFix);
            player.Modify(-0.03f, AttrType.Water, HardnessFix);
            player.Modify(0.15f, AttrType.Health, HardnessFix);
            player.Modify(0.4f, AttrType.Energy, HardnessFix);

            await ShowRestDialog(restType: "Hut");
        }

        private bool _gotOxe, _gotFishRod, _gotGun, _gotTrap;

        /// <summary>
        /// Cost: Food[0.02], Water[0.04], Energy[0.08]
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
            player.Modify(-0.02f, AttrType.Food, HardnessFix);
            player.Modify(-0.04f, AttrType.Water, HardnessFix);
            player.Modify(-0.08f, AttrType.Energy, HardnessFix);
            const int OxeRate = 50, FishRodRate = 30, TrapRate = 20, GunRate = 5;

            var gained = new List<IItem>();

            if (!_gotOxe)
            {
                _gotOxe = true;
                if (Rand.Int(100) < OxeRate)
                    gained.Add(OxeItems.OldOxe());
            }

            if (!_gotFishRod)
            {
                _gotFishRod = true;
                if (Rand.Int(100) < FishRodRate)
                    gained.Add(FishToolItems.OldFishRod());
            }


            if (ExploreCount < 1)
            {
                gained.Add(new BottledWater
                {
                    Restore = BottledWater.DefaultRestore * Rand.Float(0.8f, 1.2f),
                });
                gained.Add(new BottledWater
                {
                    Restore = BottledWater.DefaultRestore * Rand.Float(0.7f, 1.15f),
                });
                gained.Add(new EnergyBar
                {
                    FoodRestore = EnergyBar.DefaultFoodRestore * Rand.Float(0.8f, 1.3f)
                });
                gained.Add(new EnergyBar
                {
                    FoodRestore = EnergyBar.DefaultFoodRestore * Rand.Float(0.75f, 1.2f)
                });
                gained.Add(new Log
                {
                    Fuel = Log.DefaultFuel * Rand.Float(0.95f, 1f),
                });
                gained.Add(new Log
                {
                    Fuel = Log.DefaultFuel * Rand.Float(0.95f, 1f),
                });
            }


            if (!_gotGun || !_gotTrap)
            {
                if (Rand.Bool())
                {
                    if (!_gotGun)
                    {
                        _gotGun = true;
                        if (Rand.Int(100) < GunRate)
                            gained.Add(HuntingToolItems.OldShotgun());
                    }
                }
                else
                {
                    if (_gotTrap)
                    {
                        _gotTrap = true;
                        if (Rand.Int(100) < TrapRate)
                            gained.Add(HuntingToolItems.Trap());
                    }
                }
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }

    public class ForestPlace : Place
    {
        public ForestPlace(string name, int huntingRate)
        {
            Name = name;
            HuntingRate = huntingRate;
        }

        public override string Name { get; }
        public override int HuntingRate { get; }

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
        /// Cost: Food[0.02], Water[0.05], Energy[0.10]
        /// Berry x1(30%)
        /// Dirty Water x1(20%)
        /// Log x1(60%) + x1(20%)
        /// Nuts x1(60%) + x1(40%)
        /// Stick x1(60%) or x2(30%) || x3(10%)
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(-0.02f, AttrType.Food, HardnessFix);
            player.Modify(-0.05f, AttrType.Water, HardnessFix);
            player.Modify(-0.10f, AttrType.Energy, HardnessFix);
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
            {
                gained.Add(new Berry
                {
                    FoodRestore = Berry.DefaultFoodRestore * Rand.Float(0.8f, 1.5f),
                });
            }

            if (Rand.Int(100) < DirtyWaterRate * prop)
            {
                gained.Add(new DirtyWater
                {
                    Restore = DirtyWater.DefaultRestore * Rand.Float(0.5f, 1f)
                });
            }

            if (Rand.Int(100) < LogRate * prop)
            {
                gained.Add(new Log
                {
                    Fuel = Log.DefaultFuel * Rand.Float(0.5f, 0.75f),
                });
                if (Rand.Int(100) < LogDoubleRate)
                {
                    gained.Add(new Log
                    {
                        Fuel = Log.DefaultFuel * Rand.Float(0.6f, 0.8f),
                    });
                }
            }

            if (Rand.Int(100) < NutsRate * prop)
            {
                gained.Add(new Nuts
                {
                    Restore = Nuts.DefaultRestore * Rand.Float(0.8f, 1.1f)
                });
                if (Rand.Int(100) < NutsDoubleRate)
                {
                    gained.Add(new Nuts
                    {
                        Restore = Nuts.DefaultRestore * Rand.Float(0.6f, 1.1f)
                    });
                }
            }

            var stick = Rand.Int(100);
            if (stick < 60)
            {
                gained.Add(new Stick());
            }
            else if (stick < 90)
            {
                gained.Add(new Stick());
                gained.Add(new Stick());
            }
            else
            {
                gained.Add(new Stick());
                gained.Add(new Stick());
                gained.Add(new Stick());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }
}