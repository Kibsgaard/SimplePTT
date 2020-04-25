using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;

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
    private TargetMicrophoneState targetMicrophoneState;
    private Guid? selectedDeviceGuid;
    private IDisposable muteChangedSubscription;

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

      SetAudioDevice(TargetMicrophoneState.Default, controller.DefaultCaptureDevice.Id);

      controller.AudioDeviceChanged.Subscribe(OnAudioDeviceChanged);

      Mute(true);
    }

    private void OnAudioDeviceChanged(DeviceChangedArgs value)
    {
      switch (value.ChangedType)
      {
        case DeviceChangedType.DefaultChanged:
        case DeviceChangedType.DeviceAdded:
        case DeviceChangedType.DeviceRemoved:
        case DeviceChangedType.StateChanged:
          SetAudioDevice(targetMicrophoneState, GetGuid());
            break;
        case DeviceChangedType.PropertyChanged:
        case DeviceChangedType.MuteChanged:
        case DeviceChangedType.VolumeChanged:
        case DeviceChangedType.PeakValueChanged:
          break;
      }
    }

    private Guid GetGuid()
    {
      switch (targetMicrophoneState)
      {
        case TargetMicrophoneState.Default:
          return GetDefaultAudioDevice().Id;
        case TargetMicrophoneState.DefaultCommunications:
          return GetDefaultCommunicationAudioDevice().Id;
        case TargetMicrophoneState.Specific:
          return selectedDeviceGuid.Value;
        default:
          throw new ArgumentOutOfRangeException();
      }
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

    public CoreAudioDevice GetDefaultAudioDevice()
    {
      return controller.DefaultCaptureDevice;
    }

    public CoreAudioDevice GetDefaultCommunicationAudioDevice()
    {
      return controller.DefaultCaptureCommunicationsDevice;
    }

    public TargetMicrophoneState GetTargetMicrophoneState()
    {
      return targetMicrophoneState;
    }

    public enum TargetMicrophoneState
    {
      Default,
      DefaultCommunications,
      Specific
    }

    public void SetAudioDevice(TargetMicrophoneState target, Guid guid)
    {
      targetMicrophoneState = target;
      selectedDeviceGuid = guid;

      if (muteChangedSubscription != null)
        muteChangedSubscription.Dispose();

      muteChangedSubscription = GetActiveDevice().MuteChanged.Subscribe(OnMuteChanged);
      Mute(IsMuted);
    }

    private bool IsValidMouseButton(int button)
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

    private DateTime mouseDownTime = new DateTime();
    private void MouseHook_ButtonDown(object sender, MouseEventArgs mouseEvt)
    {
      if (IsValidMouseButton(mouseEvt.Button))
      {
        mouseDownTime = DateTime.Now;

        Unmute();
      }
    }

    private void MouseHook_ButtonUp(object sender, MouseEventArgs mouseEvt)
    {
      if (IsValidMouseButton(mouseEvt.Button))
      {
        if(DateTime.Now - mouseDownTime > TimeSpan.FromMilliseconds(200))
          mouseHook.ConsumeMouseKey = true;

        DeferMute();
      }
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
      }
    }

    private CoreAudioDevice GetActiveDevice()
    {
      switch (targetMicrophoneState)
      {
        case TargetMicrophoneState.Default:
          return controller.DefaultCaptureDevice;
        case TargetMicrophoneState.DefaultCommunications:
          return controller.DefaultCaptureCommunicationsDevice;
        case TargetMicrophoneState.Specific:
          return controller.GetDevice(selectedDeviceGuid.Value);
      }

      return null;
    }

    private void OnMuteChanged(DeviceMuteChangedArgs value)
    {
      if(value.Device.Id == selectedDeviceGuid)
      {
        IsMuted = value.Device.IsMuted;
        MuteToggled?.Invoke(IsMuted);
      }
    }
  }
}
