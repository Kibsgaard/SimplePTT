using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Timers;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace SimplePTT
{
  public class MicController : IDisposable
  {
    public const int MUTE_DELAY = 300;

    public bool IsMuted { get; private set; }

    private CoreAudioController controller;
    private KeyboardHook keyboardHook;
    private MouseHook mouseHook;

    private Timer timer;
    private Object muteLock;

    public MicController()
    {
      this.controller = new CoreAudioController();

      this.muteLock = new Object();
      this.timer = new Timer(MUTE_DELAY);
      this.timer.AutoReset = false;
      this.timer.Elapsed += this.Timer_Elapsed;

      this.keyboardHook = new KeyboardHook();
      this.keyboardHook.KeyDown += KeyboardHook_KeyDown;
      this.keyboardHook.KeyUp += KeyboardHook_KeyUp;

      this.mouseHook = new MouseHook();
      this.mouseHook.ButtonDown += MouseHook_ButtonDown;
      this.mouseHook.ButtonUp += MouseHook_ButtonUp;

      this.Mute(true);
    }

    void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.Mute(true);
    }

    public void Dispose()
    {
      this.Mute(false);
      this.keyboardHook.Dispose();
      this.mouseHook.Dispose();
    }

    private bool IsValidButton(int button)
    {
      switch (button)
      {
        case 4:
          return true;
      }

      return false;
    }

    private bool IsValidKey(KeyEventArgs keyEvt)
    {
      switch (keyEvt.Key)
      {
        case Key.CapsLock:
          return true;

        case Key.Z:
          return true;

        case Key.Add:
          return true;

        case Key.Subtract:
          return true;

        case Key.Multiply:
          return true;

        case Key.Divide:
          return true;

        case Key.Decimal:
          return true;

        case Key.Enter:
          return true;

        case Key.NumPad0:
          return true;

        case Key.NumPad1:
          return true;

        case Key.NumPad2:
          return true;

        case Key.NumPad3:
          return true;

        case Key.NumPad4:
          return true;

        case Key.NumPad5:
          return true;

        case Key.NumPad6:
          return true;

        case Key.NumPad7:
          return true;

        case Key.NumPad8:
          return true;

        case Key.NumPad9:
          return true;

        case Key.Scroll:
          return true;
      }

      return false;
    }

    private void KeyboardHook_KeyDown(object sender, KeyEventArgs keyEvt)
    {
      if (IsValidKey(keyEvt))
        this.Unmute();
    }

    private void KeyboardHook_KeyUp(object sender, KeyEventArgs keyEvt)
    {
      if (IsValidKey(keyEvt))
          this.DeferMute();
    }

    void MouseHook_ButtonDown(object sender, MouseEventArgs mouseEvt)
    {
      if (IsValidButton(mouseEvt.Button))
          this.Unmute();
    }

    void MouseHook_ButtonUp(object sender, MouseEventArgs mouseEvt)
    {
      if (IsValidButton(mouseEvt.Button))
        this.DeferMute();
    }

    private void DeferMute()
    {
      this.timer.Stop();
      this.timer.Start();
    }

    private void Unmute()
    {
      this.timer.Stop();
      this.Mute(false);
    }

    private void Mute(bool mute)
    {
      lock (this.muteLock)
      {
        this.GetActiveDevice().Mute(mute);
        this.IsMuted = mute;
      }
    }

    private CoreAudioDevice GetActiveDevice()
    {
      return
        this.controller.GetCaptureDevices(DeviceState.Active)
          .FirstOrDefault(o => o.IsDefaultDevice);
    }
  }
}
