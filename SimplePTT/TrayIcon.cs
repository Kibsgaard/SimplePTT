using System;
using System.Diagnostics;
using System.Windows.Forms;
using SimplePTT.Properties;

namespace SimplePTT
{
  public class TrayIcon : IDisposable
  {
    NotifyIcon icon;

    public TrayIcon()
    {
      this.icon = new NotifyIcon();
    }

    public void Display()
    {
      this.icon.Icon = Resources.Microphone;
      this.icon.Text = "SimplePTT 1.0";
      this.icon.Visible = true;
      this.icon.ContextMenuStrip = new ContextMenu().Create();
    }

    public void Dispose()
    {
      this.icon.Dispose();
    }
  }
}