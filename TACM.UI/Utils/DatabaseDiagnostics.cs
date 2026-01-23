using System;
using System.IO;
using TACM.Data;

namespace TACM.UI.Utils
{
    /// <summary>
    /// Helper class to diagnose database issues on macOS and other platforms
    /// </summary>
    public static class DatabaseDiagnostics
    {
        public static string GetDatabaseDiagnostics()
        {
            var diagnostics = new System.Text.StringBuilder();
            
            diagnostics.AppendLine("=== DATABASE DIAGNOSTICS ===");
            diagnostics.AppendLine($"Platform: {DeviceInfo.Platform}");
            diagnostics.AppendLine($"Device: {DeviceInfo.Model}");
            diagnostics.AppendLine($"OS: {DeviceInfo.VersionString}");
            diagnostics.AppendLine();
            
            // Check LocalApplicationData path
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            diagnostics.AppendLine($"LocalApplicationData: {localAppData}");
            diagnostics.AppendLine($"LocalApplicationData exists: {Directory.Exists(localAppData)}");
            diagnostics.AppendLine();
            
            // Check TACM directory
            var tacmDir = Path.Combine(localAppData, "TACM");
            diagnostics.AppendLine($"TACM Directory: {tacmDir}");
            diagnostics.AppendLine($"TACM Directory exists: {Directory.Exists(tacmDir)}");
            diagnostics.AppendLine();
            
            // Check database file
            var dbPath = Path.Combine(tacmDir, "tacm.db");
            diagnostics.AppendLine($"Database Path: {dbPath}");
            diagnostics.AppendLine($"Database exists: {File.Exists(dbPath)}");
            
            if (File.Exists(dbPath))
            {
                var fileInfo = new FileInfo(dbPath);
                diagnostics.AppendLine($"Database size: {fileInfo.Length} bytes");
                diagnostics.AppendLine($"Database created: {fileInfo.CreationTime}");
                diagnostics.AppendLine($"Database modified: {fileInfo.LastWriteTime}");
            }
            diagnostics.AppendLine();
            
            // Check AppDataDirectory
            diagnostics.AppendLine($"FileSystem.AppDataDirectory: {FileSystem.AppDataDirectory}");
            diagnostics.AppendLine();
            
            // Check AppContext.BaseDirectory
            diagnostics.AppendLine($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");
            diagnostics.AppendLine();
            
            // Try to get connection string
            try
            {
                var connectionString = TacmDbContextFactory.GetConnectionString();
                diagnostics.AppendLine($"Connection String: {connectionString}");
            }
            catch (Exception ex)
            {
                diagnostics.AppendLine($"Connection String Error: {ex.Message}");
            }
            diagnostics.AppendLine();
            
            // Check if we can create DbContext
            try
            {
                using var context = TacmDbContextFactory.CreateDbContext();
                diagnostics.AppendLine("DbContext created: SUCCESS");
                
                // Try a simple query
                var canConnect = context.Database.CanConnect();
                diagnostics.AppendLine($"Database can connect: {canConnect}");
            }
            catch (Exception ex)
            {
                diagnostics.AppendLine($"DbContext Error: {ex.Message}");
            }
            
            diagnostics.AppendLine("=== END DIAGNOSTICS ===");
            
            return diagnostics.ToString();
        }
        
        public static void LogDiagnostics()
        {
            var diagnostics = GetDatabaseDiagnostics();
            AppLogger.Log(diagnostics);
            
#if DEBUG
            System.Diagnostics.Debug.WriteLine(diagnostics);
#endif
        }
        
        public static void EnsureDatabaseDirectory()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var tacmDir = Path.Combine(localAppData, "TACM");
            
            if (!Directory.Exists(tacmDir))
            {
                Directory.CreateDirectory(tacmDir);
                AppLogger.Log($"Created TACM directory at: {tacmDir}");
            }
        }
    }
}
