using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public class EnergyBar : IUsableItem
    {
        private const int Restore = 3;
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
        private const int Restore = 4;
        public string Name => nameof(BottledWater);
        public UseType UseType => UseType.Drink;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class RawRabbit : IUsableItem, IRawItem
    {
        private const int Restore = 5;
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
        private const int Restore = 10;
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
        private const int FoodRestore = 2;
        private const int WaterRestore = 1;
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
        private const int Restore = 1;
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
        private const int Restore = 3;
        public string Name => nameof(CleanWater);
        public UseType UseType => UseType.Drink;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class Nuts : IUsableItem
    {
        private const int Restore = 2;
        public string Name => nameof(Nuts);
        public UseType UseType => UseType.Eat;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class Bandage : IMedicalSupplyItem
    {
        private const int Restore = 3;
        public string Name => nameof(Bandage);
        public UseType UseType => UseType.Use;

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Hp);
        }
    }

    public class FistAidKit : IMedicalSupplyItem
    {
        private const int HpRestore = 3;
        private const int EnergyRestore = 2;
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
        private const int WaterRestore = 3;
        private const int EnergyRestore = 4;
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
        private const int Restore = 6;
        public string Name => nameof(RawFish);
        public CookType CookType => CookType.Cook;
        public UseType UseType => UseType.Eat;

        public IUsableItem Cook()
        {
            return new CookedFish();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class CookedFish : IUsableItem
    {
        private const int Restore = 9;
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