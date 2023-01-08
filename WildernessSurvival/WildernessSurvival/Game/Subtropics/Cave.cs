// ReSharper disable CheckNamespace

using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Game.Subtropics;

namespace WildernessSurvival.Game
{
    public class CavePlace : Place
    {
        /// <summary>
        /// Cost: Food[0.05], Water[0.06], Energy[0.12]
        /// Unknown Mushrooms (5%)
        /// </summary>
        protected override async Task PerformExplore(Player player)
        {
            var gained = new List<IItem>();
            if (Rand.Float() < 0.05f)
            {
                gained.Add(UnknownMushrooms.Random());
            }

            player.AddItems(gained);
            ExploreCount++;
            await player.DisplayGainedItems(gained);
        }
    }
}