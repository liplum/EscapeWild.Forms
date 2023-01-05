namespace WildernessSurvival.game
{
    public abstract class ItemBase
    {
        protected abstract string Name { get; }

        public abstract string Description { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract class Tool : ItemBase
    {
    }

    public abstract class AvailableItem : ItemBase
    {
        public abstract void Use(Player player);
    }

    public interface IRawItem
    {
        string RawDescription { get; }

        AvailableItem Cook();
    }

    public abstract class EdibleItem : AvailableItem
    {
    }

    public abstract class Oxe : Tool
    {
    }

    public abstract class FishRod : Tool
    {
    }

    public abstract class Hunting : Tool
    {
        public enum Level
        {
            LOW,
            NOMAL,
            HIGH,
            MAX
        }

        public abstract Level HuntingLevel { get; }
    }

    public abstract class Wood : ItemBase
    {
    }

    public abstract class MedicalSupplie : AvailableItem
    {
    }
}