using System.Diagnostics;
using TACM.Core;
using TACM.Entities;
using TACM.UI.ViewModels;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif
namespace TACM.UI.Pages;

public partial class AttentionTestPage : ContentPage, IPageKeyEventHandler
{
    private DateTime _controlTime = DateTime.UtcNow;
    private Settings? _settings;

    private readonly ushort _objectQuantity;
    private readonly bool _isDemo = false;
    private readonly IDictionary<string, dynamic>? _options = null;
    private AttentionTestViewModel ViewModel { get; set; }


    public AttentionTestPage(
        ushort objectQuantity, 
        bool isDemo, 
        IDictionary<string, dynamic>? options = null
    )
	{
		InitializeComponent();

        _objectQuantity = objectQuantity;
        _isDemo = isDemo;
        _options = options;

        ViewModel = new AttentionTestViewModel(
            target: (char)_options?["target"],
            trialsCount: isDemo ? (ushort)(objectQuantity / 2) : objectQuantity,
            isDemo: _isDemo
        );

        BindingContext = ViewModel;        
    }    


    private ushort GetInterval()
    {
        switch (ViewModel.CurrentTrialNumber)
        {
            case 1:
                return _settings?.T1 ?? AppConstants.DEFAULT_T1;

            case 2:
                if (_isDemo)
                    return 0;

                return _settings?.T2 ?? AppConstants.DEFAULT_T2;

            case 3:
                if (_isDemo)
                    return 0;

                return _settings?.T3 ?? AppConstants.DEFAULT_T3;

            case 4:
                if (_isDemo)
                    return 0;

                return _settings?.T4 ?? AppConstants.DEFAULT_T4;
            default:
                return 0;
        }
    }

    protected override bool OnBackButtonPressed() => true;

    public void OnKeyPressed(char keyCode)
    {
        if (keyCode == ' ')
        {
            Debug.WriteLine("Space bar pressed.");

            ViewModel.RegisterAnswer(false);
            ViewModel.CanShowLetter = false;
        }
    }

    
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _settings = await ViewModel.GetActiveSettingsAsync();
        ViewModel.FontSize = _settings?.FontSize ?? AppConstants.DEFAULT_WORD_TEST_FONTSIZE;

        _controlTime = DateTime.UtcNow;

        // Wait 2 seconds before showing the first letter
        await Task.Delay(2000);

        while (true)
        {
            // Check if we should break the loop (when interval is 0)
            var interval = GetInterval();
            ViewModel.CanShowButtonNext = interval == 0;
            if (interval == 0)
                break;

            // Show the letter for 750ms
            ViewModel.DrawNewLetter();
            ViewModel.CanShowLetter = true;
            await Task.Delay(750);

            // Hide the letter for 2000ms
            ViewModel.CanShowLetter = false;

            // Check if answer should be registered during the off period
            if (!ViewModel.Answered && ViewModel.CurrentLetter == ViewModel.Target)
            {
                ViewModel.RegisterAnswer(true);
            }

            await Task.Delay(interval);

            // Update control time for interval tracking
            _controlTime = DateTime.UtcNow;
        }
#if WINDOWS
        KeyboardHook.F10Pressed += OnF10Pressed;
        KeyboardHook.Start();
#endif
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

#if WINDOWS
        KeyboardHook.F10Pressed -= OnF10Pressed;
        KeyboardHook.Stop();
#endif
    }
#if WINDOWS
    private void OnF10Pressed()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
        });
    }
#endif

    private async void BtnNext_Clicked(object sender, EventArgs e)
    {
        //await Task.WhenAll([
        //    ViewModel.SaveTestResultsAsync(),
        //    Navigation.PushAsync(new FinalTestMessagePage())
        //]);
        ViewModel.SaveTestResultsAsync();
        Application.Current.MainPage = new NavigationPage(new FinalTestMessagePage());
    }
}