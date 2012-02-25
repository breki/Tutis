using System.Drawing;
using System.Windows.Forms;

namespace WinFormsPlaying
{
    public class CloseButton : Button
    {
        public CloseButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
            TabStop = false;
            UseCompatibleTextRendering = true;

            //images.Images.Add(new Bitmap(@"D:\MyStuff\projects\Maperitive\trunk\Icons\candidates\crosses\cross-small.png"));
            //images.Images.Add(new Bitmap(@"D:\MyStuff\projects\Tutis\WinFormsPlaying\WinFormsPlaying\cross-button.png"));
            ImageList = images;
            ImageIndex = 0;

            AutoSize = false;
            Size = new Size(16, 16);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Blue;
            Padding = new Padding(0);
        }

        public override bool AutoSize
        {
            get { return true; }
            set
            {
            }
        }

        public override string Text
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            ImageIndex = 1;
            //FlatStyle = FlatStyle.Standard;
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            ImageIndex = 0;
            FlatStyle = FlatStyle.Flat;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // don't call the base class
            //base.OnPaintBackground(pevent);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);
            return new Size(images.Images[0].Size.Width, images.Images[0].Size.Height);
        }

        private ImageList images = new ImageList();
    }
}