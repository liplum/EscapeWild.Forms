namespace WildernessSurvival.game.Items {

    public class 能量棒 : EdibleItem {
        private const int Restore = 3;
        public override string Description => $"嗯，来劲了。可以回复{Restore}点饱腹值";
        protected override string Name => $"{nameof(能量棒)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.FOOD);
        }
    }

    public class 老旧的斧头 : Oxe {
        public override string Description => "一把老旧的斧头，但是木柄还是很结实。";
        protected override string Name => $"{nameof(老旧的斧头)}";
    }

    public class 瓶装水 : EdibleItem {
        private const int Restore = 4;
        public override string Description => $"一瓶矿泉水，不知道存放了多久。可以回复{Restore}点饮水值";
        protected override string Name => $"{nameof(瓶装水)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.WATER);
        }
    }

    public class 生兔肉 : EdibleItem, IRawItem {
        private const int Restore = 5;
        public override string Description => $"新鲜的生兔肉，你觉得应该烧熟了再吃。可以回复{Restore}点饱腹值";
        public string RawDescription => $"烹饪后：{nameof(熟兔肉)}";
        protected override string Name => $"{nameof(生兔肉)}";
        public AvailableItem Cook() {
            return new 熟兔肉();
        }

        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.FOOD);
        }
    }

    public class 熟兔肉 : EdibleItem {
        private const int Restore = 10;
        public override string Description => $"兔兔辣么可爱，你为什么要吃兔兔。可以回复{Restore}点饱腹值";
        protected override string Name => $"{nameof(熟兔肉)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.FOOD);
        }
    }

    public class 老旧的钓鱼竿 : FishRod {
        public override string Description => "一根老旧的钓鱼竿，应该还可以钓鱼。";
        protected override string Name => $"{nameof(老旧的钓鱼竿)}";
    }

    public class 木头 : Wood {
        public override string Description => "一根没什么特点的木头。";
        protected override string Name => "木头";
    }

    public class 浆果 : EdibleItem {
        private const int FoodRestore = 2;
        private const int WaterRestore = 1;
        public override string Description => $"在别的游戏里，我可是主角。可以回复{FoodRestore}点饱腹值，{WaterRestore}点饮水值。";
        protected override string Name => $"{nameof(浆果)}";
        public override void Use(Player player) {
            player.Modify(FoodRestore, Player.ValueType.FOOD);
            player.Modify(WaterRestore, Player.ValueType.WATER);
        }
    }

    public class 脏水 : EdibleItem, IRawItem {
        private const int Restore = 1;
        public override string Description => $"收集到的脏水，你觉得应该烧开了再喝。可以回复{Restore}点饮水值";
        protected override string Name => $"{nameof(脏水)}";
        public string RawDescription => $"煮开后：{nameof(净水)}";
        public AvailableItem Cook() {
            return new 净水();
        }

        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.WATER);
        }
    }

    public class 净水 : EdibleItem {
        private const int Restore = 3;
        public override string Description => $"干净的水，正好解渴。可以回复{Restore}点饮水值";
        protected override string Name => $"{nameof(净水)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.WATER);
        }
    }

    public class 坚果 : EdibleItem {
        private const int Restore = 2;
        public override string Description => $"芜香蛋。可以回复{Restore}点饱腹值";
        protected override string Name => $"{nameof(坚果)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.FOOD);
        }
    }

    public class 绷带 : MedicalSupplie {
        private const int Restore = 3;
        public override string Description => $"用来包扎伤口的医疗用品。可以回复{Restore}点生命值";
        protected override string Name => $"{nameof(绷带)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.HP);
        }
    }

    public class 急救包 : MedicalSupplie {
        private const int HPRestore = 3;
        private const int EnergyRestore = 2;
        public override string Description => $"等我打个包。可以回复{HPRestore}点生命值，{EnergyRestore}点能量值";
        protected override string Name => $"{nameof(急救包)}";
        public override void Use(Player player) {
            player.Modify(HPRestore, Player.ValueType.HP);
            player.Modify(EnergyRestore, Player.ValueType.ENERGY);
        }
    }

    public class 能量饮料 : EdibleItem {
        private const int WaterRestore = 3;
        private const int EnergyRestore = 4;
        public override string Description => $"让你随时麦动回来。可以回复{WaterRestore}点饮水值,{EnergyRestore}点能量值";
        protected override string Name => $"{nameof(能量饮料)}";
        public override void Use(Player player) {
            player.Modify(WaterRestore, Player.ValueType.WATER);
            player.Modify(EnergyRestore, Player.ValueType.ENERGY);
        }
    }

    public class 生鱼 : EdibleItem, IRawItem {
        private const int Restore = 6;
        public override string Description => $"红鲤鱼与绿鲤鱼与驴。可以回复{Restore}点饱腹值";
        public string RawDescription => $"烹饪后：{nameof(熟鱼)}";
        protected override string Name => $"{nameof(生鱼)}";
        public AvailableItem Cook() {
            return new 熟鱼();
        }

        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.FOOD);
        }
    }

    public class 熟鱼 : EdibleItem {
        private const int Restore = 9;
        public override string Description => $"授人以鱼不如授人以渔。可以回复{Restore}点饱腹值";
        protected override string Name => $"{nameof(熟鱼)}";
        public override void Use(Player player) {
            player.Modify(Restore, Player.ValueType.FOOD);
        }
    }

    public class 老旧的猎枪 : Hunting {
        public override string Description => "还是双管的。";
        public override Level HuntingLevel => Level.HIGH;
        protected override string Name => $"{nameof(老旧的猎枪)}";
    }

    public class 捕兽陷阱 : Hunting {
        public override string Description => "应该没有傻子会自己踩到它吧。";
        public override Level HuntingLevel => Level.LOW;
        protected override string Name => $"{nameof(捕兽陷阱)}";
    }
}
