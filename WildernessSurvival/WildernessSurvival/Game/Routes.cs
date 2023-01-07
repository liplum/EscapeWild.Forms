using WildernessSurvival.Core;

namespace WildernessSurvival.Game
{
    public static class Routes
    {
        public static readonly RouteMaker<IPlace> SubtropicsRoute = () => new Route(
            name: "Subtropics", hardness: Rand.Float(0f, 0.3f),
            new Route.Entry
            {
                Place = new PlainPlace(name: "Plain", huntingRate: 50),
                Proportion = 30,
                Inertia = 0.6f,
                MaxStayCount = 12,
            }, new Route.Entry
            {
                Place = new RiversidePlace(name: "Riverside", huntingRate: 30),
                Proportion = 30,
                Inertia = 0.4f,
                MaxStayCount = 8,
            }, new Route.Entry
            {
                Place = new ForestPlace(name: "Forest", huntingRate: 60),
                Proportion = 30,
                Inertia = 0.6f,
                MaxStayCount = 10,
            }, new Route.Entry
            {
                Place = new HutPlace(name: "Hut", huntingRate: 30),
                Proportion = 10,
                Inertia = 0f,
                MaxStayCount = 1,
            }
        );
    }
}