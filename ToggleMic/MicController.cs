using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace ToggleMic
{
  public class MicController
  {
    private KeyboardHook keyboardHook = new KeyboardHook();

    public MicController()
    {
      this.keyboardHook.KeyPressed += this.KeyboardHook_KeyPressed;
      this.Mute(Keyboard.IsKeyToggled(Key.Scroll) == false);
      this.IsActive = (this.GetActiveDevice().IsMuted == false);
    }

    private void KeyboardHook_KeyPressed(
      object sender, 
      KeyPressedEventArgs e)
    {
      switch (e.KeyPressed)
      {
        case Key.Scroll:
          this.Mute(Keyboard.IsKeyToggled(Key.Scroll));
          break;
      }
    }

    private void Mute(bool mute)
    {
      CoreAudioDevice micDevice = this.GetActiveDevice();
      micDevice.Mute(mute);
      this.IsActive = (micDevice.IsMuted == false);
    }

    public bool IsActive { get; set; }

    private CoreAudioDevice GetActiveDevice()
    {
      return
        this.controller.GetCaptureDevices(DeviceState.Active)
          .FirstOrDefault(o => o.IsDefaultDevice);
    }

    private CoreAudioController controller = new CoreAudioController();
  }
}
