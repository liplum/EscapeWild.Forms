using System.Collections.Generic;
using System.Linq;

namespace WildernessSurvival.Core
{
    public interface IRecipe
    {
        /// <summary>
        /// Return an Item for display purpose
        /// </summary>
        IItem TryGetResult(Backpack backpack);

        IItem ConsumeAndCraft(Backpack backpack);
    }

    public static class Craft
    {
        public static List<IRecipe> Recipes = new List<IRecipe>();

        public static void RegisterRecipe(IRecipe recipe)
        {
            Recipes.Add(recipe);
        }

        public static void RegisterRecipes(params IRecipe[] recipes)
        {
            Recipes.AddRange(recipes);
        }

        public static IList<(IRecipe recipe, IItem output)> TestAvailableRecipes(Backpack backpack)
        {
            return (from recipe in Recipes
                let output = recipe.TryGetResult(backpack)
                where output != null
                select (recipe, output)).ToList();
        }
    }

    public class NamedRecipe : IRecipe
    {
        private readonly ItemMaker<IItem> _output;
        private readonly string[] _requirements;

        public NamedRecipe(ItemMaker<IItem> output, params string[] requirements)
        {
            _output = output;
            _requirements = requirements;
        }

        public IItem TryGetResult(Backpack backpack)
        {
            return _requirements.Any(name => !backpack.HasItemOfName(name)) ? null : _output();
        }

        public IItem ConsumeAndCraft(Backpack backpack)
        {
            foreach (var name in _requirements)
            {
                backpack.RemoveItemByName(name);
            }

            return _output();
        }
    }
}