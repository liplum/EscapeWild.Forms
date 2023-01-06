using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public class EnergyBar : IEdibleItem
    {
        private const int Restore = 3;
        public string Name => nameof(EnergyBar);

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

    public class BottledWater : IEdibleItem
    {
        private const int Restore = 4;
        public string Name => nameof(BottledWater);

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class RawRabbit : IEdibleItem, IRawItem
    {
        private const int Restore = 5;
        public string Name => nameof(RawRabbit);
        public string RawDescription => $"烹饪后：{nameof(CookedRabbit)}";

        public IUsableItem Cook()
        {
            return new CookedRabbit();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class CookedRabbit : IEdibleItem
    {
        private const int Restore = 10;
        public string Name => nameof(CookedRabbit);

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

    public class Berry : IEdibleItem
    {
        private const int FoodRestore = 2;
        private const int WaterRestore = 1;
        public string Name => nameof(Berry);

        public void Use(Player player)
        {
            player.Modify(FoodRestore, AttrType.Food);
            player.Modify(WaterRestore, AttrType.Water);
        }
    }

    public class DirtyWater : IEdibleItem, IRawItem
    {
        private const int Restore = 1;
        public string Name => nameof(DirtyWater);
        public string RawDescription => $"煮开后：{nameof(CleanWater)}";

        public IUsableItem Cook()
        {
            return new CleanWater();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class CleanWater : IEdibleItem
    {
        private const int Restore = 3;
        public string Name => nameof(CleanWater);

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class Nuts : IEdibleItem
    {
        private const int Restore = 2;
        public string Name => nameof(Nuts);

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class Bandage : IMedicalSupplyItem
    {
        private const int Restore = 3;
        public string Name => nameof(Bandage);

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

        public void Use(Player player)
        {
            player.Modify(HpRestore, AttrType.Hp);
            player.Modify(EnergyRestore, AttrType.Energy);
        }
    }

    public class EnergyDrink : IEdibleItem
    {
        private const int WaterRestore = 3;
        private const int EnergyRestore = 4;
        public string Name => nameof(EnergyDrink);

        public void Use(Player player)
        {
            player.Modify(WaterRestore, AttrType.Water);
            player.Modify(EnergyRestore, AttrType.Energy);
        }
    }

    public class RawFish : IEdibleItem, IRawItem
    {
        private const int Restore = 6;
        public string Name => nameof(RawFish);
        public string RawDescription => $"烹饪后：{nameof(CookedFish)}";

        public IUsableItem Cook()
        {
            return new CookedFish();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class CookedFish : IEdibleItem
    {
        private const int Restore = 9;
        public string Name => nameof(CookedFish);

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