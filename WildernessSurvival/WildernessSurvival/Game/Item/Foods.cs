using WildernessSurvival.Core;

// ReSharper disable CheckNamespace
namespace WildernessSurvival.Game
{
    public class EnergyBar : UsableItem
    {
        public const float DefaultFoodRestore = 0.32f;
        public float FoodRestore = DefaultFoodRestore;
        public const float DefaultEnergyRestore = 0.1f;
        public float EnergyRestore = DefaultEnergyRestore;
        public override string Name => nameof(EnergyBar);
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
            builder.Add(AttrType.Energy.WithEffect(EnergyRestore));
        }
    }


    public class BottledWater : UsableItem
    {
        public const float DefaultRestore = 0.28f;
        public float Restore = DefaultRestore;
        public override string Name => nameof(BottledWater);
        public override UseType UseType => UseType.Drink;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(Restore));
        }
    }

    public class RawRabbit : UsableItem, ICookableItem
    {
        public float FlueCost => 18;
        public const float DefaultFoodRestore = 0.45f;
        public const float DefaultWaterRestore = 0.05f;
        public float FoodRestore = DefaultFoodRestore;
        public float WaterRestore = DefaultWaterRestore;
        public override string Name => nameof(RawRabbit);

        public CookType CookType => CookType.Cook;
        public override UseType UseType => UseType.Eat;

        public IItem Cook() => new CookedRabbit
        {
            // add bounce from raw food
            FoodRestore = RoastedBerry.DefaultFoodRestore + FoodRestore * 0.15f,
        };

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
            builder.Add(AttrType.Food.WithEffect(WaterRestore));
        }
    }

    public class CookedRabbit : UsableItem
    {
        public const float DefaultFoodRestore = 0.5f;
        public float FoodRestore = DefaultFoodRestore;
        public override string Name => nameof(CookedRabbit);
        public override UseType UseType => UseType.Eat;


        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
        }
    }


    public class Berry : UsableItem, ICookableItem
    {
        public float FlueCost => 3;
        public const float DefaultFoodRestore = 0.12f;
        public const float DefaultWaterRestore = 0.08f;
        public float FoodRestore = DefaultFoodRestore;
        public float WaterRestore = DefaultWaterRestore;
        public override string Name => nameof(Berry);
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
        }

        public CookType CookType => CookType.Roast;

        public IItem Cook() => new RoastedBerry
        {
            // add bounce from raw food
            FoodRestore = RoastedBerry.DefaultFoodRestore + FoodRestore * 0.2f,
        };
    }

    public class RoastedBerry : UsableItem
    {
        public const float DefaultFoodRestore = 0.14f;
        public float FoodRestore = DefaultFoodRestore;
        public override string Name => nameof(RoastedBerry);
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
        }
    }

    public class DirtyWater : UsableItem, ICookableItem
    {
        public float FlueCost => 3;
        public const float DefaultWaterRestore = 0.1f;
        public float WaterRestore = DefaultWaterRestore;
        public const float DefaultHealthDelta = -0.08f;
        public float HealthDelta = DefaultHealthDelta;
        public override string Name => nameof(DirtyWater);
        public CookType CookType => CookType.Boil;
        public override UseType UseType => UseType.Drink;

        public IItem Cook() => new CleanWater
        {
            Restore = CleanWater.DefaultRestore + WaterRestore * 0.1f,
        };


        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
            builder.Add(AttrType.Health.WithEffect(HealthDelta));
        }
    }

    public class CleanWater : UsableItem
    {
        public const float DefaultRestore = 0.3f;
        public float Restore = DefaultRestore;
        public override string Name => nameof(CleanWater);
        public override UseType UseType => UseType.Drink;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(Restore));
        }
    }

    public class Nuts : UsableItem, ICookableItem
    {
        public float FlueCost => 3;
        public const float DefaultRestore = 0.08f;
        public float Restore = DefaultRestore;
        public override string Name => nameof(Nuts);
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }

        public CookType CookType => CookType.Roast;

        public IItem Cook() => new ToastedNuts
        {
            Restore = ToastedNuts.DefaultRestore + Restore * 0.3f,
        };
    }

    public class ToastedNuts : UsableItem
    {
        public const float DefaultRestore = 0.1f;
        public float Restore = DefaultRestore;
        public override string Name => nameof(ToastedNuts);

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }

        public override UseType UseType => UseType.Eat;
    }

    public class EnergyDrink : UsableItem
    {
        public float WaterRestore = 0.3f;
        public float EnergyRestore = 0.2f;
        public override string Name => nameof(EnergyDrink);
        public override UseType UseType => UseType.Drink;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
            builder.Add(AttrType.Energy.WithEffect(EnergyRestore));
        }
    }

    public class RawFish : UsableItem, ICookableItem
    {
        public float FlueCost => 12;
        public const float DefaultFoodRestore = 0.35f;
        public float FoodRestore = DefaultFoodRestore;
        public const float DefaultWaterRestore = 0.1f;
        public float WaterRestore = DefaultWaterRestore;
        public override string Name => nameof(RawFish);
        public CookType CookType => CookType.Cook;
        public override UseType UseType => UseType.Eat;

        public IItem Cook() => new CookedFish
        {
            Restore = CookedFish.DefaultRestore + FoodRestore * 0.2f,
        };

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(FoodRestore));
            builder.Add(AttrType.Water.WithEffect(WaterRestore));
        }
    }

    public class CookedFish : UsableItem
    {
        public const float DefaultRestore = 0.35f;
        public float Restore = DefaultRestore;
        public override string Name => nameof(CookedFish);
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(Restore));
        }
    }
}