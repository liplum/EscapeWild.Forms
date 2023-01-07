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
        private readonly Dictionary<string, int> _requirements = new Dictionary<string, int>();

        public NamedRecipe(ItemMaker<IItem> output, params string[] reqs)
        {
            _output = output;
            foreach (var req in reqs)
            {
                if (_requirements.TryGetValue(req, out var number))
                    _requirements[req] = number + 1;
                else
                    _requirements[req] = 1;
            }
        }

        public IItem TryGetResult(Backpack backpack)
        {
            return _requirements.Any(p => backpack.CountItemOfName(p.Key) < p.Value) ? null : _output();
        }

        public IItem ConsumeAndCraft(Backpack backpack)
        {
            foreach (var p in _requirements)
            {
                for (var i = 0; i < p.Value; i++)
                {
                    backpack.RemoveItemByName(p.Key);
                }
            }

            return _output();
        }
    }
}