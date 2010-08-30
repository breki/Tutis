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
            //Application.Run(new LoginForm());

            using (IWindsorContainer container = new WindsorContainer())
            {
                container.Install(new WinFormsMvpInstaller());
                LoginPresenter presenter = container.Resolve<LoginPresenter>();
            }
        }
    }
}
