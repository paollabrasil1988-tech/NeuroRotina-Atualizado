using Android.App;
using Android.Content.PM;
using Android.OS;

namespace NeuroRotina
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Android 13+ exige permissão em tempo de execução para mostrar notificações.
            // No Android 9 essa permissão não existe e o bloco é ignorado.
            if (OperatingSystem.IsAndroidVersionAtLeast(33) &&
                CheckSelfPermission("android.permission.POST_NOTIFICATIONS") != Permission.Granted)
            {
                RequestPermissions(new[] { "android.permission.POST_NOTIFICATIONS" }, 1001);
            }
        }
    }
}
