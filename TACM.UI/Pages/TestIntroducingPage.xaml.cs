using TACM.Core;
using TACM.UI.Utils;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif

namespace TACM.UI.Pages;

public partial class TestIntroducingPage : ContentPage
{
    private readonly string _objectType;
    private readonly ushort _objectQuantity;
    private readonly IDictionary<string, dynamic>? _options;
    private readonly IDictionary<string, ContentPage> _pageToRedirectRegardingObjectType;

    public TestIntroducingPage(
        SpanLine[] testIntroducingTextLines,
        string? startTestButtonText,
        string objectType,
        ushort objectQuantity,
        IDictionary<string, dynamic>? options = null
    )
    {
        InitializeComponent();

        _objectType = objectType;
        _objectQuantity = objectQuantity;
        _options = options;

        BtnStartTest.Text = startTestButtonText ?? "Start Test";

        if (_objectType != "attention")
        {
            _pageToRedirectRegardingObjectType = new Dictionary<string, ContentPage>()
            {
                { AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL, new ShowWordsToMemorizePage(_objectQuantity) },
                { AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL, new ShowPicturesToMemorizePage(_objectQuantity) }
            };
        }

        BuildTestIntroducingMessage(ref testIntroducingTextLines);

#if MACCATALYST
        SetupMacShortcuts();
#endif
    }

#if MACCATALYST
    private void SetupMacShortcuts()
    {
        // Cmd + E -> Exit to Home
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

    protected override void OnAppearing()
    {
        base.OnAppearing();

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

    private void BuildTestIntroducingMessage(ref readonly SpanLine[] lines)
    {
        LabelTestIntrodingFormattedStringMessage.Spans.Clear();

        foreach (SpanLine line in lines)
        {
            LabelTestIntrodingFormattedStringMessage.Spans.Add(new Span
            {
                Text = line.Text,
                TextColor = line.TextColor ?? Colors.White,
                FontAttributes = line.FontAttributes
            });
        }
    }

    public void BtnStartTestClicked(object sender, EventArgs e)
    {
        ContentPage page = _objectType switch
        {
            AppConstants.OBJECT_TYPE_ATTENTION_TEST => new AttentionTestPage(_objectQuantity, _options?["demo"] ?? false, _options),
            _ => new StartTestPage(_objectQuantity, _objectType)
        };

        if (_objectType != "attention")
        {
            Application.Current.MainPage = new NavigationPage(_pageToRedirectRegardingObjectType[_objectType]);
        }
        else
        {
            Application.Current.MainPage = new NavigationPage(page);
        }
    }
}