using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace ToggleMic
{
  public class MicController : IDisposable
  {
    public bool IsMuted { get; private set; }

    private CoreAudioController controller;
    private KeyboardHook keyboardHook;

    public MicController()
    {
      this.controller = new CoreAudioController();
      this.keyboardHook = new KeyboardHook();

      this.keyboardHook.KeyPressed += KeyboardHook_KeyPressed;
      this.keyboardHook.KeyReleased += KeyboardHook_KeyReleased;
      this.Mute(true);
    }

    public void Dispose()
    {
      this.Mute(false);
    }

    private void KeyboardHook_KeyPressed(
      object sender,
      KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Scroll:
          this.Mute(false);
          break;
      }
    }

    private void KeyboardHook_KeyReleased(
      object sender,
      KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Scroll:
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
