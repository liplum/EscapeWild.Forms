
using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Game.Subtropics;

namespace WildernessSurvival.Game.Subtropics
{
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
            player.Modify(AttrType.Food, -0.03f, CostFix);
            player.Modify(AttrType.Water, -0.03f, CostFix);
            if (player.Food > 0f && player.Water > 0f)
            {
                player.Modify(AttrType.Health, 0.15f, BounceFix);
                player.Modify(AttrType.Energy, 0.4f, BounceFix);
            }
            else
            {
                player.Modify(AttrType.Energy, 0.1f, CostFix);
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
            player.Modify(AttrType.Food, -0.01f, CostFix);
            player.Modify(AttrType.Water, -0.02f, CostFix);
            player.Modify(AttrType.Energy, -0.03f, CostFix);
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
}