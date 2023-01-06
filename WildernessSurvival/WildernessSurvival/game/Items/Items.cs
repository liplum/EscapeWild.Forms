namespace WildernessSurvival.game.Items
{
    public class 能量棒 : IEdibleItem
    {
        private const int Restore = 3;
        public string Description => $"嗯，来劲了。可以回复{Restore}点饱腹值";
        public string Name => $"{nameof(能量棒)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class 老旧的斧头 : IOxeItem
    {
        public string Description => "一把老旧的斧头，但是木柄还是很结实。";
        public string Name => $"{nameof(老旧的斧头)}";
    }

    public class 瓶装水 : IEdibleItem
    {
        private const int Restore = 4;
        public string Description => $"一瓶矿泉水，不知道存放了多久。可以回复{Restore}点饮水值";
        public string Name => $"{nameof(瓶装水)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class 生兔肉 : IEdibleItem, IRawItem
    {
        private const int Restore = 5;
        public string Description => $"新鲜的生兔肉，你觉得应该烧熟了再吃。可以回复{Restore}点饱腹值";
        public string Name => $"{nameof(生兔肉)}";
        public string RawDescription => $"烹饪后：{nameof(熟兔肉)}";

        public IUsableItem Cook()
        {
            return new 熟兔肉();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class 熟兔肉 : IEdibleItem
    {
        private const int Restore = 10;
        public string Description => $"兔兔辣么可爱，你为什么要吃兔兔。可以回复{Restore}点饱腹值";
        public string Name => $"{nameof(熟兔肉)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class 老旧的钓鱼竿 : IFishToolItem
    {
        public string Description => "一根老旧的钓鱼竿，应该还可以钓鱼。";
        public string Name => $"{nameof(老旧的钓鱼竿)}";
    }

    public class 浆果 : IEdibleItem
    {
        private const int FoodRestore = 2;
        private const int WaterRestore = 1;
        public string Description => $"在别的游戏里，我可是主角。可以回复{FoodRestore}点饱腹值，{WaterRestore}点饮水值。";
        public string Name => $"{nameof(浆果)}";

        public void Use(Player player)
        {
            player.Modify(FoodRestore, AttrType.Food);
            player.Modify(WaterRestore, AttrType.Water);
        }
    }

    public class 脏水 : IEdibleItem, IRawItem
    {
        private const int Restore = 1;
        public string Description => $"收集到的脏水，你觉得应该烧开了再喝。可以回复{Restore}点饮水值";
        public string Name => $"{nameof(脏水)}";
        public string RawDescription => $"煮开后：{nameof(净水)}";

        public IUsableItem Cook()
        {
            return new 净水();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class 净水 : IEdibleItem
    {
        private const int Restore = 3;
        public string Description => $"干净的水，正好解渴。可以回复{Restore}点饮水值";
        public string Name => $"{nameof(净水)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Water);
        }
    }

    public class 坚果 : IEdibleItem
    {
        private const int Restore = 2;
        public string Description => $"芜香蛋。可以回复{Restore}点饱腹值";
        public string Name => $"{nameof(坚果)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class 绷带 : IMedicalSupplyItem
    {
        private const int Restore = 3;
        public string Description => $"用来包扎伤口的医疗用品。可以回复{Restore}点生命值";
        public string Name => $"{nameof(绷带)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Hp);
        }
    }

    public class 急救包 : IMedicalSupplyItem
    {
        private const int HpRestore = 3;
        private const int EnergyRestore = 2;
        public string Description => $"等我打个包。可以回复{HpRestore}点生命值，{EnergyRestore}点能量值";
        public string Name => $"{nameof(急救包)}";

        public void Use(Player player)
        {
            player.Modify(HpRestore, AttrType.Hp);
            player.Modify(EnergyRestore, AttrType.Energy);
        }
    }

    public class 能量饮料 : IEdibleItem
    {
        private const int WaterRestore = 3;
        private const int EnergyRestore = 4;
        public string Description => $"让你随时麦动回来。可以回复{WaterRestore}点饮水值,{EnergyRestore}点能量值";
        public string Name => $"{nameof(能量饮料)}";

        public void Use(Player player)
        {
            player.Modify(WaterRestore, AttrType.Water);
            player.Modify(EnergyRestore, AttrType.Energy);
        }
    }

    public class 生鱼 : IEdibleItem, IRawItem
    {
        private const int Restore = 6;
        public string Description => $"红鲤鱼与绿鲤鱼与驴。可以回复{Restore}点饱腹值";
        public string Name => $"{nameof(生鱼)}";
        public string RawDescription => $"烹饪后：{nameof(熟鱼)}";

        public IUsableItem Cook()
        {
            return new 熟鱼();
        }

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class 熟鱼 : IEdibleItem
    {
        private const int Restore = 9;
        public string Description => $"授人以鱼不如授人以渔。可以回复{Restore}点饱腹值";
        public string Name => $"{nameof(熟鱼)}";

        public void Use(Player player)
        {
            player.Modify(Restore, AttrType.Food);
        }
    }

    public class 老旧的猎枪 : IHuntingToolItem
    {
        public string Description => "还是双管的。";
        public ToolLevel HuntingToolLevel => ToolLevel.High;
        public string Name => $"{nameof(老旧的猎枪)}";
    }

    public class 捕兽陷阱 : IHuntingToolItem
    {
        public string Description => "应该没有傻子会自己踩到它吧。";
        public ToolLevel HuntingToolLevel => ToolLevel.Low;
        public string Name => $"{nameof(捕兽陷阱)}";
    }
}