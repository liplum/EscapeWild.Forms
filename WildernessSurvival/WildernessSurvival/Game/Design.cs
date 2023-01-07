using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Design
    {
        public static (int rate, int doubleRate) CalcRateByTool(this IToolItem tool) => tool.Level switch
        {
            ToolLevel.Low => (40, 10),
            ToolLevel.Normal => (55, 20),
            ToolLevel.High => (70, 30),
            _ => (100, 50)
        };

        public static float CalcExtraCostByTool(this IToolItem tool) => tool.Level switch
        {
            ToolLevel.Low => 1.35f,
            ToolLevel.Normal => 1f,
            ToolLevel.High => 0.85f,
            _ => 0.75f
        };
    }
}