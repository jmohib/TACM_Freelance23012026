using TACM.Core;
using TACM.Entities;
using TACM.UI.Utils;
using TACM.UI.ViewModels;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif

namespace TACM.UI.Pages;

public partial class WordMemoryTestPage : ContentPage
{
    private Settings? _settings;
    private bool _isDemo = false;	
	private ushort _objectQuantityForReal;
    private readonly ushort _objectQuantity;

    private WordTestMemoryViewModel ViewModel { get; set; }


	public WordMemoryTestPage(ushort objectQuantity, string[] correctAnswers)
	{
		InitializeComponent();

        _objectQuantity = objectQuantity;

        ViewModel = new WordTestMemoryViewModel(objectQuantity, correctAnswers);
		BindingContext = ViewModel;
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();

		_settings = await ViewModel.GetActiveSettingsAsync();

		ViewModel.FontSize = _settings?.FontSize ?? AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
		_objectQuantityForReal = (_settings?.GetVerbalMemoryTestWordsQuantity(true) ?? ushort.MinValue);
        _isDemo = _objectQuantity < _objectQuantityForReal;

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

		var button = ((Button)sender);
        var answerCode = Convert.ToByte(button.CommandParameter);
        
		switch(answerCode)
		{
			case 1:
				btnWord1.FontAttributes = FontAttributes.Bold;
               // btnWord1.BackgroundColor = Color.FromArgb(color);

				btnWord2.FontAttributes = FontAttributes.None;
                //btnWord2.BackgroundColor = Color.FromArgb(otherColor);
                break;

			case 2:
                btnWord1.FontAttributes = FontAttributes.None;
               // btnWord1.BackgroundColor = Color.FromArgb(otherColor);

				btnWord2.FontAttributes = FontAttributes.Bold;
               // btnWord2.BackgroundColor = Color.FromArgb(color);
                break;
		}
        btnWord1.IsEnabled = false;
        btnWord2.IsEnabled = false;
	}

    private async void BtnNextWords_Clicked(object sender, EventArgs e)
    {
        btnWord1.IsEnabled = true;
        btnWord2.IsEnabled = true;

        btnWord1.FontAttributes = FontAttributes.None;
        btnWord1.BackgroundColor = Color.FromArgb(AppConstants.DEFAULT_ANSWER_BG_COLOR);

        btnWord2.FontAttributes = FontAttributes.None;
        btnWord2.BackgroundColor = Color.FromArgb(AppConstants.DEFAULT_ANSWER_BG_COLOR);

        if (ViewModel.AllAnswersPairsWereVisited)
        {
            if (_isDemo)
            {
                (SpanLine[] lines, string buttonText) = StartTestInitialTextsProvider.GetVerbalMemoryStartPageInfo(_objectQuantityForReal);

                //await Navigation.PushAsync(
                //    new TestIntroducingPage(
                //        lines,
                //        buttonText,
                //        AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL,
                //        _objectQuantityForReal
                //    ),

                //    true
                //);
                Application.Current.MainPage = new NavigationPage(new TestIntroducingPage(
                        lines,
                        buttonText,
                        AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL,
                        _objectQuantityForReal
                    ));
            }
            else
            {
               // await Navigation.PushAsync(new FinalTestMessagePage());
                Application.Current.MainPage = new NavigationPage(new FinalTestMessagePage());
            }
        }
    }
}