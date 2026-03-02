using TACM.UI.Pages;

namespace TACM.UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
#if MACCATALYST
    private void SetupMacShortcuts()
    {
        // Try adding the menu to the Application level if Page level fails
        var exitMenu = new MenuFlyoutItem { 
            Text = "Exit to Home",
            Command = new Command(NavigateToHome)
        };
        
        exitMenu.KeyboardAccelerators.Add(new KeyboardAccelerator {
            Modifiers = KeyboardAcceleratorModifiers.Cmd,
            Key = "e"
        });

        // Use 'FlyoutBase' if MenuBarItems isn't appearing
        var menu = new MenuBarItem { Text = "Debug Tools" };
        menu.Add(exitMenu);

        if (!MenuBarItems.Contains(menu))
        {
            MenuBarItems.Add(menu);
        }
    }

#endif
        private void NavigateToHome()
        {
           // Debug.WriteLine("NavigateToHome triggered!"); // Look for this in your Output window
            MainThread.BeginInvokeOnMainThread(() => {
                Application.Current.MainPage = new NavigationPage(new MainPage());
            });
        }
    }
}
