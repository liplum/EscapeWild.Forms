using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public class EnergyBar : UsableItem
    {
        private const float Restore = 0.3f;
        public override string Name => nameof(EnergyBar);
        public override UseType UseType => UseType.Eat;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }
    }

    public class OldOxe : IOxeItem
    {
        public string Name => nameof(OldOxe);
        public ToolLevel Level => ToolLevel.Normal;
    }

    public class BottledWater : UsableItem
    {
        private const float Restore = 0.4f;
        public override string Name => nameof(BottledWater);
        public override UseType UseType => UseType.Drink;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(Restore));
        }
    }

    public class RawRabbit : UsableItem, IRawItem
    {
        private const float Restore = 0.5f;
        public override string Name => nameof(RawRabbit);

        public CookType CookType => CookType.Cook;
        public override UseType UseType => UseType.Eat;

        public IUsableItem Cook()
        {
            return new CookedRabbit();
        }

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }
    }

    public class CookedRabbit : UsableItem
    {
        private const float Restore = 0.9f;
        public override string Name => nameof(CookedRabbit);
        public override UseType UseType => UseType.Eat;


        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }
    }

    public class OldFishRod : IFishToolItem
    {
        public string Name => nameof(OldFishRod);
        public ToolLevel Level => ToolLevel.Normal;
    }

    public class Berry : UsableItem
    {
        private const float FoodRestore = 0.2f;
        private const float WaterRestore = 0.1f;
        public override string Name => nameof(Berry);
        public override UseType UseType => UseType.Eat;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
        }
    }

    public class DirtyWater : UsableItem, IRawItem
    {
        private const float Restore = 0.1f;
        public override string Name => nameof(DirtyWater);
        public CookType CookType => CookType.Boil;
        public override UseType UseType => UseType.Drink;

        public IUsableItem Cook()
        {
            return new CleanWater();
        }

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(Restore));
        }
    }

    public class CleanWater : UsableItem
    {
        private const float Restore = 0.3f;
        public override string Name => nameof(CleanWater);
        public override UseType UseType => UseType.Drink;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(Restore));
        }
    }

    public class Nuts : UsableItem
    {
        private const float Restore = 0.2f;
        public override string Name => nameof(Nuts);
        public override UseType UseType => UseType.Eat;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }
    }

    public class Bandage : UsableItem
    {
        private const float Restore = 0.3f;
        public override string Name => nameof(Bandage);
        public override UseType UseType => UseType.Use;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(Restore));
        }
    }

    public class FistAidKit : UsableItem
    {
        private const float HpRestore = 0.3f;
        private const float EnergyRestore = 0.2f;
        public override string Name => nameof(FistAidKit);
        public override UseType UseType => UseType.Use;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(HpRestore));
            builder.Add(AttrType.Energy.WithEffect(EnergyRestore));
        }
    }

    public class EnergyDrink : UsableItem
    {
        private const float WaterRestore = 0.3f;
        private const float EnergyRestore = 0.4f;
        public override string Name => nameof(EnergyDrink);
        public override UseType UseType => UseType.Drink;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
            builder.Add(AttrType.Energy.WithEffect(EnergyRestore));
        }
    }

    public class RawFish : UsableItem, IRawItem
    {
        private const float FoodRestore = 0.4f;
        private const float WaterRestore = 0.2f;
        public override string Name => nameof(RawFish);
        public CookType CookType => CookType.Cook;
        public override UseType UseType => UseType.Eat;

        public IUsableItem Cook()
        {
            return new CookedFish();
        }

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
        }
    }

    public class CookedFish : UsableItem
    {
        private const float Restore = 0.6f;
        public override string Name => nameof(CookedFish);
        public override UseType UseType => UseType.Eat;

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }
    }

    public class OldShotgun : IHuntingToolItem
    {
        public ToolLevel Level => ToolLevel.High;
        public string Name => nameof(OldShotgun);
    }

    public class Trap : IHuntingToolItem
    {
        public ToolLevel Level => ToolLevel.Normal;
        public string Name => nameof(Trap);
    }
}