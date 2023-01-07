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

        public bool HasWood => AllItems.OfType<Log>().Any();

        public IList<ICookableItem> GetRawItems() => AllItems.OfType<ICookableItem>().ToList();

        private IList<Log> Woods => AllItems.OfType<Log>().ToList();

        public void AddItem(IItem item)
        {
            AllItems.Add(item);
        }

        public void Remove(IItem item)
        {
            if (AllItems.Contains(item))
                AllItems.Remove(item);
        }

        public void AddItems(IEnumerable<IItem> items)
        {
            AllItems.AddRange(items);
        }

        public void ConsumeWood(int count)
        {
            if (count <= 0) return;
            var woods = Woods;
            var c = count > woods.Count ? woods.Count : count;
            for (var frequency = 0; frequency < c; ++frequency) AllItems.Remove(woods[frequency]);
        }
    }
}