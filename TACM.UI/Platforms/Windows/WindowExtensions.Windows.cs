
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace TACM.UI.Platforms.Windows;
public static partial class WindowExtensions
{
    public static partial void Maximize(Microsoft.UI.Xaml.Window window); // Defining declaration
}
public static partial class WindowExtensions
{
    public static partial void Maximize(Microsoft.UI.Xaml.Window window)
    {
        var nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        if (appWindow != null)
        {
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen); // Corrected line
        }
    }
}
#endif