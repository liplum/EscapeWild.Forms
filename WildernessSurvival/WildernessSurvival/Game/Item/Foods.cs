using System;
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

    public class UnknownMushrooms : UsableItem, ICookableItem
    {
        public const float DefaultHpDelta = 0.45f;
        public float HpDelta = DefaultHpDelta;
        public const float DefaultFoodDelta = 0.15f;
        public float FoodDelta = DefaultFoodDelta;
        public const float DefaultEnergyDelta = 0.15f;
        public float EnergyDelta = DefaultEnergyDelta;
        public float FlueCost { get; set; } = 2f;

        public static UnknownMushrooms Poisonous(float ratio) => new UnknownMushrooms
        {
            HpDelta = -DefaultHpDelta * ratio,
            FoodDelta = -DefaultFoodDelta * ratio,
            EnergyDelta = -DefaultEnergyDelta * ratio,
            FlueCost = 2f * (1f + ratio),
        };

        public static UnknownMushrooms Safe(float ratio) => new UnknownMushrooms
        {
            FoodDelta = DefaultFoodDelta * ratio,
            EnergyDelta = DefaultEnergyDelta * ratio,
            FlueCost = 2f * (1f + ratio),
        };

        public static UnknownMushrooms Random() => new UnknownMushrooms
        {
            HpDelta = Rand.Float(-0.4f, 0.1f),
            FoodDelta = Rand.Float(-0.2f, 0.15f),
            EnergyDelta = Rand.Float(-0.15f, 0.05f),
            FlueCost = Rand.Float(2f, 4f),
        };

        public override bool DisplayPreview => false;
        public override string Name => nameof(UnknownMushrooms);
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(HpDelta));
        }

        public CookType CookType => CookType.Roast;

        public IItem Cook() => new GrilledUnknownMushrooms
        {
            HpDelta = Math.Abs(HpDelta),
            FoodDelta = Math.Abs(FoodDelta),
            EnergyDelta = Math.Abs(EnergyDelta),
        };
    }

    public class GrilledUnknownMushrooms : UsableItem
    {
        public float HpDelta;
        public float FoodDelta;
        public float EnergyDelta;

        public override string Name => nameof(GrilledUnknownMushrooms);

        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Health.WithEffect(HpDelta));
            builder.Add(AttrType.Food.WithEffect(FoodDelta));
            builder.Add(AttrType.Energy.WithEffect(EnergyDelta));
        }
    }

    public class CookableItem : UsableItem, ICookableItem
    {
        public CookableItem(string name)
        {
            Name = name;
        }

        public ItemMaker<IItem> Cooked { get; set; }
        public AttrModifier[] Modifiers { get; set; } = Array.Empty<AttrModifier>();
        public override string Name { get; }
        public override UseType UseType => UseType.Eat;

        public float FlueCost { get; set; }

        public override void BuildAttrModification(AttrModifierBuilder builder) => builder.Add(Modifiers);

        public CookType CookType => CookType.Roast;
        public IItem Cook() => Cooked();
    }

    public class CookedItem : UsableItem
    {
        public CookedItem(string name)
        {
            Name = name;
        }

        public AttrModifier[] Modifiers { get; set; } = Array.Empty<AttrModifier>();
        public override string Name { get; }
        public override UseType UseType => UseType.Eat;

        public override void BuildAttrModification(AttrModifierBuilder builder) => builder.Add(Modifiers);
    }
}