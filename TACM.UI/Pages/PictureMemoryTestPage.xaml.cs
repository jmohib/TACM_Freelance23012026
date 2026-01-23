using TACM.Core;
using TACM.UI.Utils;
using TACM.UI.ViewModels;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif
namespace TACM.UI.Pages;

public partial class PictureMemoryTestPage : ContentPage
{
    private bool _isDemo = false;
    private ushort _objectQuantityForReal;
    private readonly ushort _objectQuantity;

    private PictureTestMemoryViewModel ViewModel { get; set; }


    public PictureMemoryTestPage(ushort objectQuantity, string[] correctAnswers)
    {
        InitializeComponent();

        _objectQuantity = objectQuantity;

        ViewModel = new PictureTestMemoryViewModel(objectQuantity, correctAnswers);
        BindingContext = ViewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var settings = await ViewModel.GetActiveSettingsAsync();

        ViewModel.FontSize = settings?.FontSize ?? AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
        _objectQuantityForReal = (settings?.GetNonVerbalMemoryTestWordsQuantity(true) ?? ushort.MinValue);
        _isDemo = _objectQuantity < _objectQuantityForReal;

        if (_isDemo)
        {
            ViewModel.TestTryNumber = 0;
            SessionCollectedData.CollectNonVerbalTestAttemp(0);
        }
        else if(!_isDemo && SessionCollectedData.GetCollectedNonVerbalTestAttempt() == 0)
        {
            ViewModel.TestTryNumber = 1;
            SessionCollectedData.CollectNonVerbalTestAttemp(1);
        }
        else
        {
            ViewModel.TestTryNumber = 2;
            SessionCollectedData.CollectNonVerbalTestAttemp(2);
        }
            //ViewModel.TestTryNumber = _isDemo ? 0 : SessionCollectedData.GetCollectedNonVerbalTestAttempt(); // Or pass it into constructor

        await ViewModel.StartTestAsync(_isDemo);
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

    private void BtnAnswer_Clicked(object sender, EventArgs args)
    {
        var color = ViewModel.AnswerState switch
        {
            AnswerState.Correct => AppConstants.CORRECT_ANSWER_BG_COLOR,
            AnswerState.Incorrect => AppConstants.WRONG_ANSWER_BG_COLOR,
            _ => AppConstants.DEFAULT_ANSWER_BG_COLOR
        };

        var otherColor = ViewModel.AnswerState switch
        {
            AnswerState.Correct => AppConstants.WRONG_ANSWER_BG_COLOR,
            AnswerState.Incorrect => AppConstants.CORRECT_ANSWER_BG_COLOR,
            _ => AppConstants.DEFAULT_ANSWER_BG_COLOR
        };

        var answerCode = Convert.ToByte(((ImageButton)sender).CommandParameter);

        switch (answerCode)
        {
            case 1:
                //  btnPicture1.BackgroundColor = Color.FromArgb(color);
                // btnPicture2.BackgroundColor = Color.FromArgb(otherColor);
                btnPicture1.BorderColor = Colors.Black;          // Border color
                btnPicture1.BorderWidth = 2;                     // Thickness of the border
                btnPicture1.CornerRadius = 10;
                break;
            case 2:
                //  btnPicture1.BackgroundColor = Color.FromArgb(otherColor);
                //   btnPicture2.BackgroundColor = Color.FromArgb(color);
                btnPicture2.BorderColor = Colors.Black;          // Border color
                btnPicture2.BorderWidth = 2;                     // Thickness of the border
                btnPicture2.CornerRadius = 10;
                break;
        }

        btnPicture1.IsEnabled = false;
        btnPicture1.Opacity = 1;
        btnPicture1.InputTransparent = true;
        

        btnPicture2.IsEnabled = false;
        btnPicture2.Opacity = 1;
        btnPicture2.InputTransparent = true;
        
    }

    private async void BtnNextPictures_Clicked(object sender, EventArgs e)
    {
        btnPicture1.IsEnabled = true;
        btnPicture1.InputTransparent = false;
        btnPicture1.BorderWidth = 0;                     // Thickness of the border
        btnPicture1.CornerRadius = 0;

        btnPicture2.IsEnabled = true;
        btnPicture2.InputTransparent = false;
        btnPicture2.BorderWidth = 0;                     // Thickness of the border
        btnPicture2.CornerRadius = 0;

        btnPicture1.BackgroundColor = Color.FromArgb(AppConstants.DEFAULT_ANSWER_BG_COLOR);
        btnPicture2.BackgroundColor = Color.FromArgb(AppConstants.DEFAULT_ANSWER_BG_COLOR);

        if (ViewModel.AllAnswersPairsWereVisited)
        {
            if (_isDemo)
            {
                (SpanLine[] lines, string buttonText) = StartTestInitialTextsProvider.GetNonVerbalMemoryStartPageInfo(_objectQuantityForReal);

                //await Navigation.PushAsync(
                //    new TestIntroducingPage(
                //        lines,
                //        buttonText,
                //        AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL,
                //        _objectQuantityForReal
                //    ),

                //    true
                //);

                Application.Current.MainPage = new NavigationPage(new TestIntroducingPage(
                        lines,
                        buttonText,
                        AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL,
                        _objectQuantityForReal
                    ));
            }
            else
            {
                //await Navigation.PushAsync(new FinalTestMessagePage());
                Application.Current.MainPage = new NavigationPage(new FinalTestMessagePage());
            }
        }
    }
}