using WildernessSurvival.Localization;

namespace WildernessSurvival.Core
{
    public interface IItem
    {
        string Name { get; }
    }

    public static class ItemI18N
    {
        public static string LocalizedName(this IItem item) => I18N.Get($"Item.{item.Name}.Name");
        public static string LocalizedDesc(this IItem item) => I18N.Get($"Item.{item.Name}.Desc");
    }

    public interface IToolItem : IItem
    {
    }

    public interface IUsableItem : IItem
    {
        public abstract void Use(Core.Player player);
    }

    public interface IRawItem
    {
        string RawDescription { get; }

        IUsableItem Cook();
    }

    public interface IEdibleItem : IUsableItem
    {
    }

    public interface IOxeItem : IToolItem
    {
    }

    public enum ToolLevel
    {
        Low,
        Normal,
        High,
        Max
    }

    public interface IFishToolItem : IToolItem
    {
    }

    public interface IHuntingToolItem : IToolItem
    {
        public ToolLevel HuntingToolLevel { get; }
    }

    public class LogItem : IItem
    {
        public string Name => "Log";
        public static readonly LogItem One = new LogItem();

        private LogItem()
        {
        }
    }

    public interface IMedicalSupplyItem : IUsableItem
    {
    }
}