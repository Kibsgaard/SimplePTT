using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SimplePTT.Properties;

namespace SimplePTT
{
    public class TrayIcon : IDisposable
    {
        NotifyIcon icon;
        Icon mutedIcon;
        Icon openMicIcon;

        public TrayIcon()
        {
            icon = new NotifyIcon();
            mutedIcon = Resources.GreyMicrophone;
            openMicIcon = Resources.GreenMicrophone;

            icon.Text = "SimplePTT 1.0";
            icon.Visible = true;
            icon.ContextMenuStrip = new ContextMenu().Create();

        }

        public void SetMutedIcon(bool muted)
        {
            icon.Icon = muted ? mutedIcon : openMicIcon;
        }

        public void Dispose()
        {
            icon.Dispose();
        }
    }
}