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

    /// <summary>
    /// For generating a route
    /// </summary>
    public class RouteBlock
    {
        public Func<IPlace> Place { get; set; }

        /// <summary>
        /// [0f,1f]
        /// Larger BlockSize will try to generate more identical place next to next.
        /// </summary>
        public float BlockSize { get; set; }
    }

    public class RouteEntry
    {
    }

    public class GenerateContext
    {
        public int TotalPlaceNumber { get; set; }
    }

    public class RouteGenerator
    {
        public IList<RouteEntry> Generate()
        {
            return new List<RouteEntry>();
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
}