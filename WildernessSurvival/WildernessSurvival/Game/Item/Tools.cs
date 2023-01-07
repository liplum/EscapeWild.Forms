using WildernessSurvival.Core;

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
}