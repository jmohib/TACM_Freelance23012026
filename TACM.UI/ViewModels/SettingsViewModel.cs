using System.Diagnostics;
using System.Windows.Input;
using TACM.Data.DbContextEntitiesExtensions;
using TACM.Entities;
using TACM.UI.Models;

namespace TACM.UI.ViewModels;

public partial class SettingsViewModel : ViewModel
{
    private SettingsModel? _settings;

    public SettingsModel? Settings 
    { 
        get => _settings;
        set
        {
            _settings = value;
            OnPropertyChanged(nameof(Settings));
        }
    }

    public ICommand SaveSettingsCommand { get; set; }
    public ICommand LoadDefaultsCommand { get; set; }
    public ICommand LoadSettingsCommand { get; set; }
    public ICommand DefaultFolderCommand { get; set; }

    public SettingsViewModel()
    {
        Settings = new SettingsModel();

        SaveSettingsCommand = new Command(SaveSettingsAsync);
        LoadDefaultsCommand = new Command(LoadDefaults);
        LoadSettingsCommand = new Command(async () => await TryLoadCurrentSettingsAsync());
        DefaultFolderCommand = new Command(OpenDefaultFolder);
    }

    public async Task TryLoadCurrentSettingsAsync()
    {
        try
        {
            var settings = await TacmDbContext.GetCurrentSettingsAsync();

            if (settings is null)
            {
                var defaultSettings = SettingsModel.GetDefaultSettings();
                Settings?.CopyFromModel(defaultSettings);
                OnPropertyChanged(nameof(Settings));
                SaveSettingsAsync();
                return;
            }

            Settings?.CopyFromEntity(settings);
            OnPropertyChanged(nameof(Settings));
        }
        catch
        {
            // If loading fails, use default settings
            var defaultSettings = SettingsModel.GetDefaultSettings();
            Settings?.CopyFromModel(defaultSettings);
            OnPropertyChanged(nameof(Settings));
        }
    }

    public async void SaveSettingsAsync()
    {
        var entity = Settings?.ToEntity<Settings>();

        if(entity is null)
            return; ;

        var saved = await TacmDbContext.SaveSettingsAsync(entity);

        Settings?.CopyFromEntity(saved);
        OnPropertyChanged(nameof(Settings));
    }

    public void LoadDefaults()
    {
        var defaultSettings = SettingsModel.GetDefaultSettings();
        Settings?.CopyFromModel(defaultSettings);
        OnPropertyChanged(nameof(Settings));
    }

    public async void OpenDefaultFolder()
    {
        // Get the default data folder path
        string folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TACM"
        );

        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Try to open the folder in the system file explorer
        try
        {
#if WINDOWS
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
#elif MACCATALYST
            System.Diagnostics.Process.Start("open", folderPath);
#else
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Default Folder", 
                    $"Data folder location:\n{folderPath}", 
                    "OK"
                );
            }
#endif
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Default Folder",
                    $"Data folder location:\n{folderPath}\n\nCould not open folder: {ex.Message}",
                    "OK"
                );
            }
        }
    }
}
