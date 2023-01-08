using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Game.Subtropics;

namespace WildernessSurvival.Game.Subtropics
{
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
            player.Modify(AttrType.Water, -0.04f, CostFix);
            player.Modify(AttrType.Energy, -0.08f, CostFix);
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
}