using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Localization;

namespace WildernessSurvival.Core
{
    public delegate T ItemMaker<out T>() where T : IItem;

    public interface IItem
    {
        string Name { get; }
    }

    public static class ItemI18N
    {
        public static string LocalizedName(this IItem item) => I18N.Get($"Item.{item.Name}.Name");
        public static string LocalizedDesc(this IItem item) => I18N.Get($"Item.{item.Name}.Desc");
        public static string LocalizedName(this ToolType type) => I18N.Get($"ToolType.{type.Name}");
    }

    public enum ToolLevel
    {
        Low,
        Normal,
        High,
        Max
    }

    public class ToolType
    {
        public string Name;

        private ToolType(string name)
        {
            Name = name;
        }

        public static readonly ToolType
            Oxe = new ToolType("Oxe"),
            Hunting = new ToolType("Hunting"),
            Fishing = new ToolType("Fishing");
    }

    public interface IToolItem : IItem
    {
        public ToolLevel Level { get; }
        public ToolType ToolType { get; }
        public float Durability { get; set; }
    }

    public class ToolItem : IToolItem
    {
        public float Durability { get; set; }
        public string Name { get; set; }
        public ToolLevel Level { get; set; }
        public ToolType ToolType { get; set; }
    }

    public enum UseType
    {
        Use,
        Drink,
        Eat
    }

    public class AttrModifier
    {
        public readonly AttrType AttrType;
        public readonly float Delta;

        public AttrModifier(AttrType attr, float delta)
        {
            AttrType = attr;
            Delta = delta;
        }
    }

    public static class AttrModifierHelper
    {
        public static AttrModifier WithEffect(this AttrType attr, float delta) => new AttrModifier(attr, delta);
    }

    public class AttrModifierBuilder
    {
        public readonly List<AttrModifier> Effects = new List<AttrModifier>();

        public void Add(AttrModifier effect)
        {
            Effects.Add(effect);
        }

        public void PerformModification(AttributeManager attrs)
        {
            foreach (var effect in Effects)
            {
                attrs.Modify(effect.AttrType, effect.Delta);
            }
        }

        public bool HasAnyEffect => Effects.Count > 0;
    }

    public interface IUsableItem : IItem
    {
        public void BuildAttrModification(AttrModifierBuilder builder);
        public bool CanUse(Player player);
        public Task Use(Player player);
        public UseType UseType { get; }
    }

    public abstract class UsableItem : IUsableItem
    {
        public abstract string Name { get; }
        public abstract void BuildAttrModification(AttrModifierBuilder builder);
        public virtual bool CanUse(Player player) => true;

        public abstract UseType UseType { get; }

        public virtual async Task Use(Player player)
        {
            var builder = new AttrModifierBuilder();
            BuildAttrModification(builder);
            builder.PerformModification(player.Attrs);
        }
    }

    public enum CookType
    {
        Cook,
        Boil,
        Roast,
    }

    public interface ICookableItem : IItem
    {
        CookType CookType { get; }

        /// <summary>
        /// Call this only once.
        /// </summary>
        IItem Cook();

        float FlueCost { get; }
    }

    public interface IFuelItem : IItem
    {
        float Fuel { get; }
    }
}