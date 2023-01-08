using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;

namespace WildernessSurvival.Game.Subtropics
{
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
            player.Modify(AttrType.Food, -(0.08f * tool.CalcExtraCostByTool()), CostFix);
            player.Modify(AttrType.Water, -(0.08f * tool.CalcExtraCostByTool()), CostFix);
            player.Modify(AttrType.Energy, -(0.1f * tool.CalcExtraCostByTool()), CostFix);
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
            player.Modify(AttrType.Food, -0.02f, CostFix);
            player.Modify(AttrType.Water, -0.04f, CostFix);
            player.Modify(AttrType.Energy, -0.08f, CostFix);
            const int RawFishRate = 10, CleanWaterRate = 70, DoubleRate = 40;

            var proportion = 3 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 3f;

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
}