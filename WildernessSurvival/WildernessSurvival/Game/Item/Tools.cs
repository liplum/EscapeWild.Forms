using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;

// ReSharper disable CheckNamespace
namespace WildernessSurvival.Game
{
    public static class OxeItems
    {
        public static readonly ItemMaker<ToolItem> OldOxe = () => new ToolItem(durability: 10)
        {
            Name = "OldOxe",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Oxe,
        };
    }

    public static class FishToolItems
    {
        public static readonly ItemMaker<ToolItem> OldFishRod = () => new ToolItem(durability: 10)
        {
            Name = "OldFishRod",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Fishing,
        };
    }

    public static class HuntingToolItems
    {
        public static readonly ItemMaker<ToolItem> OldShotgun = () => new ToolItem(durability: 10)
        {
            Name = "OldShotgun",
            Level = ToolLevel.High,
            ToolType = ToolType.Hunting,
        };

        public static readonly ItemMaker<ToolItem> Trap = () => new ToolItem(durability: 2)
        {
            Name = "Trap",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Hunting,
        };
    }

    public class FireStarterItem : UsableItem
    {
        public FireStarterItem(string name)
        {
            Name = name;
        }

        public float FireRate { get; set; } = 1f;
        public float InitialFireFuel { get; set; } = 20f;
        public override UseType UseType => UseType.Use;
        public override bool CanUse(Player player) => !player.HasFire;

        public override string Name { get; }

        public override void BuildUseEffect(UseEffectBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(-0.03f));
            builder.Add(AttrType.Water.WithEffect(-0.05f));
            builder.Add(AttrType.Energy.WithEffect(-0.08f));
        }

        public override async Task Use(Player player)
        {
            if (IsUsed || player.HasFire) return;
            await base.Use(player);
            var wet = player.Location.Wet;
            if (Rand.Float() < FireRate * (1f - wet))
            {
                player.FireFuel = InitialFireFuel;
                await player.DisplayMakingFireResult("Fire.Success".Tr());
            }
            else
            {
                var reason = wet > 0.5f ? "Fire.Failed.Wet" : "Fire.Failed.FireRate";
                await player.DisplayMakingFireResult(reason.Tr());
            }
        }
    }

    public static class FireStarterItems
    {
        public static readonly ItemMaker<FireStarterItem> HandDrillKit = () => new FireStarterItem("HandDrillKit")
        {
            FireRate = 0.4f,
            InitialFireFuel = 3.5f,
        };
    }
}