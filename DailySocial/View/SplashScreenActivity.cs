using Android.App;
using Android.OS;
using System.Threading;

namespace DailySocial.View
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/starticon")]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Thread.Sleep(3000); // Simulate a long loading process on app startup.
            StartActivity(typeof(MainActivity));
        }
    }
}