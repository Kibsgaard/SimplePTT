using System;
using System.Windows.Forms;

using SimplePTT;

static class Program
{
  public static MicController MicController;

  [STAThread]
  static void Main()
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.ApplicationExit += Application_ApplicationExit;

    using (TrayIcon tray = new TrayIcon())
    {
      MicController = new MicController(tray);
      Application.Run();
    }
  }

  static void Application_ApplicationExit(object sender, EventArgs e)
  {
    if (Program.MicController != null)
    {
      Program.MicController.Dispose();
    }
  }
}