using System.Windows.Forms;
using MbUnit.Framework;

namespace App
{
    public class Tests
    {
        [Test, Explicit]
        public void Test()
        {
            using (Form1 form = new Form1())
            {
                BrekiLayoutPanel panel = form.brekiLayoutPanel1;
                panel.SuspendLayout();

                Label label = new Label();
                label.Text = "User name:";
                label.AutoSize = true;
                SimpleLayoutElement layoutElement1 = new SimpleLayoutElement("label", label.PreferredWidth, label.PreferredHeight);
                panel.AddControl(label, layoutElement1);

                TextBox textBox = new TextBox();
                textBox.Text = "something";
                textBox.AutoSize = true;
                SimpleLayoutElement layoutElement2 = new SimpleLayoutElement("textbox1", textBox.PreferredSize.Width, textBox.PreferredHeight);
                textBox.Text = string.Empty;
                panel.AddControl(textBox, layoutElement2);
                layoutElement2
                    .LeftNoFlyZone(layoutElement1)
                    .RightDock(panel.RightBorder);

                panel.ResumeLayout(true);
                form.ShowDialog();
            }
        }
    }
}
