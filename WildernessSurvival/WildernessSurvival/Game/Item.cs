namespace WildernessSurvival.Game
{
    public interface IItem
    {
        string Name { get; }

        string Description { get; }
    }

    public interface IToolItem : IItem
    {
    }

    public interface IUsableItem : IItem
    {
        public abstract void Use(Player player);
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
        public static readonly LogItem One = new LogItem();

        private LogItem()
        {
            
        }
        public string Description => "一根没什么特点的木头。";
        public string Name => "木头";
    }

    public interface IMedicalSupplyItem : IUsableItem
    {
    }
}