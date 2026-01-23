using TACM.UI.Pages;
using TACM.Data;
using TACM.UI.Utils;

namespace TACM.UI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        // Log app startup immediately
        AppLogger.Log("========================================");
        AppLogger.Log($"App started at {DateTime.Now}");
        AppLogger.Log($"Platform: {DeviceInfo.Platform}");
        AppLogger.Log($"App data directory: {FileSystem.AppDataDirectory}");
        AppLogger.Log($"Log file path: {AppLogger.GetLogPath()}");
        AppLogger.Log("========================================");
        
#if WINDOWS
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            TACM.UI.Platforms.Windows.WindowExtensions.Maximize(handler.PlatformView);
        });
#endif

    
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var appShell = new AppShell
        {
            FlyoutBehavior = FlyoutBehavior.Disabled
        };

        var window = new Window(appShell);

        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();
        
        // Initialize database on app start
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            AppLogger.Log("Starting database initialization...");
            
            // Log diagnostic information
            DatabaseDiagnostics.LogDiagnostics();
            
            using var context = TacmDbContextFactory.CreateDbContext();
            
            // Ensure database and tables are created
            bool wasCreated = context.Database.EnsureCreated();
            
            AppLogger.Log($"Database initialized successfully (wasCreated: {wasCreated})");
            
            // Seed default settings if database was just created or no settings exist
            if (!context.Settings.Any())
            {
                AppLogger.Log("No settings found, seeding default settings...");
                
                var defaultSettings = new Entities.Settings
                {
                    Id = 0,
                    Target = 'X',
                    FontSize = 72,
                    T1 = 2500,
                    T2 = 1500,
                    T3 = 3500,
                    T4 = 3500,
                    GoProbability = 20,
                    NoProbability = 80,
                    Trials = 50,
                    RND = 3500,
                    AppTitle = "Test of Verbal, Non Verbal",
                    VerbalMemoryTestDemoWordsQuantity = 3,
                    VerbalMemoryTestWordsQuantity = 25,
                    NonVerbalMemoryTestDemoWordsQuantity = 3,
                    NonVerbalMemoryTestWordsQuantity = 25,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                
                context.Settings.Add(defaultSettings);
                context.SaveChanges();
                
                AppLogger.Log("Default settings seeded successfully");
            }
            else
            {
                AppLogger.Log("Settings already exist, checking for updates...");
                
                // Update existing settings to correct demo quantities if they're wrong
                var existingSettings = context.Settings.FirstOrDefault();
                if (existingSettings != null)
                {
                    if (existingSettings.VerbalMemoryTestDemoWordsQuantity != 3 || 
                        existingSettings.NonVerbalMemoryTestDemoWordsQuantity != 3)
                    {
                        AppLogger.Log($"Fixing demo quantities: Verbal={existingSettings.VerbalMemoryTestDemoWordsQuantity}->3, NonVerbal={existingSettings.NonVerbalMemoryTestDemoWordsQuantity}->3");
                        existingSettings.VerbalMemoryTestDemoWordsQuantity = 3;
                        existingSettings.NonVerbalMemoryTestDemoWordsQuantity = 3;
                        context.SaveChanges();
                        AppLogger.Log("Demo quantities updated successfully");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AppLogger.Log($"CRITICAL: Database initialization failed - {ex.Message}");
            AppLogger.Log($"Stack trace: {ex.StackTrace}");
            
            // Show error to user
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Database Error",
                        $"Failed to initialize database. The app may not function correctly.\n\nError: {ex.Message}",
                        "OK"
                    );
                }
            });
        }
    }
}