using TACM.Core;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif

namespace TACM.UI.Pages;

public partial class RememberFollowingObjectsPage : ContentPage
{
    private readonly ushort _objectQuantity;
    private readonly string _objectType;

    private readonly IDictionary<string, ContentPage> _pageToRedirectRegardingObjectType;

    public RememberFollowingObjectsPage(ushort objectQuantity, string objectType)
    {
        InitializeComponent();

        _objectQuantity = objectQuantity;
        _objectType = objectType;

        _pageToRedirectRegardingObjectType = new Dictionary<string, ContentPage>()
        {
            { AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL, new ShowWordsToMemorizePage(_objectQuantity) },
            { AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL, new ShowPicturesToMemorizePage(_objectQuantity) }
        };

        BuildStartPageSpanText();

#if MACCATALYST
        SetupMacShortcuts();
#endif
    }

#if MACCATALYST
    private void SetupMacShortcuts()
    {
        // Register Command + E for macOS
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

    private void BuildStartPageSpanText()
    {
        Span[] spanLines = [
            new Span()
            {
                Text = $"Remember the following {_objectQuantity} {_objectType}.",
                TextColor = Colors.White
            },

            new Span() { Text = Environment.NewLine },
            new Span() { Text = "Click ", TextColor = Colors.White },
            new Span() { Text = "NEXT ", TextColor = Colors.LightSkyBlue, FontAttributes = FontAttributes.Bold },
            new Span() { Text = "to begin.", TextColor = Colors.White }
        ];

        lblRememberText.FormattedText.Spans.Clear();

        foreach (var line in spanLines)
            lblRememberText.FormattedText.Spans.Add(line);
    }

    public void BtnStartPage(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(_pageToRedirectRegardingObjectType[_objectType]);
    }
}
