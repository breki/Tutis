using System.Windows.Forms;

namespace WinFormsMvp
{
    public partial class MyDocumentForm : Form, IMyDocumentView
    {
        public MyDocumentForm(IMainAppView mainAppView)
        {
            this.mainAppView = mainAppView;
            InitializeComponent();
            this.MdiParent = (Form)mainAppView;
        }

        public void Open()
        {
            WindowState = FormWindowState.Maximized;
            Show();
        }

        private readonly IMainAppView mainAppView;
    }
}
