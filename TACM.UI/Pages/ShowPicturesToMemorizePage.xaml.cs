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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var settings = await ViewModel.GetActiveSettingsAsync();

        ViewModel.FontSize = settings?.FontSize ?? AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
        ViewModel.CanShowButtonNext = false;

        //var timer = new System.Timers.Timer(TimeSpan.FromSeconds(AppConstants.SECONDS_BETWEEN_WORDS));

        //timer.Elapsed += Timer_Elapsed;
        //timer.Start();
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
        //if (ViewModel.ToggleRandomDrawnPictures())
        //    return;

        //(sender as System.Timers.Timer)?.Stop();
        //ViewModel.CanShowButtonNext = true;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_isPicVisible)
            {
                ViewModel.HideCurrentPicture(); // You need to implement this to hide the word
                _isPicVisible = false;
                _timer.Interval = TimeSpan.FromSeconds(0.5).TotalMilliseconds; // Gap before next word
            }
            else
            {
                if (ViewModel.ToggleSequentialPictures()) // Show next word
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

    private async void btnNext_Clicked(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new PictureMemoryTestPage(_objectQuantity, ViewModel.RandomDrawPictures));
        Application.Current.MainPage = new NavigationPage(new PictureMemoryTestPage(_objectQuantity, ViewModel.RandomDrawPictures));
    }
}