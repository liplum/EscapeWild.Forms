using System;
using System.Collections.Generic;
using System.Linq;
using WildernessSurvival.Localization;

namespace WildernessSurvival.Core
{
    public class Route
    {
        private const int ChangedRate = 30;
        private static readonly Random Random = new Random();
        private readonly List<Place> _allPlace;

        /// <summary>
        ///     亚热带平原
        /// </summary>
        public static readonly Func<Route> SubtropicsRoute = () => new Route(
            new Place("Plain", false, false, 30, 50, false),
            new Place("Riverside", false, true, 30, 30, false),
            new Place("Forest", true, false, 30, 60, false),
            new Place("Hut", false, false, 10, 30, true)
        );


        public Route(params Place[] Places)
        {
            _allPlace = Places.ToList();
            CurPlace = _allPlace[0];
        }

        public Route(Route route)
        {
            _allPlace = route._allPlace;
        }

        public Place CurPlace { get; private set; }

        public Place NextPlace
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

    public class Place
    {
        public Place(string name, bool hasLog, bool canFish, int appearRate, int huntingRate, bool isSpecial)
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

    public static class PlaceI18N
    {
        public static string LocalizedName(this Place place) => I18N.Get($"Place.{place.Name}.Name");
    }
}