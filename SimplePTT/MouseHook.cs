using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SimplePTT
{
  public class MouseEventArgs : EventArgs
  {
    public int Button { get; }

    public MouseEventArgs(int button)
    {
      Button = button;
    }
  }

  public class MouseHook : IDisposable
  {
    [StructLayout(LayoutKind.Sequential)]
    private class POINT
    {
      public int X;
      public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private class MouseLLHookStruct
    {
      public POINT Point;
      public int MouseData;
      public int Flags;
      public int Time;
      public int DWExtraInfo;
    }

    private static int HiWord(int Number)
    {
      return (ushort)((Number >> 16) & 0xffff);
    }

    private const int WH_MOUSE_LL = 14;

    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_XBUTTONDOWN = 0x020B;

    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_RBUTTONUP = 0x0205;
    private const int WM_MBUTTONUP = 0x0208;
    private const int WM_XBUTTONUP = 0x020C;

    private const int XBUTTON1 = 1;
    private const int XBUTTON2 = 2;

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    public event EventHandler<MouseEventArgs> ButtonDown;
    public event EventHandler<MouseEventArgs> ButtonUp;
    public bool ConsumeMouseKey;

    private readonly LowLevelMouseProc mouseProc;
    private readonly IntPtr hookId;

    public MouseHook()
    {
      mouseProc = HookCallback;
      hookId = SetHook(mouseProc);
    }

    public void Dispose()
    {
      UnhookWindowsHookEx(hookId);
    }

    private void OnButtonDown(int button)
    {
      if (ButtonDown != null)
        ButtonDown.Invoke(null, new MouseEventArgs(button));
    }

    private void OnButtonUp(int button)
    {
      if (ButtonUp != null)
        ButtonUp.Invoke(null, new MouseEventArgs(button));
    }

    private IntPtr SetHook(LowLevelMouseProc proc)
    {
      using (Process curProcess = Process.GetCurrentProcess())
      using (ProcessModule curModule = curProcess.MainModule)
        return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
      if (nCode < 0)
        return CallNextHookEx(hookId, nCode, wParam, lParam);

      ConsumeMouseKey = false;
      var wParamLong = (long)wParam;

      switch (wParamLong)
      {
        case WM_LBUTTONDOWN:
          OnButtonDown(1);
          break;
        case WM_LBUTTONUP:
          OnButtonUp(1);
          break;
        case WM_RBUTTONDOWN:
          OnButtonDown(2);
          break;
        case WM_RBUTTONUP:
          OnButtonUp(2);
          break;
        case WM_MBUTTONDOWN:
          OnButtonDown(3);
          break;
        case WM_MBUTTONUP:
          OnButtonUp(3);
          break;
        case WM_XBUTTONDOWN:
          TriggerSideButtonEvent(lParam, OnButtonDown);
          ConsumeMouseKey = true;
          break;
        case WM_XBUTTONUP:
          TriggerSideButtonEvent(lParam, OnButtonUp);
          break;
      }

      if (ConsumeMouseKey)
        return (IntPtr)1;

      return CallNextHookEx(hookId, nCode, wParam, lParam);
    }

    private void TriggerSideButtonEvent(IntPtr lParam, Action<int> target)
    {
      var mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

      switch (HiWord(mouseHookStruct.MouseData))
      {
        case XBUTTON1:
          target(4);
          break;
        case XBUTTON2:
          target(5);
          break;
      }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
  }
}
