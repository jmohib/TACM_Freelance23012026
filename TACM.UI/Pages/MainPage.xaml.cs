
using System.Data;
using TACM.Core;
using TACM.UI.Models;
using TACM.UI.Utils;
using TACM.UI.ViewModels;
//using Windows.UI.ApplicationSettings;

namespace TACM.UI.Pages;

public partial class MainPage : ContentPage
{
    private Entities.Settings? _settings;
    private readonly MainPageViewModel ViewModel;
    

	public MainPage()
	{
		InitializeComponent();

		ViewModel = new MainPageViewModel();
		BindingContext = ViewModel;
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _settings = await ViewModel.GetActiveSettingsAsync();
        if (_settings != null)
        {
            ViewModel.Title = _settings.AppTitle;
        }
        // ref var sessionId = ref SessionCollectedData.GetCollectedSessionId();
        var sessionId = SessionCollectedData.GetCollectedSessionId();

        if (sessionId == 0)
            return;

        ViewModel.LoadExistingSessionAsync(sessionId);
    }

    private async Task OpenAttentionTestPage(bool demo)
    {
        var target = _settings?.Target ?? 'X';
        var quantity = _settings?.Trials ?? 50;

        (SpanLine[] lines, string buttonText) = StartTestInitialTextsProvider.GetAttentionTestStartPageInfo(target);

        var options = new Dictionary<string, dynamic>
        {
            { "demo", true },
            { "target", target },
            { "t1", _settings?.T1 ?? AppConstants.DEFAULT_T1 },
            { "t2", _settings?.T2 ?? AppConstants.DEFAULT_T2 },
            { "t3", _settings?.T3 ?? AppConstants.DEFAULT_T3 },
            { "t4", _settings?.T4 ?? AppConstants.DEFAULT_T4 }
        };

        await Navigation.PushAsync(
            new TestIntroducingPage(
                lines,
                buttonText,
                AppConstants.OBJECT_TYPE_ATTENTION_TEST,
                quantity,
                options
            ),

            true
        );
    }


    public async void BtnSettingsClicked(object sender, EventArgs args)
    {
        try
        {
            var settingsPage = new SettingsPage();
            await Navigation.PushAsync(settingsPage, true);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not open Settings page: {ex.Message}", "OK");
        }
    }

    public async void BtnExitClicked(object sender, EventArgs args)
    {
        Application.Current.Quit();
    }

    public async void BtnReportClicked(object sender, EventArgs args)
    {
        try
        {
            // Get the current session ID
            var sessionId = SessionCollectedData.GetCollectedSessionId();
            
            if (sessionId == 0)
            {
                await DisplayAlert("Report", "No active session found. Please start a test session first.", "OK");
                return;
            }

            // Create a simple report message
            var reportText = $"Session ID: {sessionId}\n\n";
            reportText += "Test results report functionality will be available in a future update.\n\n";
            reportText += "Current session is active and data is being recorded.";

            await DisplayAlert("Test Results Report", reportText, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to generate report: {ex.Message}", "OK");
        }
    }

    public async void BtnVerbalMemoryClicked(object sender, EventArgs args)
	{
        try
        {
            AppLogger.Log("===== BtnVerbalMemoryClicked START =====");
            
            SessionTry.Set("isVerbal", true);
            SessionTry.Set("isNonVerbal", false);

            var quantityForReal = _settings?.GetVerbalMemoryTestWordsQuantity(true) ?? ushort.MinValue;
            var quantityForDemo = _settings?.GetVerbalMemoryTestWordsQuantity(false) ?? ushort.MinValue;
            var objType = AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL;

            AppLogger.Log($"Quantities - Real: {quantityForReal}, Demo: {quantityForDemo}");
            (SpanLine[] lines, string buttonText) = StartTestInitialTextsProvider.GetVerbalMemoryStartPageInfo(quantityForDemo);

            AppLogger.Log("Creating TestIntroducingPage...");
            var page = new TestIntroducingPage( 
                lines, 
                buttonText,
                objType,
                quantityForDemo
            );
            
            AppLogger.Log("Pushing page to navigation...");
            await Navigation.PushAsync(page, false);
            
            AppLogger.Log("===== BtnVerbalMemoryClicked END =====");
        }
        catch (Exception ex)
        {
            AppLogger.Log($"EXCEPTION in BtnVerbalMemoryClicked: {ex.Message}");
            AppLogger.Log($"Stack trace: {ex.StackTrace}");
            await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
        }
    }

    public async void BtnNonVerbalMemoryClicked(object sender, EventArgs args)
    {
        try
        {
            AppLogger.Log("===== BtnNonVerbalMemoryClicked START =====");
            
            SessionTry.Set("isVerbal", false);
            SessionTry.Set("isNonVerbal", true);

            var quantityForReal = _settings?.GetVerbalMemoryTestWordsQuantity(true) ?? ushort.MinValue;
            var quantityForDemo = _settings?.GetVerbalMemoryTestWordsQuantity(false) ?? ushort.MinValue;
            var objType = AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL;

            AppLogger.Log($"Quantities - Real: {quantityForReal}, Demo: {quantityForDemo}");
            (SpanLine[] lines, string buttonText) = StartTestInitialTextsProvider.GetNonVerbalMemoryStartPageInfo(quantityForDemo);

            AppLogger.Log("Creating TestIntroducingPage...");
            var page = new TestIntroducingPage(
                lines, 
                buttonText,
                objType,
                quantityForDemo
            );
            
            AppLogger.Log("Pushing page to navigation...");
            await Navigation.PushAsync(page, true);
            
            AppLogger.Log("===== BtnNonVerbalMemoryClicked END =====");
        }
        catch (Exception ex)
        {
            AppLogger.Log($"EXCEPTION in BtnNonVerbalMemoryClicked: {ex.Message}");
            AppLogger.Log($"Stack trace: {ex.StackTrace}");
            await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
        }
    }

    private async void BtnAttentionDemo_Clicked(object sender, EventArgs e) => await OpenAttentionTestPage(true);

    private async void BtnAttention_Clicked(object sender, EventArgs e) => await OpenAttentionTestPage(false);
}