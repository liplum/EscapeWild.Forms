using System.Collections.Generic;
using Android.Widget;
using WildernessSurvival.Droid.UI;
using WildernessSurvival.UI;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(ToastAndroid))]

namespace WildernessSurvival.Droid.UI
{
    public class ToastAndroid : IToast
    {
        private readonly HashSet<Toast> AllToasts = new HashSet<Toast>();

        public void Clear()
        {
            foreach (var toast in AllToasts) toast.Cancel();
            AllToasts.Clear();
        }

        public void LongAlert(string message)
        {
            Alert(message, ToastLength.Long);
        }

        public void ShortAlert(string message)
        {
            Alert(message, ToastLength.Short);
        }

        public void Alert(string message, ToastLength length)
        {
            Clear();
            var toast = Toast.MakeText(Application.Context, message, length);
            if (toast != null)
            {
                toast.AddCallback(new Callback(this, toast));
                AllToasts.Add(toast);
                toast.Show();
            }
        }

        public class Callback : Toast.Callback
        {
            public readonly ToastAndroid outer;
            public readonly Toast toast;

            public Callback(ToastAndroid outer, Toast toast)
            {
                this.outer = outer;
                this.toast = toast;
            }

            public override void OnToastHidden()
            {
                base.OnToastHidden();
                outer.AllToasts.Remove(toast);
            }
        }
    }
}