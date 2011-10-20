using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MbUnit.Framework;

namespace Piksla
{
    public class MaskingTests
    {
        [Test]
        public void Test()
        {
            Bitmap bitmap3 = new Bitmap(500, 500);

            Graphics g3 = Graphics.FromImage(bitmap3);
            g3.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g3.SmoothingMode = SmoothingMode.HighQuality;

            Brush redBrush = new SolidBrush(Color.Red);
            Brush maskingBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));

            g3.CompositingMode = CompositingMode.SourceCopy;
            g3.CompositingQuality = CompositingQuality.HighQuality;
            g3.FillRectangle(redBrush, 10, 10, 110, 110);
            g3.FillRectangle(maskingBrush, 90, 90, 190, 190);

            bitmap3.Save("masking.png", ImageFormat.Png);
        }
    }
}