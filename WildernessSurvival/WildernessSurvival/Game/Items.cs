using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public class EnergyBar : IUsableItem
    {
        private const float Restore = 0.3f;
        public string Name => nameof(EnergyBar);
        public UseType UseType => UseType.Eat;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class OldOxe : IOxeItem
    {
        public string Name => nameof(OldOxe);
        public ToolLevel Level => ToolLevel.Normal;
    }

    public class BottledWater : IUsableItem
    {
        private const float Restore = 0.4f;
        public string Name => nameof(BottledWater);
        public UseType UseType => UseType.Drink;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class RawRabbit : IUsableItem, IRawItem
    {
        private const float Restore = 0.5f;
        public string Name => nameof(RawRabbit);

        public CookType CookType => CookType.Cook;
        public UseType UseType => UseType.Eat;

        public IUsableItem Cook()
        {
            return new CookedRabbit();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class CookedRabbit : IUsableItem
    {
        private const float Restore = 0.9f;
        public string Name => nameof(CookedRabbit);
        public UseType UseType => UseType.Eat;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class OldFishRod : IFishToolItem
    {
        public string Name => nameof(OldFishRod);
        public ToolLevel Level => ToolLevel.Normal;
    }

    public class Berry : IUsableItem
    {
        private const float FoodRestore = 0.2f;
        private const float WaterRestore = 0.1f;
        public string Name => nameof(Berry);
        public UseType UseType => UseType.Eat;

        public void Use(Player player)
        {
            player.Modify(FoodRestore, AttrType.Food);
            player.Modify(WaterRestore, AttrType.Water);
        }
    }

    public class DirtyWater : IUsableItem, IRawItem
    {
        private const float Restore = 0.1f;
        public string Name => nameof(DirtyWater);
        public CookType CookType => CookType.Boil;
        public UseType UseType => UseType.Drink;

        public IUsableItem Cook()
        {
            return new CleanWater();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class CleanWater : IUsableItem
    {
        private const float Restore = 0.3f;
        public string Name => nameof(CleanWater);
        public UseType UseType => UseType.Drink;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class Nuts : IUsableItem
    {
        private const float Restore = 0.2f;
        public string Name => nameof(Nuts);
        public UseType UseType => UseType.Eat;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class Bandage : IMedicalSupplyItem
    {
        private const float Restore = 0.3f;
        public string Name => nameof(Bandage);
        public UseType UseType => UseType.Use;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Hp);
        }
    }

    public class FistAidKit : IMedicalSupplyItem
    {
        private const float HpRestore = 0.3f;
        private const float EnergyRestore = 0.2f;
        public string Name => nameof(FistAidKit);
        public UseType UseType => UseType.Use;

        public void Use(Player player)
        {
            player.Modify(HpRestore, AttrType.Hp);
            player.Modify(EnergyRestore, AttrType.Energy);
        }
    }

    public class EnergyDrink : IUsableItem
    {
        private const float WaterRestore = 0.3f;
        private const float EnergyRestore = 0.4f;
        public string Name => nameof(EnergyDrink);
        public UseType UseType => UseType.Drink;

        public void Use(Player player)
        {
            player.Modify(WaterRestore, AttrType.Water);
            player.Modify(EnergyRestore, AttrType.Energy);
        }
    }

    public class RawFish : IUsableItem, IRawItem
    {
        private const float FoodRestore = 0.4f;
        private const float WaterRestore = 0.2f;
        public string Name => nameof(RawFish);
        public CookType CookType => CookType.Cook;
        public UseType UseType => UseType.Eat;

        public IUsableItem Cook()
        {
            return new CookedFish();
        }

        public void Use(Player player)
        {
            player.Modify(FoodRestore, AttrType.Food);
            player.Modify(WaterRestore, AttrType.Food);
        }
    }

    public class CookedFish : IUsableItem
    {
        private const float Restore = 0.6f;
        public string Name => nameof(CookedFish);
        public UseType UseType => UseType.Eat;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
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