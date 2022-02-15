using System;
using System.Collections.Generic;
using System.Linq;

namespace WildernessSurvival.game {
    public class Route {

        private const int CHANGED_RATE = 30;//%
        private static readonly Random Random = new Random();

        /// <summary>
        /// 亚热带平原
        /// </summary>
        public static readonly Route SubtropicsRoute = new Route(
            new Place("平原", HasALotOfLog: false, CanFish: false, AppearRate: 30, HuntingRate: 50, IsSpecial: false),
            new Place("河边", HasALotOfLog: false, CanFish: true, AppearRate: 30, HuntingRate: 30, IsSpecial: false),
            new Place("森林", HasALotOfLog: true, CanFish: false, AppearRate: 30, HuntingRate: 60, IsSpecial: false),
            new Place("小屋", HasALotOfLog: false, CanFish: false, AppearRate: 10, HuntingRate: 30, IsSpecial: true)
            );

        private readonly List<Place> AllPlace;

        public Place CurPlace {
            get;
            private set;
        }

        public class Place {
            public string Name {
                get;
            }
            /// <summary>
            /// 是否有大量的木头
            /// </summary>
            public bool HasALotOfLog {
                get;
            }
            /// <summary>
            /// 能否捕鱼
            /// </summary>
            public bool CanFish {
                get;
            }

            public int AppearRate {
                get;
            }

            public int HuntingRate {
                get;
            }

            public bool IsSpecial {
                get;
            }

            public Place(string Name, bool HasALotOfLog, bool CanFish, int AppearRate, int HuntingRate, bool IsSpecial) {
                this.Name = Name;
                this.HasALotOfLog = HasALotOfLog;
                this.CanFish = CanFish;
                this.AppearRate = AppearRate;
                this.HuntingRate = HuntingRate;
                this.IsSpecial = IsSpecial;
            }
        }

        public Route(params Place[] Places) {
            AllPlace = Places.ToList();
            CurPlace = AllPlace[0];
        }

        public Route(Route route) {
            AllPlace = route.AllPlace;
        }

        public Place NextPlace {
            get {
                var isChanged = Random.Next(100);
                if (CurPlace.IsSpecial || isChanged < CHANGED_RATE) {
                    var range = 100 - CurPlace.AppearRate;
                    var next = Random.Next(range);
                    var OtherPlace = (from p in AllPlace where p.Name != CurPlace.Name select p).ToList();

                    for (int i = 0, sum = 0; i <= OtherPlace.Count; ++i) {
                        var p = OtherPlace[i];
                        sum += p.AppearRate;
                        if (next <= sum) {
                            CurPlace = p;
                            return p;
                        }
                    }
                }
                return CurPlace;
            }
        }

        public void Reset() {
            CurPlace = AllPlace[0];
        }
    }
}
