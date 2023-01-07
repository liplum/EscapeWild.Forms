using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Recipes
    {
        public static void RegisterAll()
        {
            Craft.RegisterRecipe(new NamedRecipe(
                FireStarterItems.HandDrillKit,
                nameof(Sticks), nameof(Sticks)
            ));
        }
    }
}