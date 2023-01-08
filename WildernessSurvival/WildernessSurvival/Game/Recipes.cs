using System.Linq;
using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Recipes
    {
        public static void RegisterAll()
        {
            Craft.RegisterRecipe(new NamedRecipe(
                nameof(Sticks), nameof(Sticks)
            )
            {
                Output = inputs => new HandDrillKit
                {
                    InitialFireFuel = HandDrillKit.DefaultInitialFireFuel +
                                      inputs.OfType<IFuelItem>().Sum(e => e.Fuel)
                },
                Preview = () => new HandDrillKit(),
                Modifiers = new[]
                {
                    AttrType.Energy.WithEffect(-0.05f)
                }
            });
        }
    }
}