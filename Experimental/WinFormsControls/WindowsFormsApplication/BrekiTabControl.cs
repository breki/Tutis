using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public class BrekiTabControl : TabControl
    {
        public BrekiTabControl()
        {
            Padding = new Point(15, 5);
            //TabPages.Add(new TabPage("A very looooong text"));

            //this.SizeMode = TabSizeMode.Fixed;
            //this.ItemSize = new Size(300, 20);

            //SetStyle(ControlStyles.ResizeRedraw
            //    | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}