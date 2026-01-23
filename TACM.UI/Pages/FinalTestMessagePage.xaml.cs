using TACM.UI.Models;
using TACM.UI.ViewModels;
using TACM.Core;
using TACM.UI.Utils;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif

namespace TACM.UI.Pages;

public partial class FinalTestMessagePage : ContentPage
{
    private Entities.Settings? _settings;
    private readonly MainPageViewModel ViewModel;
    private WordTestMemoryViewModel ViewModel1 { get; set; }
    public  FinalTestMessagePage()
	{
		InitializeComponent();
        ViewModel = new MainPageViewModel();
        ViewModel1 = new WordTestMemoryViewModel(25, Array.Empty<string>());
        GetCorrectAnswersCount();

        
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _settings = await ViewModel.GetActiveSettingsAsync();

#if WINDOWS
        KeyboardHook.F10Pressed += OnF10Pressed;
        KeyboardHook.F12Pressed += OnF12Pressed;
        KeyboardHook.Start();
#endif
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

#if WINDOWS
        KeyboardHook.F10Pressed -= OnF10Pressed;
        KeyboardHook.F12Pressed -= OnF12Pressed;
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

    private void OnF12Pressed()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
        var quantityForReal = _settings?.GetVerbalMemoryTestWordsQuantity(true) ?? ushort.MinValue;
        var quantityForDemo = _settings?.GetVerbalMemoryTestWordsQuantity(false) ?? ushort.MinValue;
        var objType = AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL;
        SpanLine[] lines = Array.Empty<SpanLine>();
        string buttonText = string.Empty;
        
        if(SessionTry.Get<bool>("isVerbal"))
        {
            objType = AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL;
            var result = StartTestInitialTextsProvider.GetVerbalMemoryStartPageInfo(quantityForReal);

            lines = result.lines;
            buttonText = result.buttonText;
        }
        else if(SessionTry.Get<bool>("isNonVerbal"))
        {
            objType = AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL;
            var result = StartTestInitialTextsProvider.GetNonVerbalMemoryStartPageInfo(quantityForReal);

            lines = result.lines;
            buttonText = result.buttonText;
        }

        Application.Current.MainPage = new NavigationPage(new TestIntroducingPage(
                lines,
                buttonText,
                objType,
                quantityForReal
            ));
            
        });
    }
#endif

    private async Task GetCorrectAnswersCount()
    {
        int count = await ViewModel1.GetCorrectAnswerCount();
        int testAttempt = 0;
        if (SessionTry.Get<string>("CurrentTestType") == "verbal")
        {
            if (SessionTry.Get<bool>("isFirstVerbalCompleted"))
            {
                testAttempt = 1;
            }
            else if (SessionTry.Get<bool>("isSecondVerbalCompleted"))
            {
                testAttempt = 2;
            }
        }
        else if (SessionTry.Get<string>("CurrentTestType") == "non-verbal")
        {
            if (SessionTry.Get<bool>("isFirstNonVerbalCompleted"))
            {
                testAttempt = 1;
            }
            else if (SessionTry.Get<bool>("isSecondNonVerbalCompleted"))
            {
                testAttempt = 2;
            }
        }
        else if (SessionTry.Get<string>("CurrentTestType") == "attention")
        {
            testAttempt = 1;
            count = 8;
        }

            sp_text.Text = "You are done. Please call the examiner. T" + testAttempt.ToString() + " : " + count.ToString();
    }

    public async void BtnBackToHome_Clicked(object sender, EventArgs e)
	{
        //await Navigation.PushAsync(new MainPage());
        ///Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}