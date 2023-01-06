using System;
using System.Collections.Generic;
using System.Linq;
using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Routes
    {
        public static readonly RouteMaker<IPlace> SubtropicsRoute = () => new SubtropicsRoute(
            new SubtropicsPlace("Plain", false, false, 30, 50, false),
            new SubtropicsPlace("Riverside", false, true, 30, 30, false),
            new SubtropicsPlace("Forest", true, false, 30, 60, false),
            new SubtropicsPlace("Hut", false, false, 10, 30, true)
        );
    }

    public class SubtropicsRoute : IRoute<SubtropicsPlace>
    {
        private const int ChangedRate = 30;
        private static readonly Random Random = new Random();
        private readonly List<SubtropicsPlace> _allPlace;

        public SubtropicsRoute(params SubtropicsPlace[] Places)
        {
            _allPlace = Places.ToList();
            CurPlace = _allPlace[0];
        }

        public SubtropicsPlace CurPlace { get; private set; }

        public SubtropicsPlace NextPlace
        {
            get
            {
                var needChange = Random.Next(100);
                if (CurPlace.IsSpecial || needChange < ChangedRate)
                {
                    var cur = CurPlace;
                    var range = 100 - CurPlace.AppearRate;
                    var next = Random.Next(range);
                    var OtherPlace = (from p in _allPlace where p != cur select p).ToList();

                    for (int i = 0, sum = 0; i <= OtherPlace.Count; ++i)
                    {
                        var p = OtherPlace[i];
                        sum += p.AppearRate;
                        if (next <= sum)
                        {
                            CurPlace = p;
                            return p;
                        }
                    }
                }

                return CurPlace;
            }
        }

        public void Reset()
        {
            CurPlace = _allPlace[0];
        }
    }

    public class SubtropicsPlace : IPlace
    {
        public SubtropicsPlace(string name, bool hasLog, bool canFish, int appearRate, int huntingRate, bool isSpecial)
        {
            Name = name;
            HasLog = hasLog;
            CanFish = canFish;
            AppearRate = appearRate;
            HuntingRate = huntingRate;
            IsSpecial = isSpecial;
        }

        public string Name { get; }

        public bool HasLog { get; }
        public bool CanFish { get; }

        public int AppearRate { get; }

        public int HuntingRate { get; }

        public bool IsSpecial { get; }
    }
}