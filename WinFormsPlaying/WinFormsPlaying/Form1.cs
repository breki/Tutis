using System;
using System.Windows.Forms;

namespace WinFormsPlaying
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            button2.Controls.Add(button1);
            brekiTabPanel1.AddTab("Map", null, false);
            brekiTabPanel1.AddTab("Release Notes", null, true);
            brekiTabPanel1.AddTab("Rules Editor", null, true);
            brekiTabPanel1.TabCloseButtonClick += new EventHandler<BrekiTabsFlowEventArgs>(brekiTabPanel1_TabCloseButtonClick);
        }

        private void brekiTabPanel1_TabButtonClick(object sender, BrekiTabsFlowEventArgs e)
        {
            brekiTabPanel1.ActivateTab(e.TabIndex);
        }

        void brekiTabPanel1_TabCloseButtonClick(object sender, BrekiTabsFlowEventArgs e)
        {
            brekiTabPanel1.RemoveTab(e.TabIndex);
        }
    }
}
