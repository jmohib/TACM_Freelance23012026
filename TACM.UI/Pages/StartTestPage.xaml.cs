using TACM.Core;
#if WINDOWS
using TACM.UI.Platforms.Windows;
#endif

namespace TACM.UI.Pages;

public partial class StartTestPage : ContentPage
{
    private readonly ushort _objectQuantity;
    private readonly string _objectType;

    public StartTestPage(ushort objectQuantity, string objectType)
    {
        InitializeComponent();

        _objectQuantity = objectQuantity;
        _objectType = objectType;

        BuildStartPageSpanText();

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
                Text = $"You're now ready to begin. Try to remember the following {_objectQuantity} {_objectType}.",
                TextColor = Colors.White
            },

            new Span() { Text = Environment.NewLine },
            new Span() { Text = "Click ", TextColor = Colors.White },
            new Span() { Text = "NEXT ", TextColor = Colors.LightSkyBlue, FontAttributes = FontAttributes.Bold },
            new Span() { Text = "to begin.", TextColor = Colors.White }
        ];

        lblStartTestText.FormattedText.Spans.Clear();

        foreach (var line in spanLines)
            lblStartTestText.FormattedText.Spans.Add(line);
    }

    public void BtnStartTestClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new RememberFollowingObjectsPage(_objectQuantity, _objectType));
    }
}
