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
        string Name { get; }
        Task PerformAction(Player player, ActionType action);
        ISet<ActionType> AvailableActions { get; }

        /// <summary>
        /// [0f,1f]
        /// </summary>
        float Wet { get; }
    }

    public static class RouteI18N
    {
        public static string LocalizedName(this IPlace place) =>
            I18N.Get($"Route.{place.Route.Name}.{place.Name}");

        public static string LocalizedName<T>(this IRoute<T> route) where T : IPlace =>
            I18N.Get($"Route.{route.Name}");
    }
}