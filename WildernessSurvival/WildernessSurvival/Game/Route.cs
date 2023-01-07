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

            public int MaxAppearCount;

            public int MaxStayCount;

            public override string ToString() => Place.ToString();
        }

        private class EntryImpl
        {
            public Entry Meta;
            public float AppearRate;
            public int AppearCount;
            public int Proportion => Meta.Proportion;
            public bool CanAppear => MaxAppearCount <= 0 || AppearCount < MaxAppearCount;
            public int MaxAppearCount => Meta.MaxAppearCount;
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
            var old = _cur;
            var otherPlaces = (from p in _entries where p != old && p.CanAppear select p).ToList();
            var changeHit = Rand.Float(0f, otherPlaces.Sum(e => e.AppearRate));
            var sum = 0f;
            foreach (var newPlace in otherPlaces)
            {
                sum += newPlace.AppearRate;
                if (changeHit <= sum)
                {
                    // Hit the place
                    await old.Place.OnLeave(player);
                    _cur = newPlace;
                    await newPlace.Place.OnEnter(player);
                    newPlace.AppearCount++;
                    return newPlace.Place;
                }
            }

            return old.Place;
        }
    }

    public abstract class Place : IPlace
    {
        public string Name { get; set; }

        public int HuntingRate { get; set; }

        public IRoute<IPlace> Route { get; set; }
        public float Wet { get; set; }

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
            player.Modify(AttrType.Food, -0.05f, HardnessFix);
            player.Modify(AttrType.Water, -0.05f, HardnessFix);
            player.Modify(AttrType.Energy, -0.135f, HardnessFix);
            player.AdvanceTrip(HardnessFix(Player.MoveStep));
            player.Location = await Owner.GoNextPlace(player);
            player.FireFuel = 0;
        }

        /// <summary>
        /// Rest
        /// Cost: Food[0.03], Water[0.03]
        /// Restore: Health[0.1], Energy[0.25]
        /// </summary>
        protected virtual async Task PerformRest(Player player)
        {
            player.Modify(AttrType.Food, -0.03f, HardnessFix);
            player.Modify(AttrType.Water, -0.03f, HardnessFix);
            if (player.Food > 0f && player.Water > 0f)
            {
                player.Modify(AttrType.Health, 0.1f, HardnessFix);
                player.Modify(AttrType.Energy, 0.25f, HardnessFix);
            }
            else
            {
                player.Modify(AttrType.Energy, 0.05f, HardnessFix);
            }

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
        /// Cost based on tool level
        /// Prerequisites: Has any hunting tool
        /// </summary>
        protected virtual async Task PerformHunt(Player player)
        {
            var tool = player.TryGetBestToolOf(ToolType.Hunting);
            player.Modify(AttrType.Food, -(0.08f * tool.CalcExtraCostByTool()), HardnessFix);
            player.Modify(AttrType.Water, -(0.08f * tool.CalcExtraCostByTool()), HardnessFix);
            player.Modify(AttrType.Energy, -(0.15f * tool.CalcExtraCostByTool()), HardnessFix);

            var (rate, doubleRate) = tool.CalcRateByTool();

            var gained = new List<IItem>();
            var broken = await player.DamageTool(tool, 3f);
            if (Rand.Int(100) < rate * HuntingRate / 100f)
            {
                gained.Add(new RawRabbit
                {
                    FoodRestore = RawRabbit.DefaultFoodRestore * Rand.Float(1f, 1.2f),
                    WaterRestore = RawRabbit.DefaultWaterRestore * Rand.Float(1f, 1.2f),
                });
                if (!broken && Rand.Int(100) < doubleRate * HuntingRate / 100f)
                {
                    gained.Add(new RawRabbit
                    {
                        FoodRestore = RawRabbit.DefaultFoodRestore * Rand.Float(1f, 1.5f),
                        WaterRestore = RawRabbit.DefaultWaterRestore * Rand.Float(1f, 1.5f),
                    });
                    await player.DamageTool(tool, 2f);
                }
            }

            player.AddItems(gained);
            await player.DisplayGainedItems(gained);
        }

        /// <summary>
        /// Cost: Food[0.04], Water[0.03], Energy[0.3]
        /// Cost based on tool level
        /// Gain: Log x1 or x2, Sticks x2 or x3
        /// </summary>
        protected virtual async Task PerformCutDownTree(Player player)
        {
            var tool = player.TryGetBestToolOf(ToolType.Oxe);
            player.Modify(AttrType.Food, -(0.04f * tool.CalcExtraCostByTool()), HardnessFix);
            player.Modify(AttrType.Water, -(0.03f * tool.CalcExtraCostByTool()), HardnessFix);
            player.Modify(AttrType.Energy, -(0.3f * tool.CalcExtraCostByTool()), HardnessFix);
            var (rate, _) = tool.CalcRateByTool();
            var gained = new List<IItem>();
            gained.Add(new Log
            {
                Fuel = Log.DefaultFuel * Rand.Float(0.55f, 0.75f),
            });
            for (var i = 0; i < 2; i++)
            {
                gained.Add(new Sticks());
            }

            var broken = await player.DamageTool(tool, 3f);
            if (!broken && Rand.Int(100) < rate)
            {
                gained.Add(new Log
                {
                    Fuel = Log.DefaultFuel * Rand.Float(0.55f, 0.75f),
                });
                gained.Add(new Sticks());
                await player.DamageTool(tool, 2f);
            }

            player.AddItems(gained);
            await player.DisplayGainedItems(gained);
        }

        protected abstract Task PerformExplore(Player player);

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
        /// <summary>
        /// Cost: Water[0.04], Energy[0.08]
        /// Berry x1(60%) + x1(30%)
        /// Dirty Water x1(60%) + x1(30%)
        /// Stick x1(30%)
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(AttrType.Water, -0.04f, HardnessFix);
            player.Modify(AttrType.Energy, -0.08f, HardnessFix);
            const int BerryRate = 60, DirtyWaterRate = 60, StickRate = 30, DoubleRate = 30;

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
                if (Rand.Int(100) < DoubleRate)
                {
                    gained.Add(new Berry
                    {
                        FoodRestore = Berry.DefaultFoodRestore * Rand.Float(0.8f, 1.5f),
                    });
                }
            }

            if (Rand.Int(100) < DirtyWaterRate * prop)
            {
                gained.Add(new DirtyWater
                {
                    WaterRestore = DirtyWater.DefaultWaterRestore * Rand.Float(0.9f, 1.2f)
                });
                if (Rand.Int(100) < DoubleRate)
                {
                    gained.Add(new DirtyWater
                    {
                        WaterRestore = DirtyWater.DefaultWaterRestore * Rand.Float(0.8f, 1.5f)
                    });
                }
            }

            if (Rand.Int(100) < StickRate)
            {
                gained.Add(new Sticks());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }

    public class RiversidePlace : Place
    {
        public override ISet<ActionType> AvailableActions
        {
            get
            {
                var actions = base.AvailableActions;
                actions.Add(ActionType.Fish);
                return actions;
            }
        }


        /// <summary>
        /// Cost: Food[0.08], Water[0.08], Energy[0.1]
        /// Cost based on tool level
        /// Gain: Raw Fish x1 or x2
        /// Tool damage: 3f + 2f
        /// Prerequisites: Current location allows fishing.
        /// </summary>
        protected override async Task PerformFish(Player player)
        {
            var tool = player.TryGetBestToolOf(ToolType.Fishing);
            player.Modify(AttrType.Food, -(0.08f * tool.CalcExtraCostByTool()), HardnessFix);
            player.Modify(AttrType.Water, -(0.08f * tool.CalcExtraCostByTool()), HardnessFix);
            player.Modify(AttrType.Energy, -(0.1f * tool.CalcExtraCostByTool()), HardnessFix);
            var (rate, doubleRate) = tool.CalcRateByTool();

            var gained = new List<IItem>();
            var broken = await player.DamageTool(tool, 3f);
            if (Rand.Int(100) < rate)
            {
                gained.Add(new RawFish
                {
                    FoodRestore = RawFish.DefaultFoodRestore * Rand.Float(0.9f, 1.1f),
                    WaterRestore = RawFish.DefaultWaterRestore * Rand.Float(0.8f, 1.4f),
                });
                if (!broken && Rand.Int(100) < doubleRate)
                {
                    gained.Add(new RawFish
                    {
                        FoodRestore = RawFish.DefaultFoodRestore * Rand.Float(0.9f, 1.1f),
                        WaterRestore = RawFish.DefaultWaterRestore * Rand.Float(0.8f, 1.4f),
                    });
                    await player.DamageTool(tool, 2f);
                }
            }

            player.AddItems(gained);
            await player.DisplayGainedItems(gained);
        }

        /// <summary>
        /// Cost: Food[0.02], Water[0.04], Energy[0.08]
        /// Raw Fish x1(10%)
        /// Clean Water x1(70%) + x1(40%) 
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(AttrType.Food, -0.02f, HardnessFix);
            player.Modify(AttrType.Water, -0.04f, HardnessFix);
            player.Modify(AttrType.Energy, -0.08f, HardnessFix);
            const int RawFishRate = 10, CleanWaterRate = 70, DoubleRate = 40;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var gained = new List<IItem>();

            if (Rand.Int(100) < RawFishRate * prop)
                gained.Add(new RawFish
                {
                    FoodRestore = RawFish.DefaultFoodRestore * Rand.Float(0.9f, 1.1f),
                    WaterRestore = RawFish.DefaultWaterRestore * Rand.Float(0.8f, 1.4f),
                });

            if (Rand.Int(100) < CleanWaterRate * prop)
            {
                gained.Add(new CleanWater
                {
                    Restore = CleanWater.DefaultRestore * Rand.Float(0.8f, 1.2f)
                });
                if (Rand.Int(100) < DoubleRate)
                {
                    gained.Add(new CleanWater
                    {
                        Restore = CleanWater.DefaultRestore * Rand.Float(0.9f, 1.45f)
                    });
                }
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }

    public class HutPlace : Place
    {
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
            player.Modify(AttrType.Food, -0.03f, HardnessFix);
            player.Modify(AttrType.Water, -0.03f, HardnessFix);
            if (player.Food > 0f && player.Water > 0f)
            {
                player.Modify(AttrType.Health, 0.15f, HardnessFix);
                player.Modify(AttrType.Energy, 0.4f, HardnessFix);
            }
            else
            {
                player.Modify(AttrType.Energy, 0.1f, HardnessFix);
            }

            await ShowRestDialog(restType: "Hut");
        }

        private bool _gotOxe, _gotFishRod, _gotGun, _gotTrap;

        /// <summary>
        /// Cost: Food[0.01], Water[0.02], Energy[0.03]
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
            player.Modify(AttrType.Food, -0.01f, HardnessFix);
            player.Modify(AttrType.Water, -0.02f, HardnessFix);
            player.Modify(AttrType.Energy, -0.03f, HardnessFix);
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
            player.Modify(AttrType.Food, -0.02f, HardnessFix);
            player.Modify(AttrType.Water, -0.05f, HardnessFix);
            player.Modify(AttrType.Energy, -0.10f, HardnessFix);
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
                    WaterRestore = DirtyWater.DefaultWaterRestore * Rand.Float(0.5f, 1f)
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
                gained.Add(new Sticks());
            }
            else if (stick < 90)
            {
                gained.Add(new Sticks());
                gained.Add(new Sticks());
            }
            else
            {
                gained.Add(new Sticks());
                gained.Add(new Sticks());
                gained.Add(new Sticks());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }
}