using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace TACM.UI.Utils
{
    public static class AppLogger
    {
        private static readonly string LogFilePath =
            Path.Combine(FileSystem.AppDataDirectory, "app-log.txt");

        public static void Log(string message)
        {
            try
            {
                var logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}";
                
                // Ensure directory exists
                var directory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }
                
                File.AppendAllText(LogFilePath, logLine + Environment.NewLine);
                Debug.WriteLine(logLine);
            }
            catch (Exception ex)
            {
                // Write to debug output so we can see what's failing
                Debug.WriteLine($"LOGGER ERROR: {ex.Message}");
                Debug.WriteLine($"LOGGER ERROR PATH: {LogFilePath}");
                Debug.WriteLine($"LOGGER ERROR STACK: {ex.StackTrace}");
            }
        }

        public static string GetLogPath() => LogFilePath;
    }
}
