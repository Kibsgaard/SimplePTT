using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ToggleMic
{
  public class KeyEventArgs : EventArgs
  {
    public Key Key { get; private set; }
    public int VKCode { get; private set; }

    public KeyEventArgs(Key key, int vkCode)
    {
      this.Key = key;
      this.VKCode = vkCode;
    }
  }
}
