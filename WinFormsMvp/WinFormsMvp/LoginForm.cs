using System;
using System.Windows.Forms;

namespace WinFormsMvp
{
    public partial class LoginForm : Form, ILoginView
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public event EventHandler LoginButtonClicked;

        public string UserName
        {
            get { return TextBoxUserName.Text; }
            set { TextBoxUserName.Text = value; }
        }

        public string Password
        {
            get { return TextBoxPassword.Text; }
            set { TextBoxPassword.Text = value; }
        }

        public string Run ()
        {
            return ShowDialog().ToString();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (LoginButtonClicked != null)
                LoginButtonClicked(sender, e);
        }
    }
}
