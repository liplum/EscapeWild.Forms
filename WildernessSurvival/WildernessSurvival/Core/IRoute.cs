using WildernessSurvival.Localization;

namespace WildernessSurvival.Core
{
    public delegate IRoute<TPlace> RouteMaker<out TPlace>() where TPlace : IPlace;

    public interface IRoute<out TPlace> where TPlace : IPlace
    {
        TPlace CurPlace { get; }

        TPlace NextPlace { get; }
    }

    public interface IPlace
    {
        string Name { get; }

        public bool HasLog { get; }
        public bool CanFish { get; }

        public int HuntingRate { get; }
    }

    public static class PlaceI18N
    {
        public static string LocalizedName(this IPlace place) => I18N.Get($"Place.{place.Name}.Name");
    }
}