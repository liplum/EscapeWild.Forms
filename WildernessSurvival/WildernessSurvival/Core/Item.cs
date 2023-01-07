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
        public int RemainingTimes { get; }
    }

    public class ToolItem : IToolItem
    {
        public ToolItem(int durability)
        {
            RemainingTimes = durability;
        }

        public int RemainingTimes { get; }
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

    public class UseEffect
    {
        public readonly AttrType AttrType;
        public readonly float Delta;

        public UseEffect(AttrType attr, float delta)
        {
            AttrType = attr;
            Delta = delta;
        }
    }

    public static class UseEffectHelper
    {
        public static UseEffect WithEffect(this AttrType attr, float delta) => new UseEffect(attr, delta);
    }

    public interface IAcceptUseEffect
    {
        public void Perform(AttrType attr, float delta);
    }

    public class UseEffectBuilder
    {
        public readonly List<UseEffect> Effects = new List<UseEffect>();

        public void Add(UseEffect effect)
        {
            Effects.Add(effect);
        }

        public void PerformUseEffects(IAcceptUseEffect on)
        {
            foreach (var effect in Effects)
            {
                on.Perform(effect.AttrType, effect.Delta);
            }
        }

        public bool HasAnyEffect => Effects.Count > 0;
    }

    public interface IUsableItem : IItem
    {
        public void BuildUseEffect(UseEffectBuilder builder);
        public bool CanUse(Player player);
        public Task Use(Player player);
        public UseType UseType { get; }
        public bool IsUsed { get; }
    }

    public abstract class UsableItem : IUsableItem
    {
        public abstract string Name { get; }
        public abstract void BuildUseEffect(UseEffectBuilder builder);
        public virtual bool CanUse(Player player) => true;

        public abstract UseType UseType { get; }
        public bool IsUsed { get; protected set; }

        public virtual async Task Use(Player player)
        {
            if (IsUsed) return;
            IsUsed = true;
            var builder = new UseEffectBuilder();
            BuildUseEffect(builder);
            foreach (var effect in builder.Effects)
            {
                player.Modify(effect.Delta, effect.AttrType);
            }
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
    }

    public interface IFuelItem : IItem
    {
        float Fuel { get; }
    }

    public class PlayerAcceptUseEffectWrapper : IAcceptUseEffect
    {
        private readonly Player _player;
        private readonly ValueFixer _fixer;

        public PlayerAcceptUseEffectWrapper(Player player, ValueFixer fixer = null)
        {
            _player = player;
            _fixer = fixer;
        }

        public void Perform(AttrType attr, float delta)
        {
            _player.Modify(delta, attr, _fixer);
        }
    }

    public class MockPlayerAcceptUseEffect : IAcceptUseEffect
    {
        public float Health;
        public float Food;
        public float Water;
        public float Energy;

        public void Perform(AttrType attr, float delta)
        {
            switch (attr)
            {
                case AttrType.Health:
                    Health += delta;
                    break;
                case AttrType.Food:
                    Food += delta;
                    break;
                case AttrType.Water:
                    Water += delta;
                    break;
                case AttrType.Energy:
                    Energy += delta;
                    break;
            }
        }
    }
}