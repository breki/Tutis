using System.Windows.Forms;

namespace WinFormsMvp
{
    public partial class MainForm : Form, IMainAppView
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void Run()
        {
            Application.Run(this);
        }
    }
}
