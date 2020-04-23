using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace SimplePTT
{
  public class MicController : IDisposable
  {
    public const int MUTE_DELAY = 300;

    public bool IsMuted { get; private set; }
    public event Action<bool> MuteToggled;

    private readonly CoreAudioController controller;
    private readonly KeyboardHook keyboardHook;
    private readonly MouseHook mouseHook;

    private readonly Timer timer;
    private readonly Object muteLock;
    private Guid? selectedDeviceGuid;

    public MicController()
    {
      controller = new CoreAudioController();

      muteLock = new Object();
      timer = new Timer(MUTE_DELAY);
      timer.AutoReset = false;
      timer.Elapsed += Timer_Elapsed;

      keyboardHook = new KeyboardHook();
      keyboardHook.KeyDown += KeyboardHook_KeyDown;
      keyboardHook.KeyUp += KeyboardHook_KeyUp;

      mouseHook = new MouseHook();
      mouseHook.ButtonDown += MouseHook_ButtonDown;
      mouseHook.ButtonUp += MouseHook_ButtonUp;

      Mute(true);
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      Mute(true);
    }

    public void Dispose()
    {
      Mute(false);
      keyboardHook.Dispose();
      mouseHook.Dispose();
    }

    public Guid? GetSelectedDeviceId()
    {
      return selectedDeviceGuid;
    }

    public IEnumerable<CoreAudioDevice> GetAudioDevices()
    {
      return controller.GetCaptureDevices(DeviceState.Active);
    }

    public void SetAudioDevice(Guid guid)
    {
      selectedDeviceGuid = guid;
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

    private bool IsPushToTalkKey(KeyEventArgs keyEvt)
    {
      switch (keyEvt.Key)
      {
        case Key.F13:
        case Key.CapsLock:
          return true;
      }

      return false;
    }

    private bool IsToggleMuteKey(KeyEventArgs keyEvt)
    {
      switch (keyEvt.Key)
      {
        case Key.Pause:
        case Key.Oem3:
        case Key.Oem5:
          return true;
      }

      return false;
    }

    private void KeyboardHook_KeyDown(object sender, KeyEventArgs keyEvt)
    {
      if (IsPushToTalkKey(keyEvt))
        Unmute();
      else if (IsToggleMuteKey(keyEvt))
        ToggleMute();
    }

    private void ToggleMute()
    {
      Mute(!IsMuted);
    }

    private void KeyboardHook_KeyUp(object sender, KeyEventArgs keyEvt)
    {
      if (IsPushToTalkKey(keyEvt))
        DeferMute();
    }

    private void MouseHook_ButtonDown(object sender, MouseEventArgs mouseEvt)
    {
      if (IsValidButton(mouseEvt.Button))
        Unmute();
    }

    private void MouseHook_ButtonUp(object sender, MouseEventArgs mouseEvt)
    {
      if (IsValidButton(mouseEvt.Button))
        DeferMute();
    }

    private void DeferMute()
    {
      timer.Stop();
      timer.Start();
    }

    private void Unmute()
    {
      timer.Stop();
      Mute(false);
    }

    private void Mute(bool mute)
    {
      lock (muteLock)
      {
        GetActiveDevice().Mute(mute);
        IsMuted = mute;
        MuteToggled?.Invoke(mute);
      }
    }

    private CoreAudioDevice GetActiveDevice()
    {
      if (!selectedDeviceGuid.HasValue)
        selectedDeviceGuid = controller.GetCaptureDevices(DeviceState.Active).FirstOrDefault(o => o.IsDefaultDevice).Id;

      return controller.GetDevice(selectedDeviceGuid.Value);
    }
  }
}
