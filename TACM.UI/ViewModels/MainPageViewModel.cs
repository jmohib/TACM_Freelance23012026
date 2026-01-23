using System.Windows.Input;
using TACM.Data;
using TACM.Data.DbContextEntitiesExtensions;
using TACM.Entities;
using TACM.UI.Models;
using TACM.UI.Utils;

namespace TACM.UI.ViewModels;

public partial class MainPageViewModel : ViewModel
{
    private bool _sessionStarted = false;
    private bool _sessionNotStartedYet = true;

    public SessionModel? Session { get; set; }
    public ICommand StartSessionCommand { get; set; }
    private string _title = "";

    public string Title
    {
        get => _title;
        set
        {
            AppLogger.Log("Setting Title property");
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public bool SessionStarted
    {
        get => _sessionStarted;
        set
        {
            AppLogger.Log($"SessionStarted changed -> {value}");
            _sessionStarted = value;
            OnPropertyChanged(nameof(SessionStarted));
        }
    }

    public bool SessionNotStartedYet
    {
        get => _sessionNotStartedYet;
        set
        {
            AppLogger.Log($"SessionNotStartedYet changed -> {value}");
            _sessionNotStartedYet = value;
            OnPropertyChanged(nameof(SessionNotStartedYet));
        }
    }

    public MainPageViewModel() : base()
    {
        AppLogger.Log("MainPageViewModel constructor started");

        Session = new SessionModel();
        StartSessionCommand = new Command(StartSessionAsync);

        AppLogger.Log("MainPageViewModel constructor finished");
    }

    public async void LoadExistingSessionAsync(int sessionId)
    {
        AppLogger.Log($"LoadExistingSessionAsync({sessionId}) called");

        var context = TacmDbContextFactory.CreateDbContext();
        AppLogger.Log("DbContext created");

        var entity = await context.GetSessionByIdAsync(sessionId);
        AppLogger.Log(entity == null
            ? "No existing session found"
            : "Existing session loaded successfully");

        if (entity is null)
            return;

        Session?.CopyFromEntity(entity);
        AppLogger.Log("Session copied into model");

        SessionStarted = true;
        SessionNotStartedYet = false;

        AppLogger.Log("LoadExistingSessionAsync completed");
    }

    public async void StartSessionAsync()
    {
        AppLogger.Log("StartSessionAsync called");

        SessionCollectedData.ClearCollectedData();
        AppLogger.Log("Cleared SessionCollectedData");

        try
        {
            if (!Session?.AllPropertiesHaveValues() ?? false)
            {
                AppLogger.Log("Session validation failed – missing properties");
                return;
            }

            AppLogger.Log("Session validation passed");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                AppLogger.Log("Updating SessionStarted=true on UI thread");
                SessionStarted = true;
            });

            SessionNotStartedYet = false;
            AppLogger.Log("SessionNotStartedYet=false");

            var context = TacmDbContextFactory.CreateDbContext();
            AppLogger.Log("DbContext created");

            var session = await context.SaveSessionAsync(Session?.ToEntity<Session>()).ConfigureAwait(false);
            AppLogger.Log(session == null
                ? "SaveSessionAsync returned NULL"
                : $"Session saved with ID {session.Id}");

            SessionStarted = true;
            SessionNotStartedYet = false;

            if (session is not null)
            {
                SessionCollectedData.CollectSessionId(session.Id);
                AppLogger.Log($"SessionCollectedData updated with session id: {session.Id}");
            }
        }
        catch (Exception ex)
        {
            AppLogger.Log("ERROR in StartSessionAsync: " + ex);

            SessionStarted = false;
            SessionNotStartedYet = true;

            throw; // still throw so UI can show error
        }
    }

    public async Task<int> GetCorrectAnswerCount()
    {
        AppLogger.Log("GetCorrectAnswerCount called");

        var count = await TacmDbContext.GetCorrectAnswerCountAsync(
            SessionCollectedData.GetCollectedTestResultId());

        AppLogger.Log($"CorrectAnswerCount = {count}");

        return count;
    }
}

