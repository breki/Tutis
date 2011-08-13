using System;
using System.Windows.Forms;
using Castle.Windsor;

namespace WinFormsMvp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IWindsorContainer container = new WindsorContainer())
            {
                container.Install(new WinFormsMvpInstaller());
            }
        }
    }
}
