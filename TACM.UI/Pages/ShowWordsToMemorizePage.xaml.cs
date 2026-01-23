using TACM.Core;
using TACM.UI.ViewModels;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif
namespace TACM.UI.Pages;

public partial class ShowWordsToMemorizePage : ContentPage
{
    private readonly ushort _objectQuantity;
    private System.Timers.Timer _timer;
    private bool _isWordVisible = true;
    private ShowWordsToMemorizeViewModel ViewModel {  get; set; }	



	public ShowWordsToMemorizePage(ushort objectQuantity)
	{
		InitializeComponent();

        _objectQuantity = objectQuantity;

        ViewModel = new ShowWordsToMemorizeViewModel(objectQuantity);
		BindingContext = ViewModel;
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var settings = await ViewModel.GetActiveSettingsAsync();
        ViewModel.FontSize = settings?.FontSize ?? AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
        ViewModel.CanShowButtonNext = false;

        ViewModel.ToggleRandomDrawnWords(); // Show first word immediately
        _isWordVisible = true;

        _timer = new System.Timers.Timer(TimeSpan.FromSeconds(AppConstants.SECONDS_TO_STAY_WORDS).TotalMilliseconds);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();
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

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_isWordVisible)
            {
                ViewModel.HideCurrentWord(); // You need to implement this to hide the word
                _isWordVisible = false;
                _timer.Interval = TimeSpan.FromSeconds(0.5).TotalMilliseconds; // Gap before next word
            }
            else
            {
                if (ViewModel.ToggleRandomDrawnWords()) // Show next word
                {
                    _isWordVisible = true;
                    _timer.Interval = TimeSpan.FromSeconds(AppConstants.SECONDS_TO_STAY_WORDS).TotalMilliseconds;
                }
                else
                {
                    _timer.Stop();
                    ViewModel.CanShowButtonNext = true;
                    ViewModel.ShowStartText = true;
                    ViewModel.CanShowWord = false;
                }
            }
        });
    }


    private async void BtnNext_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new WordMemoryTestPage(_objectQuantity, ViewModel.RandomDrawWords));
        Application.Current.MainPage = new NavigationPage(new WordMemoryTestPage(_objectQuantity, ViewModel.RandomDrawWords));
    }
}