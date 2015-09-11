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

    private void KeyboardHook_KeyDown(object sender, KeyEventArgs keyEvt)
    {
      switch (keyEvt.Key)
      {
      }
    }

    private void KeyboardHook_KeyUp(object sender, KeyEventArgs keyEvt)
    {
      switch (keyEvt.Key)
      {
      }
    }

    void MouseHook_ButtonDown(object sender, MouseEventArgs mouseEvt)
    {
      switch (mouseEvt.Button)
      {
        case 4:
          this.Unmute();
          break;
      }
    }

    void MouseHook_ButtonUp(object sender, MouseEventArgs mouseEvt)
    {
      switch (mouseEvt.Button)
      {
        case 4:
          this.DeferMute();
          break;
      }
    }

    private void DeferMute()
    {
      this.timer.Stop();
      this.timer.Start();
    }

    private void Unmute()
    {
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
