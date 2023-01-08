using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;
using WildernessSurvival.Utils;

namespace WildernessSurvival.Game.Subtropics
{
    public class Route : IRoute<Place>
    {
        public string Name { get; set; }
        private readonly List<RouteEntry> _entries;

        public float _routeProgress;

        public float GetRouteProgress()
        {
            return _routeProgress;
        }

        public async Task SetRouteProgress(Player player, float value)
        {
            var old = CurrentEntry;
            await old.Place.OnLeave(player);
            _routeProgress = value;
            await CurrentEntry.Place.OnEnter(player);
        }

        public Hardness Hardness;

        public Route(List<RouteEntry> entries)
        {
            entries.ForEach(e => e.Place.Route = this);
            _entries = entries;
        }

        public Place InitialPlace => _entries[0].Place;

        /// <summary>
        /// [0f,1f]
        /// </summary>
        public float JourneyProgress => _routeProgress / (_entries.Count - 1);

        public RouteEntry CurrentEntry => _entries[((int)_routeProgress).CoerceIn(0, _entries.Count - 1)];
        public Place CurrentPlace => _entries[((int)_routeProgress).CoerceIn(0, _entries.Count - 1)].Place;
    }

    /// <summary>
    /// For generating a route
    /// </summary>
    public class RouteBlock
    {
        public Func<Place> Place { get; set; }

        /// <summary>
        /// [0f,1f]
        /// Larger BlockSize will try to generate more identical place next to next.
        /// </summary>
        public float BlockSize { get; set; }
    }

    public class RouteEntry
    {
        public Place Place;
    }

    public class RouteGenerator
    {
        public Hardness Hardness;

        public IList<RouteBlock> Blocks;

        public Action<List<RouteEntry>> Decorate;

        public Route Generate(string name)
        {
            var blocks = GenerateWithBlock();
            Decorate?.Invoke(blocks);
            return new Route(blocks)
            {
                Name = name,
                Hardness = Hardness,
            };
        }

        private List<RouteEntry> GenerateWithBlock()
        {
            // To prompt how many place should be generated.
            var PlaceNumberPrompt = Hardness.JourneyLength();
            var res = new List<RouteEntry>();
            var totalSize = Blocks.Sum(e => e.BlockSize);
            foreach (var block in Blocks)
            {
                var shouldGenerate = (int)(block.BlockSize / totalSize * PlaceNumberPrompt);
                for (var i = 0; i < shouldGenerate; i++)
                {
                    var place = block.Place();
                    res.Add(new RouteEntry
                    {
                        Place = place
                    });
                }
            }

            return res;
        }
    }

    public abstract class Place : IPlace
    {
        public string Name { get; set; }

        public int HuntingRate { get; set; }

        public IRoute<IPlace> Route { get; set; }
        public float Wet { get; set; }

        protected Route Owner => Route as Route;

        public float CostFix(float raw) => Owner.Hardness.AttrCostFix(raw);
        public float BounceFix(float raw) => Owner.Hardness.AttrBounceFix(raw);
        protected int ExploreCount;

        public virtual async Task OnLeave(Player player)
        {
            if (player.HasFire)
            {
                await App.Current.MainPage.DisplayAlert(
                    title: ActionType.Fire.LocalizedName(),
                    message: "Fire.PutOut.Move".Tr(),
                    cancel: "OK".Tr()
                );
                player.FireFuel = 0;
            }
        }


        public virtual async Task OnEnter(Player player)
        {
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
            player.Modify(AttrType.Food, -0.05f, CostFix);
            player.Modify(AttrType.Water, -0.05f, CostFix);
            player.Modify(AttrType.Energy, -0.135f, CostFix);
            var routeProgress = Owner.GetRouteProgress();
            await Owner.SetRouteProgress(player, routeProgress + 1f);
            player.JourneyProgress = Owner.JourneyProgress;
            player.Location = Owner.CurrentPlace;
        }

        /// <summary>
        /// Rest
        /// Cost: Food[0.03], Water[0.03]
        /// Restore: Health[0.1], Energy[0.25]
        /// </summary>
        protected virtual async Task PerformRest(Player player)
        {
            player.Modify(AttrType.Food, -0.03f, CostFix);
            player.Modify(AttrType.Water, -0.03f, CostFix);
            if (player.Food > 0f && player.Water > 0f)
            {
                player.Modify(AttrType.Health, 0.1f, BounceFix);
                player.Modify(AttrType.Energy, 0.25f, BounceFix);
            }
            else
            {
                player.Modify(AttrType.Energy, 0.05f, BounceFix);
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
            player.Modify(AttrType.Food, -(0.08f * tool.CalcExtraCostByTool()), CostFix);
            player.Modify(AttrType.Water, -(0.08f * tool.CalcExtraCostByTool()), CostFix);
            player.Modify(AttrType.Energy, -(0.15f * tool.CalcExtraCostByTool()), CostFix);

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
            player.Modify(AttrType.Food, -(0.04f * tool.CalcExtraCostByTool()), CostFix);
            player.Modify(AttrType.Water, -(0.03f * tool.CalcExtraCostByTool()), CostFix);
            player.Modify(AttrType.Energy, -(0.3f * tool.CalcExtraCostByTool()), CostFix);
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