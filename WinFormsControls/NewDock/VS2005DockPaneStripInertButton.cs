using System.Drawing;

namespace NewDock
{
    public sealed class VS2005DockPaneStripInertButton : InertButtonBase
    {
        private Bitmap m_image0, m_image1;

        public VS2005DockPaneStripInertButton(Bitmap image0, Bitmap image1)
            : base()
        {
            m_image0 = image0;
            m_image1 = image1;
        }

        private int m_imageCategory = 0;
        public int ImageCategory
        {
            get { return m_imageCategory; }
            set
            {
                if (m_imageCategory == value)
                    return;

                m_imageCategory = value;
                Invalidate();
            }
        }

        public override Bitmap Image
        {
            get { return ImageCategory == 0 ? m_image0 : m_image1; }
        }
    }
}