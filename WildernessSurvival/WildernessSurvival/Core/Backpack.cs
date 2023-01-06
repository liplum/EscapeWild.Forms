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

        public bool HasRaw => AllItems.OfType<IRawItem>().Any();

        public bool HasOxe => AllItems.OfType<IOxeItem>().Any();

        public bool HasFishRod => AllItems.OfType<IFishToolItem>().Any();

        public bool HasHuntingTool => AllItems.OfType<IHuntingToolItem>().Any();

        public bool HasWood => AllItems.OfType<LogItem>().Any();

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

        public IList<IHuntingToolItem> HuntingTools => AllItems.OfType<IHuntingToolItem>().ToList();

        private IList<LogItem> Woods => AllItems.OfType<LogItem>().ToList();

        public void AddItem(IItem item)
        {
            AllItems.Add(item);
        }

        public void Use(IUsableItem item)
        {
            item.Use(Owner);
        }

        public void Remove(IItem item)
        {
            if (AllItems.Contains(item))
                AllItems.Remove(item);
        }

        public void Append(IEnumerable<IItem> items)
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