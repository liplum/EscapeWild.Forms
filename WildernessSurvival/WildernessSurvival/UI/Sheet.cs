using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WildernessSurvival.UI
{
    public static class Sheet
    {
        public static async Task ShowModalSheetAndAwaitPop(this NavigableElement self, Page page)
        {
            // Await the navigation pop
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            page.Disappearing += (sender2, e2) => { waitHandle.Set(); };
            await self.Navigation.PushModalAsync(page, true);
            await Task.Run(() => waitHandle.WaitOne());
        }
    }
}