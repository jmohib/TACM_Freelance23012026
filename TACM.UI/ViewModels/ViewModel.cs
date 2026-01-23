using System.ComponentModel;
using System.Text.RegularExpressions;
using TACM.Data;
using TACM.Data.DbContextEntitiesExtensions;
using TACM.Entities;
using System.Reflection;
using TACM.UI.Utils;

namespace TACM.UI.ViewModels;

public abstract class ViewModel : INotifyPropertyChanged
{
    protected readonly List<string> _words = [];
    protected readonly List<string> _pictures = [];
    protected readonly TacmDbContext TacmDbContext;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? PicturesBasePath {  get; private set; }


    protected ViewModel()
    {
        TacmDbContext = TacmDbContextFactory.CreateDbContext();
    }


    protected ImageSource GetImageSource(in string imageFilePath) =>
        ImageSource.FromFile(Path.Combine(PicturesBasePath ?? "", imageFilePath));

    public virtual Task<Settings?> GetActiveSettingsAsync() => TacmDbContext.GetCurrentSettingsAsync();

    public virtual void OnPropertyChanged(string propertyName) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public virtual void LoadWordsDictionary()
    {
        AppLogger.Log("LoadWordsDictionary called");
        
        string filePath;

#if MACCATALYST
        // Try multiple possible locations for Mac Catalyst
        var possiblePaths = new[]
        {
            Path.Combine(Foundation.NSBundle.MainBundle.ResourcePath ?? "", "ForTests", "words.txt"),
            Path.Combine(Foundation.NSBundle.MainBundle.BundlePath, "Contents", "Resources", "ForTests", "words.txt"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", "ForTests", "words.txt"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ForTests", "words.txt")
        };
        
        AppLogger.Log($"Checking {possiblePaths.Length} possible paths for words.txt");
        foreach (var path in possiblePaths)
        {
            AppLogger.Log($"  Checking: {path} - {(File.Exists(path) ? "EXISTS ✓" : "NOT FOUND ✗")}");
        }
        
        filePath = possiblePaths.FirstOrDefault(File.Exists) 
                   ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ForTests", "words.txt");
#else
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        filePath = Path.Combine(baseDirectory, "Resources", "ForTests", "words.txt");
#endif

        _words.Clear();

        if (!File.Exists(filePath))
        {
            AppLogger.Log($"ERROR: Words file not found at: {filePath}");
            return;
        }

        AppLogger.Log($"SUCCESS: Loading words from: {filePath}");

        using var reader = new StreamReader(filePath);
        
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                _words.Add(line.Trim());
            }
        }

        reader.Close();
        
        AppLogger.Log($"Loaded {_words.Count} words successfully");
    }

    //public virtual void LoadPicturesDictionary()
    //{
    //    _pictures.Clear();

    //    var replaceRegex1 = new Regex(@"^.*\\", RegexOptions.Singleline | RegexOptions.CultureInvariant);
    //    var replaceRegex2 = new Regex(@"\\[\w]+\.[\w]{3,4}$", RegexOptions.Singleline | RegexOptions.CultureInvariant);

    //    var dirFiles = Directory.GetFiles("Resources\\ForTests\\Pictures");
    //    var files = dirFiles.Select(_ => replaceRegex1.Replace(_, ""));
    //    var basePath = replaceRegex2.Replace(dirFiles.ElementAt(0), "");

    //    _pictures.AddRange(files);
    //    PicturesBasePath = basePath;
    //}

    public virtual void LoadPicturesDictionary(int startIndex = 1, int endIndex = 81)
    {
        AppLogger.Log($"LoadPicturesDictionary called (range: {startIndex}-{endIndex})");
        
        string filePath;

#if MACCATALYST
        // Try multiple possible locations for Mac Catalyst
        var possiblePaths = new[]
        {
            Path.Combine(Foundation.NSBundle.MainBundle.ResourcePath ?? "", "ForTests", "Pictures"),
            Path.Combine(Foundation.NSBundle.MainBundle.BundlePath, "Contents", "Resources", "ForTests", "Pictures"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", "ForTests", "Pictures"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ForTests", "Pictures")
        };
        
        AppLogger.Log($"Checking {possiblePaths.Length} possible paths for Pictures directory");
        foreach (var path in possiblePaths)
        {
            AppLogger.Log($"  Checking: {path} - {(Directory.Exists(path) ? "EXISTS ✓" : "NOT FOUND ✗")}");
        }
        
        filePath = possiblePaths.FirstOrDefault(Directory.Exists) 
                   ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ForTests", "Pictures");
#else
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        filePath = Path.Combine(baseDirectory, "Resources", "ForTests", "Pictures");
#endif

        _pictures.Clear();

        if (!Directory.Exists(filePath))
        {
            AppLogger.Log($"ERROR: Pictures directory not found at: {filePath}");
            return;
        }

        AppLogger.Log($"SUCCESS: Loading pictures from: {filePath}");

        var dirFiles = Directory.GetFiles(filePath);
        AppLogger.Log($"Found {dirFiles.Length} total files in directory");

        var filteredFiles = dirFiles
            .Where(filePath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                return int.TryParse(fileName, out int number) && number >= startIndex && number <= endIndex;
            })
            .OrderBy(filePath => int.Parse(Path.GetFileNameWithoutExtension(filePath)))
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .Cast<string>()
            .ToList();

        _pictures.AddRange(filteredFiles);

        PicturesBasePath = Path.GetDirectoryName(dirFiles.First()) ?? "";
        
        AppLogger.Log($"Loaded {_pictures.Count} pictures successfully");
        AppLogger.Log($"PicturesBasePath set to: {PicturesBasePath}");
    }


}
