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

        public IItem PopItemByName(string name)
        {
            var removed = AllItems.FirstOrDefault(e => e.Name.Equals(name));
            AllItems.Remove(removed);
            return removed;
        }

        public int RemoveItemsWhere(Predicate<IItem> predicate) => AllItems.RemoveAll(predicate);

        public IItem PopItemByType(Type type)
        {
            var removed = AllItems.FirstOrDefault(type.IsInstanceOfType);
            AllItems.Remove(removed);
            return removed;
        }

        public bool RemoveItem(IItem item) => AllItems.Remove(item);

        public void AddItems(IEnumerable<IItem> items) => AllItems.AddRange(items);
    }
}