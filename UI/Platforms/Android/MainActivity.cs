using Android.App;
using Android.Content.PM;
using Android.OS;
#if ANDROID
using Plugin.Firebase.Core.Platforms.Android;
using Plugin.Firebase.CloudMessaging;
#endif
namespace UI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
