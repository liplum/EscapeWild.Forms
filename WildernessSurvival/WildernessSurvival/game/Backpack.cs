using System.Collections.Generic;
using System.Linq;

namespace WildernessSurvival.game
{
    public class Backpack
    {
        public Backpack(Player owner)
        {
            AllItems = new List<ItemBase>();
            Owner = owner;
        }

        private Player Owner { get; }

        public IList<ItemBase> AllItems { get; private set; }

        public bool HasRaw
        {
            get
            {
                foreach (var item in AllItems)
                    if (item is IRawItem)
                        return true;
                return false;
            }
        }

        public bool HasOxe
        {
            get
            {
                foreach (var item in AllItems)
                    if (item is Oxe)
                        return true;
                return false;
            }
        }

        public bool HasFishRod
        {
            get
            {
                foreach (var item in AllItems)
                    if (item is FishRod)
                        return true;
                return false;
            }
        }

        public bool HasHunting
        {
            get
            {
                foreach (var item in AllItems)
                    if (item is Hunting)
                        return true;
                return false;
            }
        }

        public bool HasWood
        {
            get
            {
                foreach (var item in AllItems)
                    if (item is Wood)
                        return true;
                return false;
            }
        }

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

        public IList<ItemBase> HuntingTools => (from item in AllItems where item is Hunting select item).ToList();

        public IList<ItemBase> Woods => (from item in AllItems where item is Wood select item).ToList();

        public void AddItem(ItemBase item)
        {
            AllItems.Add(item);
        }

        public bool Use(ItemBase item)
        {
            if (item is AvailableItem i)
            {
                i.Use(Owner);
                return true;
            }

            return false;
        }

        public void Remove(ItemBase item)
        {
            if (AllItems.Contains(item))
                AllItems.Remove(item);
        }

        public void Append(IList<ItemBase> items)
        {
            AllItems = AllItems.Concat(items).ToList();
        }

        public void ConsumeWood(int Count)
        {
            if (Count > 0)
            {
                var woods = Woods;
                var c = Count > woods.Count ? woods.Count : Count;
                for (var frequency = 0; frequency < c; ++frequency) AllItems.Remove(woods[frequency]);
            }
        }
    }
}