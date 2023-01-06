using System.Collections.Generic;
using System.Globalization;

namespace WildernessSurvival.Localization
{
    public class LangEn : ILocalization
    {
        public CultureInfo BoundCulture { get; } = new CultureInfo("en");

        public Dictionary<string, string> TranslationKey2Localized { get; } = new Dictionary<string, string>
        {
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
            { "Cook.NoWood", "No Wood" },
            { "Dialog.Failed.Title", "Failed" },
            { "Dialog.Failed.Content", "You are dead, but last {0} turns." },
            { "Dialog.Failed.Accept", "Restart" },
            { "Dialog.Failed.Cancel", "Not Now" },
            { "Dialog.Win.Title", "Congratulation!" },
            { "Dialog.Win.Content", "You escape from the wild after {0} turns!" },
            { "Dialog.Win.Accept", "Play Again" },
            { "Dialog.Win.Cancel", "Not Now" },
            { "Place.Subtropics.Plain.Name", "Plain" },
            { "Place.Subtropics.Riverside.Name", "Riverside" },
            { "Place.Subtropics.Forest.Name", "Forest" },
            { "Place.Subtropics.Hut.Name", "Hut" },
            { "Action.Move.Name", "Move" },
            { "Action.Explore.Name", "Explore" },
            { "Action.Rest.Name", "Rest" },
            { "Action.Fire.Name", "Fire" },
            { "Action.Hunt.Name", "Hunt" },
            { "Action.CutDownTree.Name", "Cut Down" },
            { "Action.Fish.Name", "Fish" },
            { "Subtropics.Common.Rest", "You took a break and feel better." },
            { "Subtropics.Common.Fire", "You start a fire." },
            { "OK", "OK" }
        };
    }
}