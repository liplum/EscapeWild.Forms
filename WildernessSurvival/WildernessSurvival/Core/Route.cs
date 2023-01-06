using System.Collections.Generic;
using System.Threading.Tasks;
using WildernessSurvival.Localization;

namespace WildernessSurvival.Core
{
    public delegate IRoute<TPlace> RouteMaker<out TPlace>() where TPlace : IPlace;

    public interface IRoute<out TPlace> where TPlace : IPlace
    {
        string Name { get; }
        TPlace InitialPlace { get; }
    }

    public interface IPlace
    {
        IRoute<IPlace> Route { get; }
        Task PerformAction(Player player, ActionType action);
        ISet<ActionType> AvailableActions { get; }
        string Name { get; }

        public bool HasLog { get; }
        public bool CanFish { get; }

        public int HuntingRate { get; }
    }

    public static class PlaceI18N
    {
        public static string LocalizedName(this IPlace place) =>
            I18N.Get($"Place.{place.Route.Name}.{place.Name}.Name");
    }
}