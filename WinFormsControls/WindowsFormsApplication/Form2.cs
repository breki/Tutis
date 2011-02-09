using System;
using System.Windows.Forms;
using NewDock2;
using WeifenLuo.WinFormsUI.Docking;

namespace WindowsFormsApplication
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DockContent dockContent = new DockContent();
            dockContent.ShowHint = DockState.Document;
            dockContent.TabText = "ReSharper";
            dockContent.Show(dockPanel1, DockState.DockLeft);

            dockContent = new DockContent();
            dockContent.ShowHint = DockState.Document;
            dockContent.TabText = "BrekiTabPanel.cs";
            dockContent.Show(dockPanel1, DockState.Document);

            dockContent = new DockContent();
            dockContent.ShowHint = DockState.Document;
            dockContent.TabText = "ReSharper";
            dockContent.Show(dockPanel1, DockState.Document);

            brekiTabPanel1.AddTab("BrekiTabPanel.cs");
            brekiTabPanel1.AddTab("ReSharper");
            brekiTabPanel1.AddTab("File");
        }
    }
}
