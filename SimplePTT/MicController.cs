using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace SimplePTT
{
  public class MicController : IDisposable
  {
    public bool IsMuted { get; private set; }

    private CoreAudioController controller;
    private KeyboardHook keyboardHook;
    private MouseHook mouseHook;

    public MicController()
    {
      this.controller = new CoreAudioController();

      this.keyboardHook = new KeyboardHook();
      this.keyboardHook.KeyDown += KeyboardHook_KeyDown;
      this.keyboardHook.KeyUp += KeyboardHook_KeyUp;

      this.mouseHook = new MouseHook();
      this.mouseHook.ButtonDown += MouseHook_ButtonDown;
      this.mouseHook.ButtonUp += MouseHook_ButtonUp;

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
        case Key.Scroll:
          this.Mute(false);
          break;
      }
    }

    private void KeyboardHook_KeyUp(object sender, KeyEventArgs keyEvt)
    {
      switch (keyEvt.Key)
      {
        case Key.Scroll:
          this.Mute(true);
          break;
      }
    }

    void MouseHook_ButtonDown(object sender, MouseEventArgs mouseEvt)
    {
      switch (mouseEvt.Button)
      {
        case 4:
          this.Mute(false);
          break;
      }
    }

    void MouseHook_ButtonUp(object sender, MouseEventArgs mouseEvt)
    {
      switch (mouseEvt.Button)
      {
        case 4:
          this.Mute(true);
          break;
      }
    }

    private void Mute(bool mute)
    {
      this.GetActiveDevice().Mute(mute);
      this.IsMuted = mute;
    }

    private CoreAudioDevice GetActiveDevice()
    {
      return
        this.controller.GetCaptureDevices(DeviceState.Active)
          .FirstOrDefault(o => o.IsDefaultDevice);
    }
  }
}
