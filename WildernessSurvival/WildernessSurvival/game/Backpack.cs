using System.Collections.Generic;
using System.Linq;

namespace WildernessSurvival.game
{
    public class Backpack
    {
        public Backpack(Player owner)
        {
            AllItems = new List<IItem>();
            Owner = owner;
        }

        private Player Owner { get; }

        public IList<IItem> AllItems { get; private set; }

        public bool HasRaw => AllItems.OfType<IRawItem>().Any();

        public bool HasOxe => AllItems.OfType<IOxeItem>().Any();

        public bool HasFishRod => AllItems.OfType<IFishToolItem>().Any();

        public bool HasHunting => AllItems.OfType<IHuntingToolItem>().Any();

        public bool HasWood => AllItems.OfType<IWoodItem>().Any();

        /*public IList<ItemBase> AvailableItems
        {
            get
            {
                return (from item in AllItems where item is AvailableItem select item).ToList();
            }
        }*/
        public IList<IRawItem> RawItems
        {
            get
            {
                var raws = new List<IRawItem>();
                foreach (var item in AllItems)
                    if (item is IRawItem raw)
                        raws.Add(raw);
                return raws;
            }
        }

        public IList<IItem> HuntingTools => (from item in AllItems where item is IHuntingToolItem select item).ToList();

        private IList<IItem> Woods => (from item in AllItems where item is IWoodItem select item).ToList();

        public void AddItem(IItem item)
        {
            AllItems.Add(item);
        }

        public bool Use(IItem item)
        {
            if (item is IUsableItem i)
            {
                i.Use(Owner);
                return true;
            }

            return false;
        }

        public void Remove(IItem item)
        {
            if (AllItems.Contains(item))
                AllItems.Remove(item);
        }

        public void Append(IList<IItem> items)
        {
            AllItems = AllItems.Concat(items).ToList();
        }

        public void ConsumeWood(int count)
        {
            if (count > 0)
            {
                var woods = Woods;
                var c = count > woods.Count ? woods.Count : count;
                for (var frequency = 0; frequency < c; ++frequency) AllItems.Remove(woods[frequency]);
            }
        }
    }
}