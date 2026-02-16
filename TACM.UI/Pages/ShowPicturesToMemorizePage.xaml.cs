using TACM.Core;
using TACM.UI.ViewModels;
using TACM.UI.Utils;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif

namespace TACM.UI.Pages;

public partial class ShowPicturesToMemorizePage : ContentPage
{
    private readonly ushort _objectQuantity;
    private System.Timers.Timer _timer;
    private bool _isPicVisible = true;
    private ShowPicturesToMemorizeViewModel ViewModel { get; set; }

    public ShowPicturesToMemorizePage(ushort objectQuantity)
    {
        InitializeComponent();

        _objectQuantity = objectQuantity;

        ViewModel = new ShowPicturesToMemorizeViewModel(objectQuantity);
        BindingContext = ViewModel;

#if MACCATALYST
        SetupMacShortcuts();
#endif
    }

#if MACCATALYST
    private void SetupMacShortcuts()
    {
        var exitMenu = new MenuFlyoutItem { Text = "Exit to Home" };
        exitMenu.Command = new Command(NavigateToHome);
        exitMenu.KeyboardAccelerators.Add(new KeyboardAccelerator 
        { 
            Modifiers = KeyboardAcceleratorModifiers.Cmd, 
            Key = "e" 
        });

        var menuBarItem = new MenuBarItem { Text = "Actions" };
        menuBarItem.Add(exitMenu);

        this.MenuBarItems.Add(menuBarItem);
    }
#endif

    private void NavigateToHome()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _timer?.Stop(); // Stop timer before navigating away
            Application.Current.MainPage = new NavigationPage(new MainPage());
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var settings = await ViewModel.GetActiveSettingsAsync();

        ViewModel.FontSize = settings?.FontSize ?? AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
        ViewModel.CanShowButtonNext = false;

        _isPicVisible = true;

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
        _timer?.Stop();

#if WINDOWS
        KeyboardHook.F10Pressed -= OnF10Pressed;
        KeyboardHook.Stop();
#endif
    }

#if WINDOWS
    private void OnF10Pressed() => NavigateToHome();
#endif

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_isPicVisible)
            {
                ViewModel.HideCurrentPicture();
                _isPicVisible = false;
                _timer.Interval = TimeSpan.FromSeconds(0.5).TotalMilliseconds;
            }
            else
            {
                if (ViewModel.ToggleSequentialPictures())
                {
                    _isPicVisible = true;
                    _timer.Interval = TimeSpan.FromSeconds(AppConstants.SECONDS_TO_STAY_WORDS).TotalMilliseconds;
                }
                else
                {
                    _timer.Stop();
                    ViewModel.CanShowPic = false;
                    ViewModel.CanShowButtonNext = true;
                    ViewModel.ShowStartText = true;
                }
            }
        });
    }

    private void btnNext_Clicked(object sender, EventArgs e)
    {
        _timer?.Stop();
        Application.Current.MainPage = new NavigationPage(new PictureMemoryTestPage(_objectQuantity, ViewModel.RandomDrawPictures));
    }
}