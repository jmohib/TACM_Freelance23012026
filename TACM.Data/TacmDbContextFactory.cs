using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TACM.Core;
//using Foundation; // for NSBundle (Mac Catalyst/iOS)

namespace TACM.Data;

public static class TacmDbContextFactory
{
    private static string? _connectionString = null;
    private static DbContextOptions<TacmDbContext>? _dbOptions;

    public static string GetConnectionString()
    {
        string basePath;
        string? sharedSettingsPath = null;

#if MACCATALYST
        // Try multiple possible locations for Mac Catalyst
        var possiblePaths = new[]
        {
            Path.Combine(Foundation.NSBundle.MainBundle.BundlePath, "Contents", "Resources", AppConstants.SHARED_SETTINGS_FILENAME),
            Path.Combine(Foundation.NSBundle.MainBundle.ResourcePath ?? "", AppConstants.SHARED_SETTINGS_FILENAME),
            Path.Combine(AppContext.BaseDirectory, "..", "Resources", AppConstants.SHARED_SETTINGS_FILENAME),
            Path.Combine(AppContext.BaseDirectory, AppConstants.SHARED_SETTINGS_FILENAME),
            Path.Combine(FileSystem.AppDataDirectory, AppConstants.SHARED_SETTINGS_FILENAME)
        };
        
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                sharedSettingsPath = path;
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Found sharedsettings.json at: {path}");
#endif
                break;
            }
        }
        
        if (sharedSettingsPath == null)
        {
            var searchedPaths = string.Join("\n  - ", possiblePaths);
            throw new FileNotFoundException($"sharedsettings.json not found. Searched in:\n  - {searchedPaths}");
        }
        
        basePath = Path.GetDirectoryName(sharedSettingsPath) ?? AppContext.BaseDirectory;
#elif IOS
    // iOS bundles resources at the root of the app bundle
    basePath = Foundation.NSBundle.MainBundle.BundlePath;
    sharedSettingsPath = Path.Combine(basePath, AppConstants.SHARED_SETTINGS_FILENAME);
#elif ANDROID
        // Android: packaged assets are not regular files; you'd need AssetManager
        // For config JSON, better to copy it to the app data folder on first run
        basePath = FileSystem.AppDataDirectory;
        sharedSettingsPath = Path.Combine(basePath, AppConstants.SHARED_SETTINGS_FILENAME);
#else
    // Windows (MAUI WinUI)
    basePath = AppContext.BaseDirectory;
    sharedSettingsPath = Path.Combine(basePath, AppConstants.SHARED_SETTINGS_FILENAME);
#endif

        if (!File.Exists(sharedSettingsPath))
        {
            throw new FileNotFoundException($"sharedsettings.json not found at {sharedSettingsPath}");
        }

        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(AppConstants.SHARED_SETTINGS_FILENAME, optional: false, reloadOnChange: false);

        var config = configBuilder.Build();
        var connectionString = config.GetConnectionString(AppConstants.DB_CONNECTION_STRING_SETTINGS_NAME);

        return connectionString!;
    }


    public static DbContextOptions<TacmDbContext> CreateDbContextOptions(string connectionString)
    {
        var contextBuilder = new DbContextOptionsBuilder<TacmDbContext>();

        // Ensure the directory exists before creating the database
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var tacmDir = Path.Combine(appDataDir, "TACM");
        
        // CREATE DIRECTORY IF IT DOESN'T EXIST - CRITICAL FOR macOS
        if (!Directory.Exists(tacmDir))
        {
            try
            {
                Directory.CreateDirectory(tacmDir);
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Created database directory: {tacmDir}");
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Failed to create directory {tacmDir}: {ex.Message}");
#endif
                throw new InvalidOperationException($"Cannot create database directory: {tacmDir}", ex);
            }
        }
        
        var dbPath = Path.Combine(tacmDir, "tacm.db");
        
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");
#endif
        
        contextBuilder.UseSqlite($"Data Source={dbPath}");

        // Option B: if you want to use the actual connection string (e.g. Postgres, SQL Server)
        // contextBuilder.UseSqlite(connectionString);

        return contextBuilder.Options;
    }

    public static TacmDbContext CreateDbContext()
    {
        try
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("CreateDbContext started");
#endif
            
            if (string.IsNullOrEmpty(_connectionString))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Getting connection string...");
#endif
                _connectionString = GetConnectionString();
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Connection string obtained: {_connectionString}");
#endif
            }

            if (_dbOptions == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Creating DbContext options...");
#endif
                _dbOptions = CreateDbContextOptions(_connectionString);
            }

            var context = new TacmDbContext(_dbOptions);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("DbContext instance created successfully");
#endif
            
            return context;
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR in CreateDbContext: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
#endif
            throw;
        }
    }
}
