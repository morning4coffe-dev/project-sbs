using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Identity.Client;

namespace ProjectSBS.Droid
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Android.Content.Intent.ActionView },
        Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
        DataScheme = "project-sbs")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}