using System;
using System.Collections.Generic;
using System.Linq;
using WildernessSurvival.Game;

namespace WildernessSurvival.Core
{
    public class Backpack
    {
        public Backpack(Player owner)
        {
            Owner = owner;
        }

        private Player Owner { get; }

        public readonly List<IItem> AllItems = new List<IItem>();

        public void AddItem(IItem item)
        {
            AllItems.Add(item);
        }

        public IItem GetItemByName(string name)
        {
            return AllItems.SingleOrDefault(e => e.Name.Equals(name));
        }
        public bool HasItemOfName(string name)
        {
            return AllItems.Any(e => e.Name.Equals(name));
        }

        public bool RemoveItemByName(string name)
        {
            return AllItems.Remove(AllItems.SingleOrDefault(e => e.Name.Equals(name)));
        }

        public int RemoveItemsWhere(Predicate<IItem> predicate)
        {
            return AllItems.RemoveAll(predicate);
        }

        public bool RemoveItemByType(Type type)
        {
            return AllItems.Remove(AllItems.SingleOrDefault(type.IsInstanceOfType));
        }

        public bool RemoveItem(IItem item)
        {
            return AllItems.Remove(item);
        }

        public void AddItems(IEnumerable<IItem> items)
        {
            AllItems.AddRange(items);
        }

        public void ConsumeWood(int count)
        {
            if (count <= 0) return;
            var woods = AllItems.OfType<Log>().ToList();
            var c = count > woods.Count ? woods.Count : count;
            for (var frequency = 0; frequency < c; ++frequency) AllItems.Remove(woods[frequency]);
        }
    }
}