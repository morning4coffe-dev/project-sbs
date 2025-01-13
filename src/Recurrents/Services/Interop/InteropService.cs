using System.Reflection;
using Windows.UI.ViewManagement;

namespace Recurrents.Services.Interop;

public static class InteropService
{
    public static Version GetAppVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return AssemblyName.GetAssemblyName(assembly.Location).Version ?? new Version(1, 0);
    }

    public static async Task OpenStoreReviewUrlAsync()
    {
#if WINDOWS || HAS_UNO_SKIA_WPF
        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9N5MJT8G06KC"));
#else
        var current = Plugin.StoreReview.CrossStoreReview.Current;
        var status = await current.RequestReview(false);

        if (status == Plugin.StoreReview.ReviewStatus.Error)
        {
#if ANDROID
            current.OpenStoreReviewPage(Android.App.Application.Context.PackageName);
#endif
        }
#endif
    }

    public static void UpdateAppTitle(string title)
    {
#if !HAS_UNO
        ApplicationView.GetForCurrentView().Title = title;
#endif
    }
}
