using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using TACM.UI.Pages;
using Microsoft.Maui.Storage;

#if WINDOWS
using Microsoft.Maui.Controls.Handlers;
using Microsoft.UI.Xaml.Controls;
#endif
namespace TACM.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder 
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.ConfigureMauiHandlers(handlers =>
            {
#if WINDOWS
    handlers.AddHandler(typeof(NavigationPage), typeof(NoAnimationNavigationPageHandler));
#endif
            });


#if DEBUG
            builder.Logging.AddDebug();
#endif
            // ✅ Add sharedsettings.json to configuration pipeline
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.Resources); // Mac Catalyst Resources
#if MACCATALYST
        // On Mac Catalyst, content files go into Contents/Resources
        var sharedSettingsPath = Path.Combine(AppContext.BaseDirectory, "..", "Resources", "sharedsettings.json");
#else
            // On Windows/Android/iOS, FileSystem.AppPackageDirectory works fine
            var sharedSettingsPath = Path.Combine(basePath, "sharedsettings.json");
#endif
            return builder.Build();
        }
    }
#if WINDOWS
public class NoAnimationNavigationPageHandler : NavigationViewHandler
{
    protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.Frame platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ContentTransitions = null;
        platformView.IsNavigationStackEnabled = false;
    }
}
#endif
}
