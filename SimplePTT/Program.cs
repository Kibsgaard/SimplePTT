using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SimplePTT;

static class Program
{
  public static MicController MicController;

  [STAThread]
  static void Main()
  {
    Program.MicController = new MicController();

    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.ApplicationExit += Application_ApplicationExit;
    Application.Run(new Interface());
  }

  static void Application_ApplicationExit(object sender, EventArgs e)
  {
    if (Program.MicController != null)
    {
      Program.MicController.Dispose();
    }
  }
}