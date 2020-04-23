using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimplePTT
{
  public class TrayIcon : IDisposable
  {
    private readonly NotifyIcon icon;
    private readonly Icon mutedIcon;
    private readonly Icon openMicIcon;
    private readonly MicController micController;

    public TrayIcon(MicController micController)
    {
      icon = new NotifyIcon();
      mutedIcon = Resources.GreyMicrophone;
      openMicIcon = Resources.GreenMicrophone;

      SetMutedIcon(micController.IsMuted);
      micController.MuteToggled += SetMutedIcon;

      icon.Text = Resources.ProgramTitle;
      icon.Visible = true;
      icon.ContextMenuStrip = new ContextMenu(micController).Create();
      this.micController = micController;
    }

    public void SetMutedIcon(bool muted)
    {
      icon.Icon = muted ? mutedIcon : openMicIcon;
    }

    public void Dispose()
    {
      micController.MuteToggled -= SetMutedIcon;
      icon.Dispose();
    }
  }
}