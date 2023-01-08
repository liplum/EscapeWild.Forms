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
            Cutting = new ToolType("Cutting"),
            Oxe = new ToolType("Oxe"),
            Hunting = new ToolType("Hunting"),
            Fishing = new ToolType("Fishing");
    }

    public interface IToolItem : IItem
    {
        ToolLevel Level { get; }
        ToolType ToolType { get; }
        float Durability { get; set; }
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

    public interface IUsableItem : IItem
    {
        void BuildAttrModification(AttrModifierBuilder builder);
        bool CanUse(Player player);
        Task Use(Player player);
        IItem AfterUsed();
        UseType UseType { get; }
        bool DisplayPreview { get; }
    }

    public abstract class UsableItem : IUsableItem
    {
        public abstract string Name { get; }
        public abstract void BuildAttrModification(AttrModifierBuilder builder);
        public virtual bool CanUse(Player player) => true;

        public virtual IItem AfterUsed() => null;

        public abstract UseType UseType { get; }
        public virtual bool DisplayPreview => true;

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