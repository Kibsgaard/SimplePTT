using System;
using System.Windows.Forms;

namespace SimplePTT
{
  internal class ContextMenu
  {
    private readonly MicController micController;

    public ContextMenu(MicController micController)
    {
      this.micController = micController;
    }

    public ContextMenuStrip Create()
    {
      var title = new ToolStripMenuItem(Resources.ProgramTitle);
      title.Enabled = false;

      var exit = new ToolStripMenuItem("Exit");      
      exit.Click += new EventHandler(ExitClick);

      var targetMicrophoneMenu = new ToolStripMenuItem("Target Microphone");
      targetMicrophoneMenu.DropDownOpening += (s, e) => TargetMicrophoneDropDownOpening(targetMicrophoneMenu);
      targetMicrophoneMenu.DropDownItems.Add(new ToolStripMenuItem("Failed to load device list"));

      var menu = new ContextMenuStrip();
      menu.Items.Add(title);
      menu.Items.Add(new ToolStripSeparator());           
      menu.Items.Add(targetMicrophoneMenu);
      menu.Items.Add(new ToolStripSeparator());
      menu.Items.Add(exit);

      return menu;
    }

    private void TargetMicrophoneDropDownOpening(ToolStripMenuItem targetMicrophoneMenu)
    {
      targetMicrophoneMenu.DropDownItems.Clear();
      var activeDeviceId = micController.GetSelectedDeviceId();

      foreach (var device in micController.GetAudioDevices())
      {
        var deviceItem = new ToolStripMenuItem(device.FullName);
        deviceItem.Click += (s, e) => SelectDevice(device.Id);
        deviceItem.Checked = device.Id == activeDeviceId;
        targetMicrophoneMenu.DropDownItems.Add(deviceItem);
      }
    }

    private void SelectDevice(Guid id)
    {
      micController.SetAudioDevice(id);
    }

    private void ExitClick(object sender, EventArgs e)
    {
      Application.Exit();
    }
  }
}