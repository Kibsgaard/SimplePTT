using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ToggleMic.ViewModels
{
    [PropertyChanged.ImplementPropertyChanged]
    public class MainViewModel
    {
        private KeyboardHook keyboardHook = new KeyboardHook();

        public MainViewModel()
        {
            Active = !MicDevice.IsMuted;

            MuteCommand = new RelayCommand(o =>
            {
                Mute(Active);
            });

            keyboardHook.KeyPressed += KeyboardHook_KeyCombinationPressed;

            Mute(!Keyboard.IsKeyToggled(Key.Scroll));
        }

        private void KeyboardHook_KeyCombinationPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.KeyPressed)
            {
                case Key.Scroll:
                    Mute(Keyboard.IsKeyToggled(Key.Scroll));
                    break;
            }
        }

        private void Mute(bool mute)
        {
            MicDevice.Mute(mute);
            Active = !MicDevice.IsMuted;
        }

        public string Title { get; set; } = "Toggle - Microphone";

        [PropertyChanged.DoNotCheckEquality]
        public bool Active { get; set; }

        public ICommand MuteCommand { get; set; }

        [PropertyChanged.DoNotNotify]
        public CoreAudioController Controller { get; } = new CoreAudioController();

        [PropertyChanged.DoNotNotify]
        public CoreAudioDevice MicDevice
        {
            get
            {
                return Controller.GetCaptureDevices(AudioSwitcher.AudioApi.DeviceState.Active)
                    .FirstOrDefault(o => o.IsDefaultDevice);
            }
        }
    }
}
