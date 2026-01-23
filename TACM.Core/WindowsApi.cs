using System.Runtime.InteropServices;
using System.Text;

namespace TACM.Core;

public class WindowsApi
{
    public const int WH_MIN = (-1);
    public const int WH_MSGFILTER = (-1);
    public const int WH_JOURNALRECORD = 0;
    public const int WH_JOURNALPLAYBACK = 1;
    public const int WH_KEYBOARD = 2;
    public const int WH_GETMESSAGE = 3;
    public const int WH_CALLWNDPROC = 4;
    public const int WH_CBT = 5;
    public const int WH_SYSMSGFILTER = 6;
    public const int WH_MOUSE = 7;
    public const int WH_HARDWARE = 8;
    public const int WH_DEBUG = 9;
    public const int WH_SHELL = 10;
    public const int WH_FOREGROUNDIDLE = 11;
    public const int WH_CALLWNDPROCRET = 12;
    public const int WH_KEYBOARD_LL = 13;
    public const int WH_MOUSE_LL = 14;
    public const int WH_MAX = 14;
    public const int WH_MINHOOK = WH_MIN;
    public const int WH_MAXHOOK = WH_MAX;

    public const int KF_EXTENDED = 0x0100;
    public const int KF_DLGMODE = 0x0800;
    public const int KF_MENUMODE = 0x1000;
    public const int KF_ALTDOWN = 0x2000;
    public const int KF_REPEAT = 0x4000;
    public const int KF_UP = 0x8000;

    public delegate void HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern bool UnhookWindowsHookEx(int idHook);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern int CallNextHoookEx(int idHook, int nCode, IntPtr wParam, int lParam);

    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    public static extern int MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    public static extern int ToUnicode(
        uint virtualKeyCode,
        uint scanCode,
        byte[] keyboardState,
        [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder receivingBuffer,
        int bufferSize,
        uint flags
    );

    internal static int HIWORD(IntPtr wParam)
    {
        return (int)((wParam.ToInt64() >> 16) & 0xffff);
    }

    internal static int LOWORD(IntPtr wParam)
    {
        return (int)(wParam.ToInt64() & 0xffff);
    }
}
