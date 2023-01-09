using System.Threading.Tasks;
using WildernessSurvival.Core;
using WildernessSurvival.Localization;

// ReSharper disable CheckNamespace
namespace WildernessSurvival.Game
{
    public static class CuttingItems
    {
        public static readonly ItemMaker<ToolItem> SurvivalKnife = () => new ToolItem
        {
            Durability = 18,
            Name = "SurvivalKnife",
            Level = ToolLevel.High,
            ToolType = ToolType.Cutting,
        };
    }

    public static class OxeItems
    {
        public static readonly ItemMaker<ToolItem> OldOxe = () => new ToolItem
        {
            Durability = 15,
            Name = "OldOxe",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Oxe,
        };
    }

    public static class FishToolItems
    {
        public static readonly ItemMaker<ToolItem> OldFishRod = () => new ToolItem
        {
            Durability = 15,
            Name = "OldFishRod",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Fishing,
        };
    }

    public static class HuntingToolItems
    {
        public static readonly ItemMaker<ToolItem> OldShotgun = () => new ToolItem
        {
            Durability = 15,
            Name = "OldShotgun",
            Level = ToolLevel.High,
            ToolType = ToolType.Hunting,
        };

        public static readonly ItemMaker<ToolItem> Trap = () => new ToolItem
        {
            Durability = 6,
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

        public virtual float FireRate { get; set; } = 1f;
        public virtual float InitialFireFuel { get; set; } = 20f;
        public override UseType UseType => UseType.Use;
        public override bool CanUse(Player player) => !player.HasFire;

        public override string Name { get; }

        public override void BuildAttrModification(AttrModifierBuilder builder)
        {
            builder.Add(AttrType.Food.WithEffect(-0.03f));
            builder.Add(AttrType.Water.WithEffect(-0.05f));
            builder.Add(AttrType.Energy.WithEffect(-0.08f));
        }

        public override async Task Use(Player player)
        {
            if (player.HasFire) return;
            await base.Use(player);
            var wet = player.Location.Wet;
            int makingPromptNumber = player["Fire.MakingPrompt"] ?? 0;
            // wet will reduce the possibility to start a fire
            var wetFix = 1f - wet;
            var maxFireMakingPrompt = player.Hardness.MaxFireMakingPrompt();
            var maxTryFix = 1f + (float)makingPromptNumber / maxFireMakingPrompt;
            if (Rand.Float() < FireRate * wetFix * maxTryFix)
            {
                player.FireFuel = InitialFireFuel;
                player["Fire.MakingPrompt"] = 0;
                await App.Current.MainPage.DisplayAlert(
                    title: ActionType.Fire.LocalizedName(),
                    message: "Fire.Success".Tr(),
                    cancel: "OK".Tr()
                );
            }
            else
            {
                player["Fire.MakingPrompt"] = makingPromptNumber + 1;
                var reason = wet > 0.3f ? "Fire.Failed.Wet" : "Fire.Failed.FireRate";
                await App.Current.MainPage.DisplayAlert(
                    title: ActionType.Fire.LocalizedName(),
                    message: reason.Tr(),
                    cancel: "OK".Tr()
                );
            }
        }
    }

    public class HandDrillKit : FireStarterItem
    {
        public const float DefaultFireRate = 0.4f;
        public override float FireRate { get; set; } = DefaultFireRate;
        public const float DefaultInitialFireFuel = 1.5f;
        public override float InitialFireFuel { get; set; } = DefaultInitialFireFuel;

        public HandDrillKit() : base("HandDrillKit")
        {
        }
    }
}