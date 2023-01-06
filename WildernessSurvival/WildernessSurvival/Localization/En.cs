﻿using System.Collections.Generic;
using System.Globalization;

namespace WildernessSurvival.Localization
{
    public class LangEn : ILocalization
    {
        public CultureInfo BoundCulture { get; } = new CultureInfo("en");

        public Dictionary<string, string> TranslationKey2Localized { get; } = new Dictionary<string, string>
        {
            { "Attr.Health", "HEALTH" },
            { "Attr.Food", "FOOD" },
            { "Attr.Water", "WATER" },
            { "Attr.Energy", "ENERGY" },
            { "Attr.Location", "LOCATION" },
            { "Item.Log.Name", "Log" },
            { "Item.Log.Desc", "An unremarkable piece of wood." },
            { "Item.EnergyBar.Name", "Energy Bar" },
            { "Item.EnergyBar.Desc", "You're not you when you're hungry." },
            { "Item.OldOxe.Name", "Old Oxe" },
            { "Item.OldOxe.Desc", "An old axe, but still sharp." },
            { "Item.BottledWater.Name", "Bottled Water" },
            { "Item.BottledWater.Desc", "A bottle of water, no one knows how long it has been stored." },
            { "Item.RawRabbit.Name", "Raw Rabbit" },
            { "Item.RawRabbit.Desc", "No fat, rich in protein." },
            { "Item.CookedRabbit.Name", "Cooked Rabbit" },
            { "Item.CookedRabbit.Desc", "Good smell." },
            { "Item.OldFishRod.Name", "Old Fish Rod" },
            { "Item.OldFishRod.Desc", "Now go fishing." },
            { "Item.Berry.Name", "Berry" },
            { "Item.Berry.Desc", "It reminds me of berry jam." },
            { "Item.DirtyWater.Name", "Dirty Water" },
            { "Item.DirtyWater.Desc", "It's better to boil." },
            { "Item.CleanWater.Name", "Clean Water" },
            { "Item.CleanWater.Desc", "Quenching your thirst." },
            { "Item.Nuts.Name", "Nuts" },
            { "Item.Nuts.Desc", "Where is my nutcracker?" },
            { "Item.Bandage.Name", "Bandage" },
            { "Item.Bandage.Desc", "No more wounds." },
            { "Item.FistAidKit.Name", "Fist Aid Kit" },
            { "Item.FistAidKit.Desc", "I need healing!" },
            { "Item.EnergyDrink.Name", "Energy Drink" },
            { "Item.EnergyDrink.Desc", "Watch out the beast." },
            { "Item.RawFish.Name", "Raw Fish" },
            { "Item.RawFish.Desc", "Phishing." },
            { "Item.CookedFish.Name", "Cooked Fish" },
            { "Item.CookedFish.Desc", "Good smell." },
            { "Item.OldShotgun.Name", "Old Shotgun" },
            { "Item.OldShotgun.Desc", "It still can work." },
            { "Item.Trap.Name", "Trap" },
            { "Item.Trap.Desc", "Wait and see." },
            { "Cook.Cook", "Cook" },
            { "Cook.CookTip", "Select one to cook" },
            { "Cook.Header", "Campfire" },
            { "Cook.NoWood", "No Wood" },
            { "Cook.AfterCooked", "After Cooked: {0}" },
            { "Cook.AfterBoiled", "After Boiled: {0}" },
            { "Backpack.Header", "Backpack" },
            { "Backpack.CannotUse", "Can't Use" },
            { "Backpack.Use", "Use" },
            { "Backpack.UseTip", "Select one to use" },
            { "Backpack.Eat", "Eat" },
            { "Backpack.Drink", "Drink" },
            { "Dialog.Failed.Title", "Failed" },
            { "Dialog.Failed.Content", "You are dead, but last {0} turns." },
            { "Dialog.Failed.Accept", "Restart" },
            { "Dialog.Failed.Cancel", "Not Now" },
            { "Dialog.Win.Title", "Congratulation!" },
            { "Dialog.Win.Content", "You escape from the wild after {0} turns!" },
            { "Dialog.Win.Accept", "Play Again" },
            { "Dialog.Win.Cancel", "Not Now" },
            { "Dialog.DisplayGainedItems.Title", "Result" },
            { "Dialog.DisplayGainedItems.Content", "You got {0}." },
            { "Dialog.DisplayNoItemGained.Title", "Too Bad" },
            { "Dialog.DisplayNoItemGained.Content", "You got nothing." },
            { "Route.Subtropics", "Subtropics" },
            { "Route.Subtropics.Plain", "Plain" },
            { "Route.Subtropics.Riverside", "Riverside" },
            { "Route.Subtropics.Forest", "Forest" },
            { "Route.Subtropics.Hut", "Hut" },
            { "Action.Move", "Move" },
            { "Action.Explore", "Explore" },
            { "Action.Rest", "Rest" },
            { "Action.Fire", "Fire" },
            { "Action.Hunt", "Hunt" },
            { "Action.CutDownTree", "Cut Down" },
            { "Action.Fish", "Fish" },
            { "Action.Restart", "Restart" },
            { "Action.Backpack", "Backpack" },
            { "Action.Cook", "Cook" },
            { "Subtropics.Common.Rest", "You took a break and feel better." },
            { "Subtropics.Common.Fire", "You start a fire." },
            { "OK", "OK" },
            { "Alright", "Alright" }
        };
    }
}