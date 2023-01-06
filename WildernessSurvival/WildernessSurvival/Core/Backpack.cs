using System.Collections.Generic;
using System.Linq;

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

        public bool HasOxe => AllItems.OfType<IOxeItem>().Any();

        public bool HasFishingTool => AllItems.OfType<IFishToolItem>().Any();

        public bool HasHuntingTool => AllItems.OfType<IHuntingToolItem>().Any();

        public bool HasWood => AllItems.OfType<LogItem>().Any();

        public IList<IRawItem> GetRawItems() => AllItems.OfType<IRawItem>().ToList();

        public IList<IHuntingToolItem> GetHuntingTools() => AllItems.OfType<IHuntingToolItem>().ToList();

        private IList<LogItem> Woods => AllItems.OfType<LogItem>().ToList();

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