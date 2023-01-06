using System.Collections.Generic;
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
        public ToolLevel Level { get; }
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
        public void Use(Player player);
        public UseType UseType { get; }
    }

    public abstract class UsableItem : IUsableItem
    {
        public abstract string Name { get; }
        public abstract void BuildUseEffect(UseEffectBuilder builder);
        public abstract UseType UseType { get; }

        public virtual void Use(Player player)
        {
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
        Boil
    }

    public interface IRawItem : IItem
    {
        CookType CookType { get; }
        IUsableItem Cook();
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
    }

    public class LogItem : IItem
    {
        public string Name => "Log";
        public static readonly LogItem One = new LogItem();

        private LogItem()
        {
        }
    }
}