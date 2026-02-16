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
            Application.Current.MainPage = new NavigationPage(new MainPage());
        });
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
    private void OnF10Pressed() => NavigateToHome();
#endif

    private void BtnAnswer_Clicked(object sender, EventArgs args)
    {
        var button = ((Button)sender);
        var answerCode = Convert.ToByte(button.CommandParameter);

        switch (answerCode)
        {
            case 1:
                btnWord1.FontAttributes = FontAttributes.Bold;
                btnWord2.FontAttributes = FontAttributes.None;
                break;

            case 2:
                btnWord1.FontAttributes = FontAttributes.None;
                btnWord2.FontAttributes = FontAttributes.Bold;
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

                Application.Current.MainPage = new NavigationPage(new TestIntroducingPage(
                        lines,
                        buttonText,
                        AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL,
                        _objectQuantityForReal
                    ));
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new FinalTestMessagePage());
            }
        }
    }
}