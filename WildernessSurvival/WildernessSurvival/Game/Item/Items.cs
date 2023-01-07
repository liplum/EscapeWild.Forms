using WildernessSurvival.Core;

// ReSharper disable CheckNamespace
namespace WildernessSurvival.Game
{
    public class Bandage : UsableItem
    {
        public float Restore = 0.3f;
        public override string Name => nameof(Bandage);
        public override UseType UseType => UseType.Use;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(Restore));
        }
    }

    public class FistAidKit : UsableItem
    {
        public float HpRestore = 0.3f;
        public float EnergyRestore = 0.2f;
        public override string Name => nameof(FistAidKit);
        public override UseType UseType => UseType.Use;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(HpRestore));
            builder.Add(AttrType.Energy.WithEffect(EnergyRestore));
        }
    }


    public class Stick : IItem
    {
        public string Name => nameof(Stick);
        public static Stick One = new Stick();

        private Stick()
        {
        }
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