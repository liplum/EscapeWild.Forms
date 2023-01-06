using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildernessSurvival.Localization;

namespace WildernessSurvival.Core
{
    public class ActionType
    {
        public readonly string Name;

        private ActionType(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

        public static readonly ActionType
            Move = new ActionType("Move"),
            Explore = new ActionType("Explore"),
            Rest = new ActionType("Rest"),
            Fire = new ActionType("Fire"),
            Hunt = new ActionType("Hunt"),
            CutDownTree = new ActionType("CutDownTree"),
            Fish = new ActionType("Fish");
    }

    public static class ActionTypeI18N
    {
        public static string LocalizedName(this ActionType action) => I18N.Get($"Action.{action.Name}");
    }

    public partial class Player
    {
        public async Task PerformAction(ActionType action)
        {
            if (IsDead) return;
            await Location.PerformAction(this, action);
            ++TurnCount;
        }

        public async Task DisplayGainedItems(List<IItem> gained)
        {
            if (gained.Count > 0)
            {
                const string Head = "Dialog.DisplayGainedItems";
                var result = string.Join(", ", from item in gained select item.LocalizedName());
                await App.Current.MainPage.DisplayAlert(
                    title: $"{Head}.Title".Tr(),
                    message: string.Format($"{Head}.Content".Tr(), result),
                    cancel: "OK".Tr()
                );
            }
            else
            {
                const string Head = "Dialog.DisplayNoItemGained";
                await App.Current.MainPage.DisplayAlert(
                    title: $"{Head}.Title".Tr(),
                    message: $"{Head}.Content".Tr(),
                    cancel: "Alright".Tr()
                );
            }
        }
    }
}