using System;
using System.Collections.Generic;
using System.Linq;

namespace WildernessSurvival.game
{
    public class Route
    {
        private const int ChangedRate = 30; //%
        private static readonly Random Random = new Random();

        /// <summary>
        ///     亚热带平原
        /// </summary>
        public static readonly Route SubtropicsRoute = new Route(
            new Place("平原", false, false, 30, 50, false),
            new Place("河边", false, true, 30, 30, false),
            new Place("森林", true, false, 30, 60, false),
            new Place("小屋", false, false, 10, 30, true)
        );

        private readonly List<Place> _allPlace;

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
                var isChanged = Random.Next(100);
                if (CurPlace.IsSpecial || isChanged < ChangedRate)
                {
                    var range = 100 - CurPlace.AppearRate;
                    var next = Random.Next(range);
                    var OtherPlace = (from p in _allPlace where p.Name != CurPlace.Name select p).ToList();

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

        public class Place
        {
            public Place(string Name, bool HasALotOfLog, bool CanFish, int AppearRate, int HuntingRate, bool IsSpecial)
            {
                this.Name = Name;
                this.HasALotOfLog = HasALotOfLog;
                this.CanFish = CanFish;
                this.AppearRate = AppearRate;
                this.HuntingRate = HuntingRate;
                this.IsSpecial = IsSpecial;
            }

            public string Name { get; }

            /// <summary>
            ///     是否有大量的木头
            /// </summary>
            public bool HasALotOfLog { get; }

            /// <summary>
            ///     能否捕鱼
            /// </summary>
            public bool CanFish { get; }

            public int AppearRate { get; }

            public int HuntingRate { get; }

            public bool IsSpecial { get; }
        }
    }
}