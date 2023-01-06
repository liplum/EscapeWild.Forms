using System.Collections.Generic;
using System.Globalization;

namespace WildernessSurvival.Localization
{
    public class LangZhCn : ILocalization
    {
        public CultureInfo BoundCulture { get; } = new CultureInfo("zh-CN");

        public Dictionary<string, string> TranslationKey2Localized { get; } = new Dictionary<string, string>
        {
            { "Item.Log.Name", "木头" },
            { "Item.Log.Desc", "一根没什么特点的木头。" },
            { "Item.EnergyBar.Name", "能量棒" },
            { "Item.EnergyBar.Desc", "嗯，来劲了。" },
            { "Item.OldOxe.Name", "老旧的斧头" },
            { "Item.OldOxe.Desc", "一把老旧的斧头，但是木柄还是很结实。" },
            { "Item.BottledWater.Name", "瓶装水" },
            { "Item.BottledWater.Desc", "一瓶矿泉水，不知道放了多久。" },
            { "Item.RawRabbit.Name", "生兔肉" },
            { "Item.RawRabbit.Desc", "新鲜的生兔肉，你觉得应该烧熟了再吃。" },
            { "Item.CookedRabbit.Name", "烤兔肉" },
            { "Item.CookedRabbit.Desc", "兔兔辣么可爱，你为什么要吃兔兔。" },
            { "Item.OldFishRod.Name", "老旧的钓鱼竿" },
            { "Item.OldFishRod.Desc", "一根老旧的钓鱼竿，应该还可以钓鱼。" },
            { "Item.Berry.Name", "浆果" },
            { "Item.Berry.Desc", "在别的游戏里，我可是主角。" },
            { "Item.DirtyWater.Name", "脏水" },
            { "Item.DirtyWater.Desc", "你觉得应该烧开了再喝。" },
            { "Item.CleanWater.Name", "净水" },
            { "Item.CleanWater.Desc", "干净的水，正好解渴。" },
            { "Item.Nuts.Name", "坚果" },
            { "Item.Nuts.Desc", "芜香蛋。" },
            { "Item.Bandage.Name", "绷带" },
            { "Item.Bandage.Desc", "用来包扎伤口。" },
            { "Item.FistAidKit.Name", "急救包" },
            { "Item.FistAidKit.Desc", "等我打个包先。" },
            { "Item.EnergyDrink.Name", "能量饮料" },
            { "Item.EnergyDrink.Desc", "让你随时麦动回来。" },
            { "Item.RawFish.Name", "生鱼" },
            { "Item.RawFish.Desc", "红鲤鱼与绿鲤鱼与驴。" },
            { "Item.CookedFish.Name", "熟鱼" },
            { "Item.CookedFish.Desc", "授人以鱼不如授人以渔。" },
            { "Item.OldShotgun.Name", "老旧的猎枪" },
            { "Item.OldShotgun.Desc", "还是双管的。" },
            { "Item.Trap.Name", "捕兽陷阱" },
            { "Item.Trap.Desc", "应该没有傻子会自己踩到它吧。" },
            { "Cook.Cook", "烹饪" },
            { "Cook.NoWood", "没有木头" },
        };
    }
}