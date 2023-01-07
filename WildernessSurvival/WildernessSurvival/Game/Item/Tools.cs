using WildernessSurvival.Core;

// ReSharper disable CheckNamespace
namespace WildernessSurvival.Game
{
    public static class OxeItems
    {
        public static readonly ToolItem OldOxe = new ToolItem
        {
            Name = "OldOxe",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Oxe,
        };
    }

    public static class FishToolItems
    {
        public static readonly ToolItem OldFishRod = new ToolItem
        {
            Name = "OldFishRod",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Fishing,
        };
    }

    public static class HuntingToolItems
    {
        public static readonly ToolItem OldShotgun = new ToolItem
        {
            Name = "OldShotgun",
            Level = ToolLevel.High,
            ToolType = ToolType.Hunting,
        };

        public static readonly ToolItem Trap = new ToolItem
        {
            Name = "Trap",
            Level = ToolLevel.Normal,
            ToolType = ToolType.Hunting,
        };
    }
}