using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Game.Subtropics;

namespace WildernessSurvival.Game.Subtropics
{
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
        /// Log x1(20%)
        /// Nuts x1(60%) + x1(40%)
        /// Stick x1(60%) || x2(30%) || x3(10%)
        /// Unknown Mushrooms (3%)
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            player.Modify(AttrType.Food, -0.02f, HardnessFix);
            player.Modify(AttrType.Water, -0.05f, HardnessFix);
            player.Modify(AttrType.Energy, -0.10f, HardnessFix);
            const float BerryRate = 0.3f,
                LogRate = 0.2f,
                NutsRate = 0.5f,
                NutsDoubleRate = 0.4f;

            var proportion = 10 - ExploreCount;
            proportion = proportion <= 0 ? 1 : proportion;
            var prop = proportion / 10f;

            var gained = new List<IItem>();

            if (Rand.Float() < BerryRate * prop)
            {
                gained.Add(new Berry
                {
                    FoodRestore = Berry.DefaultFoodRestore * Rand.Float(0.8f, 1.5f),
                });
            }

            if (Rand.Float() < LogRate * prop)
            {
                gained.Add(new Log
                {
                    Fuel = Log.DefaultFuel * Rand.Float(0.5f, 0.75f),
                });
            }

            if (Rand.Float() < NutsRate * prop)
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

            var stick = Rand.Float();
            if (stick < 0.6f)
            {
                gained.Add(new Sticks());
            }
            else if (stick < 0.9f)
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

            if (Rand.Float() < 0.03f)
            {
                gained.Add(UnknownMushrooms.Random());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }
}