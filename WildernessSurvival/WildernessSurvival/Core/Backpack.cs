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

        public void AddItem(IItem item) => AllItems.Add(item);

        public IItem GetItemByName(string name) => AllItems.FirstOrDefault(e => e.Name.Equals(name));

        public bool HasItemOfName(string name) => AllItems.Any(e => e.Name.Equals(name));

        public int CountItemOfName(string name) => AllItems.Count(e => e.Name.Equals(name));

        public int CountItemWhere(Func<IItem, bool> predicate) => AllItems.Count(predicate);

        public bool RemoveItemByName(string name) => AllItems.Remove(AllItems.FirstOrDefault(e => e.Name.Equals(name)));

        public int RemoveItemsWhere(Predicate<IItem> predicate) => AllItems.RemoveAll(predicate);

        public bool RemoveItemByType(Type type) => AllItems.Remove(AllItems.FirstOrDefault(type.IsInstanceOfType));

        public bool RemoveItem(IItem item) => AllItems.Remove(item);

        public void AddItems(IEnumerable<IItem> items) => AllItems.AddRange(items);

        public void ConsumeWood(int count)
        {
            if (count <= 0) return;
            var woods = AllItems.OfType<Log>().ToList();
            var c = count > woods.Count ? woods.Count : count;
            for (var frequency = 0; frequency < c; ++frequency) AllItems.Remove(woods[frequency]);
        }
    }
}