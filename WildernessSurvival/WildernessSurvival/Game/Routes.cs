using System.Collections.Generic;
using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Routes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardness"></param>
        /// <returns></returns>
        public static IRoute<IPlace> SubtropicsRoute(Hardness hardness)
        {
            var generator = new Subtropics.RouteGenerator
            {
                Hardness = hardness,
                Blocks = new List<Subtropics.RouteBlock>
                {
                    new Subtropics.RouteBlock
                    {
                        Place = () => new Subtropics.PlainPlace
                        {
                            Name = "Plain",
                            HuntingRate = 50,
                        },
                        BlockSize = 30f,
                    },
                    new Subtropics.RouteBlock
                    {
                        Place = () => new Subtropics.RiversidePlace
                        {
                            Name = "Riverside",
                            HuntingRate = 30,
                            Wet = 0.4f,
                        },
                        BlockSize = 10f,
                    },
                    new Subtropics.RouteBlock
                    {
                        Place = () => new Subtropics.ForestPlace
                        {
                            Name = "Forest",
                            HuntingRate = 60,
                        },
                        BlockSize = 10f,
                    }
                },
                Decorate = places =>
                {
                    var hutPos = places.Count switch
                    {
                        0 => 0,
                        1 => 0,
                        _ => Rand.Int(1, places.Count - 1)
                    };
                    places.Insert(hutPos, new Subtropics.RouteEntry
                    {
                        Place = new Subtropics.HutPlace
                        {
                            Name = "Hut",
                            HuntingRate = 30,
                        }
                    });
                }
            };
            return generator.Generate(name: "Subtropics");
        }
    }
}