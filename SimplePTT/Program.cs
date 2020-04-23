using System;
using System.Windows.Forms;

using SimplePTT;

internal static class Program
{
  [STAThread]
  private static void Main()
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    using (var micController = new MicController())
    using (var tray = new TrayIcon(micController))
    {      
      Application.Run();
    }
  }

}