using TACM.UI.ViewModels;
using TACM.UI.Utils;

namespace TACM.UI.Pages;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel ViewModel;

	public SettingsPage()
	{
		InitializeComponent();
		Shell.SetNavBarIsVisible(this, false);
		ViewModel = new SettingsViewModel();
		BindingContext = ViewModel;
       
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

		await ViewModel.TryLoadCurrentSettingsAsync();
    }

    public async void BtnExitClicked(object sender, EventArgs args)
	{
		try
		{
			AppLogger.Log("Exit button clicked in Settings page");
			
			// Ensure any pending UI updates complete
			await Task.Delay(50);
			
			// Try the most reliable navigation method first
			if (Navigation != null && Navigation.NavigationStack.Count > 1)
			{
				AppLogger.Log($"Navigation stack has {Navigation.NavigationStack.Count} pages, popping...");
				await Navigation.PopAsync(animated: true);
				AppLogger.Log("Successfully popped from navigation stack");
			}
			else
			{
				AppLogger.Log("Using Shell.Current.GoToAsync fallback");
				await Shell.Current.GoToAsync("..", animate: true);
				AppLogger.Log("Successfully navigated using Shell");
			}
		}
		catch (Exception ex)
		{
			AppLogger.Log($"ERROR in Exit button: {ex.Message}\nStack: {ex.StackTrace}");
			
			// Ultimate fallback for macOS - navigate to MainPage directly
			try
			{
				AppLogger.Log("Attempting ultimate fallback navigation");
				await Shell.Current.GoToAsync("//MainPage");
				AppLogger.Log("Fallback navigation succeeded");
			}
			catch (Exception ex2)
			{
				AppLogger.Log($"All navigation attempts failed: {ex2.Message}");
				await DisplayAlert("Navigation Error", "Unable to return to main page. Please restart the application.", "OK");
			}
		}
	}
}