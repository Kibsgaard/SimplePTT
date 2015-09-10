using System;
using System.Linq;
using System.Windows.Interop;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;

namespace ToggleMic
{
  public class KeyboardHook : IDisposable
  {
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;

    private delegate IntPtr LowLevelKeyboardProc(
      int nCode,
      IntPtr wParam,
      IntPtr lParam);

    public event EventHandler<KeyEventArgs> KeyPressed;
    public event EventHandler<KeyEventArgs> KeyReleased;

    private LowLevelKeyboardProc keyboardProc;
    private IntPtr hookId = IntPtr.Zero;

    public KeyboardHook()
    {
      this.keyboardProc = this.HookCallback;
      this.hookId = this.SetHook(this.keyboardProc);
    }

    public void Dispose()
    {
      KeyboardHook.UnhookWindowsHookEx(this.hookId);
    }

    private void OnKeyPressed(KeyEventArgs e)
    {
      if (this.KeyPressed != null)
        this.KeyPressed.Invoke(null, e);
    }

    private void OnKeyReleased(KeyEventArgs e)
    {
      if (this.KeyReleased != null)
        this.KeyReleased.Invoke(null, e);
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
      using (Process curProcess = Process.GetCurrentProcess())
      {
        using (ProcessModule curModule = curProcess.MainModule)
        {
          return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
      }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
      if (nCode >= 0)
      {
        bool keyDown = (wParam == (IntPtr)WM_KEYDOWN);
        bool keyUp = (wParam == (IntPtr)WM_KEYUP);

        if (keyDown || keyUp)
        {
          int vkCode = Marshal.ReadInt32(lParam);
          Key key = KeyInterop.KeyFromVirtualKey(vkCode);

          if (keyDown)
          {
            OnKeyPressed(new KeyEventArgs(key, vkCode));
          }
          else
          {
            OnKeyReleased(new KeyEventArgs(key, vkCode));
          }
        }
      }

      return CallNextHookEx(hookId, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
  }
}
