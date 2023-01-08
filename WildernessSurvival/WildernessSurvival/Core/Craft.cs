using System;
using System.Collections.Generic;
using System.Linq;

namespace WildernessSurvival.Core
{
    public interface IRecipe
    {
        /// <summary>
        /// Return an Item for display purpose
        /// </summary>
        IItem TryBuildPreview(Backpack backpack);

        IItem ConsumeAndCraft(Backpack backpack);
        void BuildCraftAttrRequirements(AttrModifierBuilder builder);
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

        public static IList<(IRecipe recipe, IItem preview)> TestAvailableRecipes(Backpack backpack)
        {
            return (from recipe in Recipes
                let output = recipe.TryBuildPreview(backpack)
                where output != null
                select (recipe, output)).ToList();
        }
    }

    public class NamedRecipe : IRecipe
    {
        public Func<IList<IItem>, IItem> Output;
        public Func<IItem> Preview;
        private readonly Dictionary<string, int> _requirements = new Dictionary<string, int>();
        public AttrModifier[] Modifiers { get; set; } = Array.Empty<AttrModifier>();

        public void BuildCraftAttrRequirements(AttrModifierBuilder builder) => builder.Add(Modifiers);

        public NamedRecipe(params string[] reqs)
        {
            foreach (var req in reqs)
            {
                if (_requirements.TryGetValue(req, out var number))
                    _requirements[req] = number + 1;
                else
                    _requirements[req] = 1;
            }
        }

        public IItem TryBuildPreview(Backpack backpack)
        {
            return _requirements.Any(p => backpack.CountItemOfName(p.Key) < p.Value) ? null : Preview();
        }

        public IItem ConsumeAndCraft(Backpack backpack)
        {
            var inputs = new List<IItem>();
            foreach (var p in _requirements)
            {
                for (var i = 0; i < p.Value; i++)
                {
                    var input = backpack.PopItemByName(p.Key);
                    inputs.Add(input);
                }
            }

            return Output(inputs);
        }
    }
}