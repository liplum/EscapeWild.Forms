using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Routes
    {
        public static readonly RouteMaker<IPlace> SubtropicsRoute = () => new Route(
            name: "Subtropics",
            new PlainPlace(name: "Plain", appearRate: 30, huntingRate: 50),
            new RiversidePlace(name: "Riverside", appearRate: 30, huntingRate: 30),
            new ForestPlace(name: "Forest", appearRate: 30, huntingRate: 60),
            new HutPlace(name: "Hut", appearRate: 10, huntingRate: 30)
        );
    }
}