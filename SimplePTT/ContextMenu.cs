using System;
using System.Windows.Forms;

namespace SimplePTT
{
  class ContextMenu
  {
    public ContextMenuStrip Create()
    {
      // Add the default menu options.
      ContextMenuStrip menu = new ContextMenuStrip();
      ToolStripMenuItem item;
      ToolStripSeparator sep;

      // Name
      item = new ToolStripMenuItem();
      item.Text = Resources.ProgramTitle;
      item.Enabled = false;
      menu.Items.Add(item);

      // Separator
      sep = new ToolStripSeparator();
      menu.Items.Add(sep);

      // Exit
      item = new ToolStripMenuItem();
      item.Text = "Exit";
      item.Click += new System.EventHandler(Exit_Click);
      menu.Items.Add(item);

      return menu;
    }

    void Exit_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }
  }
}