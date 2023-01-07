using WildernessSurvival.Core;

// ReSharper disable CheckNamespace
namespace WildernessSurvival.Game
{
    public class Bandage : UsableItem
    {
        public float Restore = 0.3f;
        public override string Name => nameof(Bandage);
        public override UseType UseType => UseType.Use;

        public override void BuildAttrModification(AttrModifierBuilder builder)
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

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(HpRestore));
            builder.Add(AttrType.Energy.WithEffect(EnergyRestore));
        }
    }

    public class Sticks : IFuelItem
    {
        public const float DefaultFuel = 1;
        public float Fuel { get; set; } = DefaultFuel;
        public string Name => nameof(Sticks);
    }


    public class CutGrass : IFuelItem
    {
        public const float DefaultFuel = 1;
        public float Fuel { get; set; } = DefaultFuel;
        public string Name => nameof(CutGrass);
    }


    public class Log : IFuelItem
    {
        public const float DefaultFuel = 15;
        public float Fuel { get; set; } = DefaultFuel;
        public string Name => nameof(Log);
    }
}